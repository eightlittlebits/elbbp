using System;
using System.Windows.Forms;

using WinMM = elbbp_ui.Interop.WinMM.NativeMethods;

namespace elbbp_ui
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // set timer resolution to 1ms to try and get the sleep accurate in the wait loop
            WinMM.TimeBeginPeriod(1);

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
