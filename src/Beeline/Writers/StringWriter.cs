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
            return (reader, buffer) =>
            {
                if (reader.IsDBNull(index)) return buffer;
                
                Comma.Write(ref buffer);
                name.Write(ref buffer);

                var chars = ArrayPool<char>.Shared.Rent(1024);
                try
                {
                    int charCount = (int) reader.GetChars(index, 0, chars, 1, 1000);
                    if (charCount == 0)
                    {
                        EmptyString.CopyTo(buffer);
                        return buffer.Slice(2);
                    }
                    
                    chars[0] = '"';
                    chars[charCount + 1] = '"';
                    charCount += 2;
                    var source = new ReadOnlySpan<char>(chars, 0, charCount);
                    var byteCount = Encoding.UTF8.GetBytes(source, buffer);
                    return buffer.Slice(byteCount);
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(chars);
                }
            };
        }
    }
}