using System;
using System.Buffers.Text;
using System.Data.Common;

namespace Beeline.Writers
{
    public static class Int32Writer
    {
        public static Writer Make(int index, NameWriter name)
        {
            return (reader, buffer, pos) =>
            {
                if (reader.IsDBNull(index)) return pos;
                
                Comma.Write(buffer, ref pos);
                name.Write(buffer, ref pos);

                Utf8Formatter.TryFormat(reader.GetInt32(index), buffer.Slice(pos, 16), out var n);

                return pos + n;
            };
        }
    }
}