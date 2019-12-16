using System;
using SharpDX;
using SharpDX.DirectSound;
using SharpDX.Multimedia;

namespace elbbp_ui
{
    internal class DirectSoundAudioDevice<T> : IDisposable where T : unmanaged
    {
        private bool _disposed = false;

        private readonly T _silence;

        private DirectSound _directSound;
        private PrimarySoundBuffer _primarySoundBuffer;

        private SecondarySoundBuffer _soundBuffer;
        private int _soundBufferLength;
        private int _soundBufferCursor = -1;

        public DirectSoundAudioDevice(IntPtr windowHandle, int sampleRate, int channelCount, T silence)
        {
            _silence = silence;

            InitialiseDirectSound(windowHandle, sampleRate, channelCount);
        }

        private unsafe void InitialiseDirectSound(IntPtr windowHandle, int sampleRate, int channelCount)
        {
            var waveFormat = new WaveFormat(sampleRate, sizeof(T) * 8, channelCount);

            _directSound = new DirectSound();
            _directSound.SetCooperativeLevel(windowHandle, CooperativeLevel.Priority);

            var primaryBufferDescription = new SoundBufferDescription()
            {
                Flags = BufferFlags.PrimaryBuffer,
                AlgorithmFor3D = Guid.Empty
            };

            _primarySoundBuffer = new PrimarySoundBuffer(_directSound, primaryBufferDescription) { Format = waveFormat };

            _soundBufferLength = waveFormat.ConvertLatencyToByteSize(500);

            var secondaryBufferDescription = new SoundBufferDescription()
            {
                Format = waveFormat,
                Flags = BufferFlags.GetCurrentPosition2 | BufferFlags.GlobalFocus,
                BufferBytes = _soundBufferLength,
                AlgorithmFor3D = Guid.Empty,
            };

            _soundBuffer = new SecondarySoundBuffer(_directSound, secondaryBufferDescription);
            FillBuffer(_soundBuffer, _silence);
        }

        private void FillBuffer(SecondarySoundBuffer buffer, T value)
        {
            DataStream ds = buffer.Lock(0, 0, LockFlags.EntireBuffer, out DataStream ds2);

            for (int i = 0; i < ds.Length; i++)
            {
                ds.Write(value);
            }

            buffer.Unlock(ds, ds2);
        }

        public void Play() => _soundBuffer.Play(0, PlayFlags.Looping);

        public void Stop() => _soundBuffer.Stop();

        public unsafe void QueueAudio(T[] samples)
        {
            // the first time through begin writing from the write cursor
            if (_soundBufferCursor == -1)
            {
                _soundBuffer.GetCurrentPosition(out _, out int writeCursor);
                _soundBufferCursor = writeCursor + 256;
            }

            int bytesToWrite = samples.Length * sizeof(T);

            DataStream ds1 = _soundBuffer.Lock(_soundBufferCursor, bytesToWrite, LockFlags.None, out DataStream ds2);

            fixed (T* p = samples)
            {
                byte* b = (byte*)p;

                ds1.WriteRange((IntPtr)b, ds1.Length);

                if (ds2 != null)
                {
                    b += ds1.Length;
                    ds2.WriteRange((IntPtr)b, ds2.Length);
                }
            }

            _soundBuffer.Unlock(ds1, ds2);

            _soundBufferCursor = (_soundBufferCursor + bytesToWrite) % _soundBufferLength;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _soundBuffer?.Stop();
                _soundBuffer?.Dispose();

                _primarySoundBuffer?.Dispose();

                _directSound?.Dispose();

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
