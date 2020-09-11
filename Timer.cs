/* Timer */

using System;
using System.Diagnostics;
using Vanara.PInvoke;

namespace SharpDune
{
    enum TimerType
	{
		TIMER_GUI = 1,                                          /*!< The identifier for GUI timer. */
		TIMER_GAME = 2                                          /*!< The identifier for Game timer. */
	}

	class TimerNode
	{
		internal uint usec_left;
		internal uint usec_delay;
		internal Action callback;
		internal bool callonce;
	}

	class Timer
	{
		volatile internal static uint g_timerGUI = 0;                                      /*!< Tick counter. Increases with 1 every tick when Timer 1 is enabled. Used for GUI. */
		volatile internal static uint g_timerGame = 0;                                     /*!< Tick counter. Increases with 1 every tick when Timer 2 is enabled. Used for game timing (units, ..). */
		volatile internal static uint g_timerInput = 0;                                    /*!< Tick counter. Increases with 1 every tick. Used for input timing. */
		volatile internal static uint g_timerSleep = 0;                                    /*!< Tick counter. Increases with 1 every tick. Used for sleeping. */
		volatile internal static uint g_timerTimeout = 0;                                  /*!< Tick counter. Decreases with 1 every tick when non-zero. Used to timeout. */

		volatile static int s_timer_count = 0;

		static ushort s_timersActive = 0;

		static TimerNode[] s_timerNodes = null;
		static int s_timerNodeCount = 0;
		static int s_timerNodeSize = 0;

		static int s_timerTime;
		static uint s_timerLastTime;

		const uint s_timerSpeed = 1000000 / 120; /* Our timer runs at 120Hz */

		static /*HANDLE*/IntPtr s_timerMainThread = IntPtr.Zero;
		static /*HANDLE*/Kernel32.TimerQueueTimerHandle s_timerThread = Kernel32.TimerQueueTimerHandle.NULL;
        
		readonly static Kernel32.WaitOrTimerCallback Timer_InterruptWindows_Del = Timer_InterruptWindows;

		/*
		 * Sleep for an amount of ticks.
		 * @param ticks The amount of ticks to sleep.
		 */
		internal static void Timer_Sleep(ushort ticks)
		{
			uint tick = g_timerSleep + ticks;
			while (tick >= g_timerSleep) Sleep.sleepIdle();
		}

		/*
		 * Set timers on and off.
		 *
		 * @param timer The timer to switch.
		 * @param set True sets the timer on, false sets it off.
		 * @return True if timer was set, false if it was not set.
		 */
		internal static bool Timer_SetTimer(TimerType timer, bool set)
		{
			byte t;
			bool ret;

			t = (byte)(1 << ((byte)timer - 1));
			ret = (s_timersActive & t) != 0;

			if (set)
			{
				s_timersActive |= t;
			}
			else
			{
				s_timersActive &= (ushort)~t;
			}

			return ret;
		}

		/*
		 * Add a timer.
		 * @param callback the callback for the timer.
		 * @param usec_delay The interval of the timer.
		 */
		internal static void Timer_Add(Action callback, uint usec_delay, bool callonce)
		{
			TimerNode node;
			if (s_timerNodeCount == s_timerNodeSize)
			{
				s_timerNodeSize += 2;
				Array.Resize(ref s_timerNodes, s_timerNodeSize); //s_timerNodes = (TimerNode *)realloc(s_timerNodes, s_timerNodeSize * sizeof(TimerNode));
				s_timerNodes[s_timerNodeSize - 2] = new TimerNode();
				s_timerNodes[s_timerNodeSize - 1] = new TimerNode();
			}
			node = s_timerNodes[s_timerNodeCount++];

			node.usec_left = usec_delay;
			node.usec_delay = usec_delay;
			node.callback = callback;
			node.callonce = callonce;
		}

		internal static uint Timer_GetTime() =>
			(uint)Environment.TickCount;

		/*
		 * Uninitialize the timer.
		 */
		internal static void Timer_Uninit()
		{
			Timer_InterruptSuspend();

			Kernel32.CloseHandle(s_timerMainThread);

			s_timerNodes = null; //free(s_timerNodes);
			s_timerNodeCount = 0;
			s_timerNodeSize = 0;
		}

		/*
		 * Suspend the timer interrupt handling.
		 */
		static void Timer_InterruptSuspend()
		{
			if (s_timerThread != IntPtr.Zero) Kernel32.DeleteTimerQueueTimer(Kernel32.TimerQueueHandle.NULL, s_timerThread, Kernel32.SafeEventHandle.Null);

			s_timerThread = IntPtr.Zero;
		}

		/*
		 * Initialize the timer.
		 */
		internal static void Timer_Init()
		{
			s_timerLastTime = Timer_GetTime();

			s_timerTime = (int)(s_timerSpeed / 1000);

			Kernel32.DuplicateHandle(Kernel32.GetCurrentProcess(), (IntPtr)Kernel32.GetCurrentThread(), Kernel32.GetCurrentProcess(), out s_timerMainThread, 0, false, Kernel32.DUPLICATE_HANDLE_OPTIONS.DUPLICATE_SAME_ACCESS);

			Timer_InterruptResume();
		}

		/*
		 * Resume the timer interrupt handling.
		 */
		static void Timer_InterruptResume() =>
			Kernel32.CreateTimerQueueTimer(out s_timerThread, Kernel32.TimerQueueHandle.NULL, Timer_InterruptWindows_Del, IntPtr.Zero, (uint)s_timerTime, (uint)s_timerTime, Kernel32.WT.WT_EXECUTEINTIMERTHREAD);

		static void Timer_InterruptWindows(IntPtr arg, bool TimerOrWaitFired)
		{
			//VARIABLE_NOT_USED(arg);
			//VARIABLE_NOT_USED(TimerOrWaitFired);

			Kernel32.SuspendThread(s_timerMainThread);

			s_timer_count++;

			Kernel32.ResumeThread(s_timerMainThread);
		}

		internal static void SleepAndProcessBackgroundTasks()
		{
			while (s_timer_count == 0)
			{
				System.Threading.Thread.Sleep(2); /* TODO : use a semaphore */
			}
			/* timer signal SIGALRM has been triggered */
			if (s_timer_count > 1)
			{
				Trace.WriteLine($"WARNING: s_timer_count = {s_timer_count}");
			}
			s_timer_count = 0;
			Timer_InterruptRun(0);
			if (s_timer_count > 0)
			{
				/* one more iteration if SIGALRM has been triggered
				 * during Timer_InterruptRun() */
				s_timer_count = 0;
				Timer_InterruptRun(1);  /* don't run "callonce" timers */
			}
		}

		static bool timerLock = false;
		/*
		 * Run the timer interrupt handler.
		 */
		static void Timer_InterruptRun(int arg)
		{
			TimerNode node;
			uint new_time, usec_delta, delta;
			int i;

			/* Lock the timer, to avoid double-calls */
			if (timerLock) return;
			timerLock = true;

			/* Calculate the time between calls */
			new_time = Timer_GetTime();
			usec_delta = (new_time - s_timerLastTime) * 1000;
			s_timerLastTime = new_time;

			/* Walk all our timers, see which (and how often) it should be triggered */
			for (i = 0; i < s_timerNodeCount; i++)
			{
				node = s_timerNodes[i];

				delta = usec_delta;

				/* No delay means: as often as possible, but don't worry about it */
				if (node.usec_delay == 0)
				{
					node.callback?.Invoke();
					continue;
				}

				if (node.callonce)
				{
					if (node.usec_left <= delta)
					{
						delta -= node.usec_left;
						node.usec_left = node.usec_delay;
						if (arg == 0) node.callback?.Invoke();
						while (node.usec_left <= delta) delta -= node.usec_left;
					}
				}
				else while (node.usec_left <= delta)
				{
					delta -= node.usec_left;
					node.usec_left = node.usec_delay;
					node.callback?.Invoke();
				}
				node.usec_left -= delta;
			}

			timerLock = false;
		}

		/*
		 * Handle game timers.
		 */
		internal static void Timer_Tick()
		{
			if ((s_timersActive & (ushort)TimerType.TIMER_GUI) != 0) g_timerGUI++;
			if ((s_timersActive & (ushort)TimerType.TIMER_GAME) != 0) g_timerGame++;
			g_timerInput++;
			g_timerSleep++;

			if (g_timerTimeout != 0) g_timerTimeout--;
		}



		//volatile internal static uint g_timerGUI = 0;           /*!< Tick counter. Increases with 1 every tick when Timer 1 is enabled. Used for GUI. */
		//volatile internal static uint g_timerGame = 0;          /*!< Tick counter. Increases with 1 every tick when Timer 2 is enabled. Used for game timing (units, ..). */
		//volatile internal static uint g_timerInput = 0;         /*!< Tick counter. Increases with 1 every tick. Used for input timing. */
		//volatile internal static uint g_timerSleep = 0;         /*!< Tick counter. Increases with 1 every tick. Used for sleeping. */
		//volatile internal static uint g_timerTimeout = 0;       /*!< Tick counter. Decreases with 1 every tick when non-zero. Used to timeout. */

		//volatile static int s_timer_count = 0;

		//static ushort s_timersActive = 0;

		//static uint s_timerLastTime;

		//static TimerNode[] s_timerNodes;
		//static int s_timerNodeCount;
		//static int s_timerNodeSize;

		//static int s_timerTime;

		//const uint s_timerSpeed = 1000000 / 120; /* Our timer runs at 120Hz */

		//static /*HANDLE*/IntPtr s_timerMainThread = IntPtr.Zero;
		//static /*HANDLE*/Kernel32.TimerQueueTimerHandle s_timerThread = Kernel32.TimerQueueTimerHandle.NULL;

		//static readonly Kernel32.WaitOrTimerCallback Timer_InterruptWindows_Del = Timer_InterruptWindows;

		///*
		//       * Set timers on and off.
		//       *
		//       * @param timer The timer to switch.
		//       * @param set True sets the timer on, false sets it off.
		//       * @return True if timer was set, false if it was not set.
		//       */
		//internal static bool Timer_SetTimer(TimerType timer, bool set)
		//{
		//	byte t;
		//	bool ret;

		//	t = (byte)(1 << (byte)(timer - 1));
		//	ret = (s_timersActive & t) != 0;

		//	if (set)
		//	{
		//		s_timersActive |= t;
		//	}
		//	else
		//	{
		//		s_timersActive &= (ushort)~t;
		//	}

		//	return ret;
		//}

		//internal static void SleepAndProcessBackgroundTasks()
		//{
		//	while (s_timer_count == 0)
		//	{
		//		System.Threading.Thread.Sleep(2); //Sleep(2); /* use a semaphore */
		//	}
		//	/* timer signal SIGALRM has been triggered */
		//	if (s_timer_count > 1)
		//	{
		//		Trace.WriteLine($"WARNING: s_timer_count = {s_timer_count}");
		//	}
		//	s_timer_count = 0;
		//	Timer_InterruptRun(0);
		//	if (s_timer_count > 0)
		//	{
		//		/* one more iteration if SIGALRM has been triggered
		//		 * during Timer_InterruptRun() */
		//		s_timer_count = 0;
		//		Timer_InterruptRun(1);  /* don't run "callonce" timers */
		//	}
		//}

		//static bool timerLock = false;
		///*
		// * Run the timer interrupt handler.
		// */
		//static void Timer_InterruptRun(int arg)
		//{
		//	TimerNode node;
		//	uint new_time, usec_delta, delta;
		//	int i;

		//	/* Lock the timer, to avoid double-calls */
		//	if (timerLock) return;
		//	timerLock = true;

		//	/* Calculate the time between calls */
		//	new_time = Timer_GetTime();
		//	usec_delta = (new_time - s_timerLastTime) * 1000;
		//	s_timerLastTime = new_time;

		//	/* Walk all our timers, see which (and how often) it should be triggered */
		//	//node = s_timerNodes[0];
		//	for (i = 0; i < s_timerNodeCount; i++)
		//	{ //, node++) {
		//		node = s_timerNodes[i];
		//		delta = usec_delta;

		//		/* No delay means: as often as possible, but don't worry about it */
		//		if (node.usec_delay == 0)
		//		{
		//			node.callback();
		//			continue;
		//		}

		//		if (node.callonce)
		//		{
		//			if (node.usec_left <= delta)
		//			{
		//				delta -= node.usec_left;
		//				node.usec_left = node.usec_delay;
		//				if (arg == 0) node.callback();
		//				while (node.usec_left <= delta) delta -= node.usec_left;
		//			}
		//		}
		//		else while (node.usec_left <= delta)
		//		{
		//			delta -= node.usec_left;
		//			node.usec_left = node.usec_delay;
		//			node.callback();
		//		}
		//		node.usec_left -= delta;
		//	}

		//	timerLock = false;
		//}

		//internal static uint Timer_GetTime() =>
		//	(uint)Environment.TickCount;

		///*
		// * Sleep for an amount of ticks.
		// * @param ticks The amount of ticks to sleep.
		// */
		//internal static void Timer_Sleep(ushort ticks)
		//{
		//	uint tick = g_timerSleep + ticks;
		//	while (tick >= g_timerSleep) Sleep.sleepIdle();
		//}

		///*
		// * Initialize the timer.
		// */
		//internal static void Timer_Init()
		//{
		//	s_timerLastTime = Timer_GetTime();

		//	//#if _WIN32

		//	s_timerTime = (int)(s_timerSpeed / 1000);

		//	Kernel32.DuplicateHandle(Kernel32.GetCurrentProcess(), (IntPtr)Kernel32.GetCurrentThread(), Kernel32.GetCurrentProcess(), out s_timerMainThread, 0, false,
		//		Kernel32.DUPLICATE_HANDLE_OPTIONS.DUPLICATE_SAME_ACCESS);

		//	//#elif TOS
		//	//	(void)Cconws("Timer_Init()\r\n");
		//	//	/* see http://toshyp.atari.org/en/004009.html#Xbtimer */
		//	//	/* s_timerSpeed * 614400 / 10000 */
		//	//#if 0
		//	//	Xbtimer(0/* Timer A */, 1/* divider = 4 */, s_timerSpeed * 6144 / 10000, Timer_Handler);
		//	//#endif
		//	//#elif DOS
		//	//	/* */
		//	//#else
		//	//	s_timerTime.it_value.tv_sec = 0;
		//	//	s_timerTime.it_value.tv_usec = s_timerSpeed;
		//	//	s_timerTime.it_interval.tv_sec = 0;
		//	//	s_timerTime.it_interval.tv_usec = s_timerSpeed;

		//	//	{
		//	//		struct sigaction timerSignal;

		//	//		sigemptyset(&timerSignal.sa_mask);
		//	//		timerSignal.sa_handler = Timer_Handler;
		//	//		timerSignal.sa_flags   = 0;
		//	//		sigaction(SIGALRM, &timerSignal, NULL);
		//	//	}
		//	//#endif /* _WIN32 */
		//	//#if !TOS && !DOS

		//	Timer_InterruptResume();

		//	//#endif /* !TOS && !DOS */
		//}

		///*
		// * Suspend the timer interrupt handling.
		// */
		//static void Timer_InterruptSuspend()
		//{
		//	//#if _WIN32

		//	if (s_timerThread != Kernel32.TimerQueueTimerHandle.NULL) Kernel32.DeleteTimerQueueTimer(Kernel32.TimerQueueHandle.NULL, s_timerThread, Kernel32.SafeEventHandle.Null);
		//	s_timerThread = Kernel32.TimerQueueTimerHandle.NULL;

		//	//#else
		//	//setitimer(ITIMER_REAL, NULL, NULL);
		//	//#endif /* _WIN32 */
		//}

		///*
		// * Resume the timer interrupt handling.
		// */
		//static void Timer_InterruptResume()
		//{
		//	//#if _WIN32

		//	Kernel32.CreateTimerQueueTimer(out s_timerThread, Kernel32.TimerQueueHandle.NULL, Timer_InterruptWindows_Del, IntPtr.Zero, (uint)s_timerTime, (uint)s_timerTime,
		//		Kernel32.WT.WT_EXECUTEINTIMERTHREAD);

		//	//#else
		//	//setitimer(ITIMER_REAL, &s_timerTime, NULL);
		//	//#endif /* _WIN32 */
		//}

		//static void Timer_InterruptWindows(/*LPVOID*/IntPtr arg, /*BOOLEAN*/bool TimerOrWaitFired)
		//{
		//	//VARIABLE_NOT_USED(arg);
		//	//VARIABLE_NOT_USED(TimerOrWaitFired);

		//	Kernel32.SuspendThread(s_timerMainThread);

		//	#if WITH_SDL || WITH_SDL2
		//	s_timer_count++;
		//	#else
		//	Timer_InterruptRun(0);
		//	#endif //WITH_SDL || WITH_SDL2

		//	Kernel32.ResumeThread(s_timerMainThread);
		//}

		///*
		// * Add a timer.
		// * @param callback the callback for the timer.
		// * @param usec_delay The interval of the timer.
		// */
		//internal static void Timer_Add(Action callback, uint usec_delay, bool callonce)
		//{
		//	TimerNode node;
		//	if (s_timerNodeCount == s_timerNodeSize)
		//	{
		//		s_timerNodeSize += 2;
		//		Array.Resize(ref s_timerNodes, s_timerNodeSize); //s_timerNodes = (TimerNode *)realloc(s_timerNodes, s_timerNodeSize * sizeof(TimerNode));
		//		s_timerNodes[s_timerNodeSize - 1] = new TimerNode();
		//		s_timerNodes[s_timerNodeSize - 2] = new TimerNode();
		//	}
		//	node = s_timerNodes[s_timerNodeCount++];

		//	node.usec_left = usec_delay;
		//	node.usec_delay = usec_delay;
		//	node.callback = callback;
		//	node.callonce = callonce;
		//}

		///*
		// * Handle game timers.
		// */
		//internal static void Timer_Tick()
		//{
		//	if ((s_timersActive & (ushort)TimerType.TIMER_GUI)  != 0) g_timerGUI++;
		//	if ((s_timersActive & (ushort)TimerType.TIMER_GAME) != 0) g_timerGame++;
		//	g_timerInput++;
		//	g_timerSleep++;

		//	if (g_timerTimeout != 0) g_timerTimeout--;
		//}

		///*
		// * Uninitialize the timer.
		// */
		//internal static void Timer_Uninit()
		//{
		//	//#if !TOS && !DOS
		//	Timer_InterruptSuspend();
		//	//#endif // !TOS && !DOS
		//	//#if _WIN32
		//	Kernel32.CloseHandle(s_timerMainThread);
		//	//#endif // _WIN32

		//	s_timerNodes = null; //free(s_timerNodes);
		//	s_timerNodeCount = 0;
		//	s_timerNodeSize = 0;
		//}
	}
}
