using System;

namespace elbbp_core
{
    public class BytePusher
    {
        private const int KB = 1024;
        private const int MB = 1024 * KB;

        private readonly uint[] _palette;
        private readonly uint[] _frameBuffer;
        private readonly byte[] _audioBuffer;

        private byte[] _memory;

        public uint[] GetFrameBuffer() => _frameBuffer;
        public byte[] GetAudioBuffer() => _audioBuffer;

        public BytePusher()
        {
            _frameBuffer = new uint[256 * 256];

            _audioBuffer = new byte[256];
            Array.Fill<byte>(_audioBuffer, 0x80);

            _palette = new uint[256];
            GeneratePalette();
        }

        private void GeneratePalette()
        {
            int i = 0;

            for (uint r = 0; r < 6; r++)
            {
                for (uint g = 0; g < 6; g++)
                {
                    for (uint b = 0; b < 6; b++)
                    {
                        _palette[i++] = (0xFFu << 24) | ((r * 0x33) << 16) | ((g * 0x33) << 8) | (b * 0x33);
                    }
                }
            }
        }

        public void LoadRom(byte[] rom)
        {
            _memory = new byte[16 * MB + 8];

            Array.Copy(rom, 0, _memory, 0, rom.Length);
        }

        public unsafe void RunFrame(ushort input)
        {
            _memory[0] = (byte)(input >> 8);
            _memory[1] = (byte)input;

            // read 3 byte big endian program counter
            int pc = Read24BitWord(_memory, 2);
            int i = 65536;

            // run a frames worth of instructions
            do
            {
                // An instruction consists of 3 big-endian 24-bit addresses A,B,C stored consecutively in memory.
                // The operation performed is: Copy 1 byte from A to B, then jump to C.
                int a = Read24BitWord(_memory, pc);
                int b = Read24BitWord(_memory, pc + 3);

                _memory[b] = _memory[a];

                pc = Read24BitWord(_memory, pc + 6);
            } while (--i > 0);

            // copy pixel data to framebuffer, performing palette lookup
            int pixelBase = _memory[5] << 16;

            for (int yy = 0; yy < 256; yy++)
            {
                int rowBase = pixelBase | yy << 8;

                for (int xx = 0; xx < 256; xx++)
                {
                    _frameBuffer[yy * 256 + xx] = _palette[_memory[rowBase | xx]];
                }
            }

            int audioBaseAddress = _memory[6] << 16 | _memory[7] << 8;

            Buffer.BlockCopy(_memory, audioBaseAddress, _audioBuffer, 0, 256);

            for (int n = 0; n < _audioBuffer.Length; n++)
            {
                // samples provided to audio api are unsigned with a silence level of
                // 0x80 so offset the signed values from BytePusher
                _audioBuffer[n] = (byte)(_audioBuffer[n] + 0x80);
            }
        }

        private static int Read24BitWord(byte[] data, int offset)
        {
            return data[offset + 0] << 16 | data[offset + 1] << 8 | data[offset + 2];
        }
    }
}
