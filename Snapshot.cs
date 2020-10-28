using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DartDeserialize
{
    public class Snapshot
    {
        public static readonly int kMagicSize = 4;
        public static readonly int kLengthOffset = 4;
        public static readonly int kMaxObjectAlignment = 16;

        private byte[] instructions;
        private byte[] metadata;
        private byte[] data; 

        public Snapshot(byte[] data, byte[] instructions)
        { 
            this.instructions = instructions;

            BinaryReader br = new BinaryReader(new MemoryStream(data));
            byte[] magic = br.ReadBytes(4);
            int metaLength = (int)Utils.RoundUp((ulong)(br.ReadInt64() + kMagicSize), kMaxObjectAlignment);

            metadata = data[..metaLength];
            this.data = data[metaLength..];
        }

        public MetadataStreamReader GetSnapshotMetadataStream()
        {
            return new MetadataStreamReader(new MemoryStream(metadata));
        }

        public Stream GetSnapshotDataStream()
        {
            return new MemoryStream(data);
        }

        public Stream GetSnapshotInstructionStream()
        {
            return new MemoryStream(instructions);
        }
    }
}
