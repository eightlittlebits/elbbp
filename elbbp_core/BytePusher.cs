using System;

namespace elbbp_core
{
    public class BytePusher
    {
        private const int KB = 1024;
        private const int MB = 1024 * KB;

        private readonly uint[] _palette;
        private readonly uint[] _frameBuffer;

        private byte[] _memory;

        public uint[] GetFrameBuffer() => _frameBuffer;

        public BytePusher()
        {
            _frameBuffer = new uint[256 * 256];
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
            int pc = _memory[2] << 16 | _memory[3] << 8 | _memory[4];
            int i = 65536;

            // run a frames worth of instructions
            do
            {
                int a = _memory[pc + 0] << 16 | _memory[pc + 1] << 8 | _memory[pc + 2];
                int b = _memory[pc + 3] << 16 | _memory[pc + 4] << 8 | _memory[pc + 5];

                _memory[b] = _memory[a];

                pc = _memory[pc + 6] << 16 | _memory[pc + 7] << 8 | _memory[pc + 8];
            } while (--i > 0);

            // copy pixel data to framebuffer, performing palette lookup
            int pixelBase = _memory[5] << 16;

            for (int yy = 0; yy < 256; yy++)
            {
                int rowBase = pixelBase | yy << 8;

                for (int xx = 0; xx < 256; xx++)
                {
                    _frameBuffer[yy * 256 + xx] = _palette[_memory[rowBase | pixelBase | xx]];
                }
            }
        }
    }
}
