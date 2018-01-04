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
            return (reader, buffer, pos) =>
            {
                if (reader.IsDBNull(index)) return pos;
                
                Comma.Write(buffer, ref pos);
                name.Write(buffer, ref pos);

                if (reader.GetBoolean(index))
                {
                    TrueString.CopyTo(buffer.Slice(pos, 4));
                    pos += 4;
                }
                else
                {
                    FalseString.CopyTo(buffer.Slice(pos, 5));
                    pos += 5;
                }

                return pos;
            };
        }
    }
}