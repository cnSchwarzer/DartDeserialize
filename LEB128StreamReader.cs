using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartDeserialize
{ 
    public class LEB128StreamReader
    {
        public const byte kDataBitsPerByte = 7;
        public const byte kByteMask = (1 << kDataBitsPerByte) - 1;
        public const byte kMaxUnsignedDataPerByte = kByteMask;
        public const sbyte kMinDataPerByte = -(1 << (kDataBitsPerByte - 1));
        public const sbyte kMaxDataPerByte = (~kMinDataPerByte & kByteMask); 
        public const byte kEndByteMarker = (255 - kMaxDataPerByte);
        public const byte kEndUnsignedByteMarker = (255 - kMaxUnsignedDataPerByte);

        public Stream BaseStream { get; private set; }
         
        public LEB128StreamReader(Stream stream)
        {
            BaseStream = stream;
        }

        public void Skip(int bytes)
        {
            BaseStream.Position += 20;
        }
        public byte[] ReadBytes(int bytes)
        {
            byte[] ret = new byte[bytes];
            BaseStream.Read(ret, 0, bytes);
            return ret;
        }
        public byte Read8()
        {
            return (byte)BaseStream.ReadByte();
        }
        public ushort Read16()
        {
            return (ushort)ReadUnroll(kEndByteMarker, 7, 14);
        }
        public uint Read32()
        {
            return (uint)ReadUnroll(kEndByteMarker, 7, 14, 21, 28);
        }
        public ulong Read64()
        {
            return ReadUnroll(kEndByteMarker, 7, 14, 21, 28, 35, 42, 49, 56, 63);
        }
        public ulong ReadUnsigned()
        {
            return Read(kEndUnsignedByteMarker);
        }

        private ulong Read(byte end_byte_marker)
        {
            ulong b = Read8();
            if (b > kMaxUnsignedDataPerByte)
                return b - end_byte_marker;
            ulong r = 0;
            byte s = 0;
            do
            {
                r |= b << s;
                s += kDataBitsPerByte;
                b = Read8();
            }
            while (b <= kMaxUnsignedDataPerByte);
            return r | ((b - end_byte_marker) << s);
        }
        private ulong ReadUnroll(byte end_byte_marker, params int[] shifts)
        {
            ulong b = Read8();
            if (b > kMaxUnsignedDataPerByte)
            {
                return b - end_byte_marker;
            }
            ulong r = b;
            foreach (int shift in shifts.Take(shifts.Length - 1))
            {
                b = Read8();
                if (b > kMaxUnsignedDataPerByte)
                {
                    return r | ((b - end_byte_marker) << shift);
                }
                r |= b << shift;
            }
            int last_shift = shifts.Last();
            b = Read8();
            return r | ((b - end_byte_marker) << last_shift);
        }
    }
}
