using System;
using System.Buffers.Text;
using System.Data.Common;

namespace Beeline.Writers
{
    public static class DoubleWriter
    {
        public static Func<DbDataReader, byte[], int, int> Make(int index, byte[] nameBytes)
        {
            return (reader, buffer, pos) =>
            {
                if (reader.IsDBNull(index)) return pos;
                
                Comma.Write(buffer, ref pos);

                nameBytes.CopyTo(buffer, pos);
                pos += nameBytes.Length;
                var value = reader.GetDouble(index);
                Utf8Formatter.TryFormat(value, new Span<byte>(buffer, pos, 128), out var n);

                return pos + n;
            };
        }
    }
}