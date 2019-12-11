using System;
using System.ComponentModel;
using System.Windows.Forms;

using User32 = elbbp_ui.Interop.User32.NativeMethods;

namespace elbbp_ui
{

    [TypeDescriptionProvider(typeof(AbstractFormTypeDescriptionProvider<EmulatorForm, Form>))]
    public abstract class EmulatorForm : Form
    {
        private bool _running;

        private static bool ApplicationStillIdle => !User32.PeekMessage(out _, IntPtr.Zero, 0, 0, 0);

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Paused
        {
            get { return !_running; }
            set { _running = !value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Initialise();

            Application.Idle += (s, ev) => { while (_running && ApplicationStillIdle) { Update(); } };
            _running = true;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            var shutdownEventArgs = new ShutdownEventArgs();

            Shutdown(shutdownEventArgs);

            e.Cancel = shutdownEventArgs.Cancel;
        }


        protected abstract void Initialise();
        protected new abstract void Update();
        protected abstract void Shutdown(ShutdownEventArgs e);

        protected class ShutdownEventArgs : CancelEventArgs
        {

        }
    }
}
