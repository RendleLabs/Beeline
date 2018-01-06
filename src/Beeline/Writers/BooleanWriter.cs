using System;
using System.Buffers;
using System.Data.Common;
using System.Text;

namespace Beeline.Writers
{
    public static class BooleanWriter
    {
        private static readonly byte[] TrueString = Encoding.UTF8.GetBytes("true");
        private static readonly byte[] FalseString = Encoding.UTF8.GetBytes("false");
        public static Writer Make(int index, NameWriter name)
        {
            return (reader, buffer) =>
            {
                if (reader.IsDBNull(index)) return buffer;
                
                Comma.Write(ref buffer);
                name.Write(ref buffer);

                if (reader.GetBoolean(index))
                {
                    TrueString.CopyTo(buffer);
                    buffer = buffer.Slice(4);
                }
                else
                {
                    FalseString.CopyTo(buffer);
                    buffer = buffer.Slice(5);
                }

                return buffer;
            };
        }
    }
}