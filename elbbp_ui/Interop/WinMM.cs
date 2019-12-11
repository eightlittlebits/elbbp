using System.Runtime.InteropServices;
using System.Security;

namespace elbbp_ui.Interop.WinMM
{
    [SuppressUnmanagedCodeSecurity]
    internal static class NativeMethods
    {
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod", SetLastError = true)]
        internal static extern uint TimeBeginPeriod(uint uMilliseconds);
    }

}
