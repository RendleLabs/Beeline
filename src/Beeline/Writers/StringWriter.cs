using System;
using System.Buffers;
using System.Data.Common;
using System.Text;

namespace Beeline.Writers
{
    public static class StringWriter
    {
        private static readonly byte[] EmptyString = Encoding.UTF8.GetBytes("\"\"");
        public static Func<DbDataReader, byte[], int, int> Make(int index, byte[] nameBytes)
        {
            return (reader, buffer, pos) =>
            {
                if (reader.IsDBNull(index)) return pos;
                
                Comma.Write(buffer, ref pos);

                nameBytes.CopyTo(buffer, pos);
                pos += nameBytes.Length;

                var chars = ArrayPool<char>.Shared.Rent(1024);
                try
                {
                    int charCount = (int) reader.GetChars(index, 0, chars, 1, 1000);
                    if (charCount == 0)
                    {
                        EmptyString.CopyTo(buffer, pos);
                        return pos + 2;
                    }
                    
                    chars[0] = '"';
                    chars[charCount + 1] = '"';
                    charCount += 2;
                    var byteCount = Encoding.UTF8.GetBytes(chars, 0, charCount, buffer, pos);
                    return pos + byteCount;
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(chars);
                }
            };
        }
    }
}