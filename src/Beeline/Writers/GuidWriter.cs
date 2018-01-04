using System;
using System.Buffers;
using System.Buffers.Text;
using System.Data.Common;

namespace Beeline.Writers
{
    public static class GuidWriter
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
                nameBytes[pos++] = QuotationMark;
                Utf8Formatter.TryFormat(reader.GetFieldValue<Guid>(index), new Span<byte>(buffer, pos, 32), out var n, new StandardFormat('O'));
                pos = pos + n;
                nameBytes[pos] = QuotationMark;
                return pos + 1;
            };
        }
    }
}