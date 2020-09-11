/* Platform dependant thread implementation for win32 */

using System;
using Vanara.PInvoke;

namespace SharpDune
{
    //TODO: Use SDL or .NET Thread/Semaphore?
    class Thread
    {
        internal static Kernel32.SafeHTHREAD/*Thread*/ Thread_Create(Kernel32.ThreadProc/*ThreadProc*/ proc, IntPtr data) =>
            Kernel32.CreateThread(null/*NULL*/, 0, proc, data, 0, out uint _/*NULL*/);

        internal static void Thread_Wait(Kernel32.SafeHTHREAD/*Thread*/ thread, out uint/*ThreadStatus*/ status)
        {
            Kernel32.WaitForSingleObject(thread, Kernel32.INFINITE);
            Kernel32.GetExitCodeThread(thread, out status);
            Kernel32.CloseHandle(thread.DangerousGetHandle());
        }

        internal static Kernel32.SafeSemaphoreHandle/*Semaphore*/ Semaphore_Create(int value) =>
            Kernel32.CreateSemaphore(null/*NULL*/, value, 1, null/*NULL*/);

        internal static void Semaphore_Destroy(Kernel32.SafeSemaphoreHandle/*Semaphore*/ sem) =>
            Kernel32.CloseHandle(sem.DangerousGetHandle());

        internal static bool Semaphore_Lock(Kernel32.SafeSemaphoreHandle/*Semaphore*/ sem) =>
            Kernel32.WaitForSingleObject(sem, Kernel32.INFINITE) == Kernel32.WAIT_STATUS.WAIT_OBJECT_0;

        internal static bool Semaphore_TryLock(Kernel32.SafeSemaphoreHandle/*Semaphore*/ sem) =>
            Kernel32.WaitForSingleObject(sem, 0) == Kernel32.WAIT_STATUS.WAIT_OBJECT_0;

        internal static bool Semaphore_Unlock(Kernel32.SafeSemaphoreHandle/*Semaphore*/ sem) =>
            Kernel32.ReleaseSemaphore(sem, 1, out int _/*NULL*/);
    }
}
