using System;
using System.Buffers;
using System.Buffers.Text;
using System.Data.Common;

namespace Beeline.Writers
{
    public static class DateTimeOffsetWriter
    {
        private const byte QuotationMark = 34;
        
        public static Writer Make(int index, NameWriter name)
        {
            return (reader, buffer, pos) =>
            {
                if (reader.IsDBNull(index)) return pos;
                
                Comma.Write(buffer, ref pos);
                name.Write(buffer, ref pos);

                buffer[pos++] = QuotationMark;
                var value = reader.GetFieldValue<DateTimeOffset>(index);
                Utf8Formatter.TryFormat(value, buffer.Slice(pos, 64), out var n, new StandardFormat('O'));
                pos = pos + n;
                buffer[pos] = QuotationMark;
                return pos + 1;
            };
        }
    }
}