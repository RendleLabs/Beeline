using System;
using System.Buffers;
using System.Buffers.Text;
using System.Data.Common;

namespace Beeline.Writers
{
    public static class DateTimeOffsetWriter
    {
        private const byte QuotationMark = 34;
        
        public static Func<DbDataReader, byte[], int, int> Make(int index, byte[] nameBytes)
        {
            return (reader, buffer, pos) =>
            {
                if (reader.IsDBNull(index)) return pos;
                
                Comma.Write(buffer, ref pos);

                nameBytes.CopyTo(buffer, pos);
                pos += nameBytes.Length;
                buffer[pos++] = QuotationMark;
                var value = reader.GetFieldValue<DateTimeOffset>(index);
                Utf8Formatter.TryFormat(value, new Span<byte>(buffer, pos, 64), out var n, new StandardFormat('O'));
                pos = pos + n;
                buffer[pos] = QuotationMark;
                return pos + 1;
            };
        }
    }
}