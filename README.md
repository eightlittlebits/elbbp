# elbbp - [![Build status](https://ci.appveyor.com/api/projects/status/fo9v56l3j40519e2?svg=true)](https://ci.appveyor.com/project/eightlittlebits/elbbp)

<p align="center">
  <img src="https://eightlittlebits.github.io/elbbp/screenshots/scrolling-logo.png" />
  <img src="https://eightlittlebits.github.io/elbbp/screenshots/audio-test.png" />
  <img src="https://eightlittlebits.github.io/elbbp/screenshots/nyan.png" />
</p>

## what

A C# implementation of BytePusher; a minimalist virtual machine invented by Javamannen in 2010.

* big-endian single instruction ByteByteJump processor running at ~3.93MHz
* 24-bit memory bus supporting 16 MiB of memory
* 256x256 pixel, 216 colour, 60fps display
* 8-bit audio output
* 16 key keyboard

## what is ByteByteJump?

ByteByteJump is an extremely simple One Instruction Set Computer (OISC). Its single instruction copies 1 byte from a memory location to another, and then performs an unconditional jump.

An instruction consists of 3 addresses stored consecutively in memory:

	A,B,C

A is the source address, B is the destination address, and C is the jump address.

Arithmetic operations and conditional jumps can be performed by using self-modifying code and lookup tables.

## why

It made for a fun afternoon project. Failing to make progress on my other emulator projects recently I though it might be fun to take on a simple one that could actually be "finished" quickly. The main implementation was completed in one afternoon with audio being added the following day.

## how

Drag and drop a BytePusher file onto the window to load it and start it running, there are sample roms available on the esolangs BytePusher page listed below.

Bytepusher uses a 4x4 hex keyboard, the same layout as CHIP-8. This is mapped as follows:

<table>
    <tr>
        <th>VM</th>
        <th>=></th>
        <th>Keyboard</th>
    </tr>
    <tr>
        <td>
            <table>
				<tr><td>1</td><td>2</td><td>3</td><td>C</td></tr>
				<tr><td>4</td><td>5</td><td>6</td><td>D</td></tr>
				<tr><td>7</td><td>8</td><td>9</td><td>E</td></tr>
				<tr><td>A</td><td>0</td><td>B</td><td>F</td></tr>
            </table>
        </td>
        <td>
			<table>
				<tr><td>=&gt;</td></tr>
				<tr><td>=&gt;</td></tr>
				<tr><td>=&gt;</td></tr>
				<tr><td>=&gt;</td></tr>
			</table>
        </td>
		<td>
			<table>
				<tr><td>7</td><td>8</td><td>9</td><td>0</td></tr>
				<tr><td>Y</td><td>U</td><td>I</td><td>O</td></tr>
				<tr><td>H</td><td>J</td><td>K</td><td>L</td></tr>
				<tr><td>N</td><td>M</td><td>,</td><td>.</td></tr>
			</table>       
		</td>
    </tr> 
</table>

## screenshots

<p align="center">
  <img src="https://eightlittlebits.github.io/elbbp/screenshots/palette-test.png" />
  <img src="https://eightlittlebits.github.io/elbbp/screenshots/scrolling-logo.png" />
  <img src="https://eightlittlebits.github.io/elbbp/screenshots/sine-scroller.png" />
</p>
<p align="center">
  <img src="https://eightlittlebits.github.io/elbbp/screenshots/sprites.png" />
  <img src="https://eightlittlebits.github.io/elbbp/screenshots/audio-test.png" />
  <img src="https://eightlittlebits.github.io/elbbp/screenshots/nyan.png" />
</p>

## resources

- [https://esolangs.org/wiki/BytePusher](https://esolangs.org/wiki/BytePusher)
- [https://esolangs.org/wiki/ByteByteJump](https://esolangs.org/wiki/ByteByteJump)

## license

MIT License

Copyright (c) 2019 David Parrott

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
