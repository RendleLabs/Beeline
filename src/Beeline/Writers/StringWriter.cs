using System;
using System.Buffers;
using System.Data.Common;
using System.Text;

namespace Beeline.Writers
{
    public static class StringWriter
    {
        private static readonly byte[] EmptyString = Encoding.UTF8.GetBytes("\"\"");
        public static Writer Make(int index, NameWriter name)
        {
            return (reader, buffer, pos) =>
            {
                if (reader.IsDBNull(index)) return pos;
                
                Comma.Write(buffer, ref pos);
                name.Write(buffer, ref pos);

                var chars = ArrayPool<char>.Shared.Rent(1024);
                try
                {
                    int charCount = (int) reader.GetChars(index, 0, chars, 1, 1000);
                    if (charCount == 0)
                    {
                        EmptyString.CopyTo(buffer.Slice(pos, 2));
                        return pos + 2;
                    }
                    
                    chars[0] = '"';
                    chars[charCount + 1] = '"';
                    charCount += 2;
                    var source = new ReadOnlySpan<char>(chars, 0, charCount);
                    var target = buffer.Slice(pos);
                    var byteCount = Encoding.UTF8.GetBytes(source, target);
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