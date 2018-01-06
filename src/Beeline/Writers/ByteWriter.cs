using System;
using System.Buffers;
using System.Buffers.Text;
using System.Data.Common;

namespace Beeline.Writers
{
    public static class ByteWriter
    {
        public static Writer Make(int index, NameWriter name)
        {
            return (reader, buffer) =>
            {
                if (reader.IsDBNull(index)) return buffer;
                
                Comma.Write(ref buffer);
                name.Write(ref buffer);
                
                Utf8Formatter.TryFormat(reader.GetByte(index), buffer, out var n);
                return buffer.Slice(n);
            };
        }
    }
}