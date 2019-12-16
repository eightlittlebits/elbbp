using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using elbbp_core;
using elbbp_ui.Interop.Gdi32;

using Gdi32 = elbbp_ui.Interop.Gdi32.NativeMethods;

namespace elbbp_ui
{
    public partial class MainForm : EmulatorForm
    {
        private static readonly string ProgramNameVersion = $"{Application.ProductName} v{Application.ProductVersion}";
        private static readonly long StopwatchFrequency = Stopwatch.Frequency;

        private const int DisplayWidth = 256;
        private const int DisplayHeight = 256;

        private bool _running;

        private readonly double _targetFrameTicks = StopwatchFrequency / 60.0;
        private long _lastFrameTimestamp;
        private double _lastFrameTime;

        private GdiBitmap _backBuffer;
        private DirectSoundAudioDevice<byte> _audioDevice;

        private BytePusher _bytePusher;
        private ushort _inputState;

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void Initialise()
        {
            PrepareUserInterface();

            _audioDevice = new DirectSoundAudioDevice<byte>(Handle, 256 * 60, 1, 0x80);
            _audioDevice.Play();
        }

        private void PrepareUserInterface()
        {
            Text = $"{ProgramNameVersion}";

            ClientSize = new System.Drawing.Size(DisplayWidth, DisplayHeight);

            _backBuffer = new GdiBitmap(DisplayHeight, DisplayWidth);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // returns true if the key was processed, if not pass to
            // default handler
            if (!ProcessKeyboard(e.KeyCode, true))
            {
                base.OnKeyDown(e);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            // returns true if the key was processed, if not pass to
            // default handler
            if (!ProcessKeyboard(e.KeyCode, false))
            {
                base.OnKeyUp(e);
            }
        }

        private bool ProcessKeyboard(Keys key, bool pressed)
        {
            ushort inputBit;

            switch (key)
            {
                case Keys.D7: inputBit = 1 << 0x1; break;
                case Keys.D8: inputBit = 1 << 0x2; break;
                case Keys.D9: inputBit = 1 << 0x3; break;
                case Keys.D0: inputBit = 1 << 0xC; break;

                case Keys.Y: inputBit = 1 << 0x4; break;
                case Keys.U: inputBit = 1 << 0x5; break;
                case Keys.I: inputBit = 1 << 0x6; break;
                case Keys.O: inputBit = 1 << 0xD; break;

                case Keys.H: inputBit = 1 << 0x7; break;
                case Keys.J: inputBit = 1 << 0x8; break;
                case Keys.K: inputBit = 1 << 0x9; break;
                case Keys.L: inputBit = 1 << 0xE; break;

                case Keys.N: inputBit = 1 << 0xA; break;
                case Keys.M: inputBit = 1 << 0x0; break;
                case Keys.Oemcomma: inputBit = 1 << 0xB; break;
                case Keys.OemPeriod: inputBit = 1 << 0xF; break;

                default:
                    return false;
            }

            if (pressed)
            {
                _inputState |= inputBit;
            }
            else
            {
                _inputState &= (ushort)~inputBit;
            }

            return true;
        }

        protected override void Update()
        {
            if (!_running)
                return;

            // frame
            _bytePusher.RunFrame(_inputState);

            // render
            OutputAudio();
            RenderDisplayToDisplayBuffer();
            UpdateWindow();

            // sleep
            WaitForFrameTime();

            Text = string.Format("{0} - {1:00.0}fps", ProgramNameVersion, 1 / _lastFrameTime);
        }

        private void OutputAudio() => _audioDevice.QueueAudio(_bytePusher.GetAudioBuffer());

        private unsafe void RenderDisplayToDisplayBuffer()
        {
            long size = _backBuffer.Width * _backBuffer.Height * _backBuffer.BytesPerPixel;

            uint[] frameBuffer = _bytePusher.GetFrameBuffer();

            // copy frame to backbuffer
            fixed (uint* pFrameBuffer = _bytePusher.GetFrameBuffer())
            {
                Buffer.MemoryCopy(pFrameBuffer, _backBuffer.BitmapData.ToPointer(), size, frameBuffer.Length * sizeof(uint));
            }
        }

        private void UpdateWindow()
        {
            using var deviceContext = new GdiDeviceContext(_renderPanel.Handle);

            Gdi32.StretchBlt(deviceContext, 0, 0, _renderPanel.Width, _renderPanel.Height,
                     _backBuffer.DeviceContext, 0, 0, _backBuffer.Width, _backBuffer.Height,
                     RasterOp.SRCCOPY);
        }


        private void WaitForFrameTime()
        {
            long elapsedTicks = Stopwatch.GetTimestamp() - _lastFrameTimestamp;

            if (elapsedTicks < _targetFrameTicks)
            {
                // get ms to sleep for, cast to int to truncate to nearest millisecond
                // take 1 ms off the sleep time as we don't always hit the sleep exactly, trade
                // burning extra cpu in the spin loop for accuracy
                int sleepMilliseconds = (int)((_targetFrameTicks - elapsedTicks) * 1000 / StopwatchFrequency) - 1;

                if (sleepMilliseconds > 0)
                {
                    Thread.Sleep(sleepMilliseconds);
                }

                // spin for the remaining partial millisecond to hit target frame rate
                while ((Stopwatch.GetTimestamp() - _lastFrameTimestamp) < _targetFrameTicks) ;
            }

            long endFrameTimestamp = Stopwatch.GetTimestamp();

            _lastFrameTime = (double)(endFrameTimestamp - _lastFrameTimestamp) / StopwatchFrequency;

            _lastFrameTimestamp = endFrameTimestamp;
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            e.Effect = DragDropEffects.None;

            if (e.Data.GetDataPresent(DataFormats.FileDrop) && e.AllowedEffect.HasFlag(DragDropEffects.Copy))
            {
                if (e.Data.GetData(DataFormats.FileDrop) is string[] strings && strings.Length == 1)
                {
                    string extension = Path.GetExtension(strings[0]);

                    if (string.Compare(extension, ".BytePusher", true) == 0 || string.Compare(extension, ".bp", true) == 0)
                    {
                        e.Effect = DragDropEffects.Copy;
                    }
                }
            }
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);

            if (e.Data.GetData(DataFormats.FileDrop, false) is string[] files && files.Length == 1)
            {
                RunBytePusherWithRom(File.ReadAllBytes(files[0]));
            }
        }

        private void RunBytePusherWithRom(byte[] rom)
        {
            _bytePusher = new BytePusher();
            _bytePusher.LoadRom(rom);
            _running = true;
        }

        protected override void Shutdown(ShutdownEventArgs e)
        {

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                _backBuffer.Dispose();

                _audioDevice.Stop();
                _audioDevice.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
