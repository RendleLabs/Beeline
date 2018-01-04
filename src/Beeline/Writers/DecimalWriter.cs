using System;
using System.Buffers.Text;
using System.Data.Common;

namespace Beeline.Writers
{
    public static class DecimalWriter
    {
        public static Func<DbDataReader, byte[], int, int> Make(int index, byte[] nameBytes)
        {
            return (reader, buffer, pos) =>
            {
                if (reader.IsDBNull(index)) return pos;
                
                Comma.Write(buffer, ref pos);

                nameBytes.CopyTo(buffer, pos);
                pos += nameBytes.Length;
                Utf8Formatter.TryFormat(reader.GetDecimal(index), new Span<byte>(buffer, pos, 32), out var n);

                return pos + n;
            };
        }
    }
}