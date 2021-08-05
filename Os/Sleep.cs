/* OS-independent inclusion of the delay routine */

namespace SharpDune.Os
{
    class Sleep
    {
#if !WITH_SDL && !WITH_SDL2
        internal static void SleepIdle() =>
            msleep(1);
#else
        internal static void SleepIdle() =>
            SleepAndProcessBackgroundTasks();
#endif

	    internal static void MSleep(int x) =>
            Thread.Sleep(x); //Sleep(x)
    }
}
