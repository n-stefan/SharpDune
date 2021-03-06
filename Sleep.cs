﻿/* OS-independent inclusion of the delay routine */

namespace SharpDune
{
    class Sleep
    {
#if !WITH_SDL && !WITH_SDL2
        internal static void sleepIdle() =>
            msleep(1);
#else
        internal static void sleepIdle() =>
            Timer.SleepAndProcessBackgroundTasks();
#endif

	    internal static void msleep(int x) =>
            System.Threading.Thread.Sleep(x); //Sleep(x)
    }
}
