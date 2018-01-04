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
        public static Func<DbDataReader, byte[], int, int> Make(int index, byte[] nameBytes)
        {
            return (reader, buffer, pos) =>
            {
                if (reader.IsDBNull(index)) return pos;
                
                Comma.Write(buffer, ref pos);

                nameBytes.CopyTo(buffer, pos);
                pos += nameBytes.Length;

                if (reader.GetBoolean(index))
                {
                    TrueString.CopyTo(buffer, pos);
                    pos += 4;
                }
                else
                {
                    FalseString.CopyTo(buffer, pos);
                    pos += 5;
                }

                return pos;
            };
        }
    }
}