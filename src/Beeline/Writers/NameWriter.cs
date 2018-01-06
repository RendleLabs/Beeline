using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Beeline.Writers
{
    public class NameWriter
    {
        private readonly byte[] _bytes;
        private readonly int _length;

        public NameWriter(string name)
        {
            _bytes = Encoding.UTF8.GetBytes($"\"{name}\": ");
            _length = _bytes.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ref Span<byte> buffer)
        {
            _bytes.CopyTo(buffer);
            buffer = buffer.Slice(_length);
        }
    }
}