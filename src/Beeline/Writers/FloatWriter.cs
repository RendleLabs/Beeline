using System;
using System.Buffers.Text;
using System.Data.Common;

namespace Beeline.Writers
{
    public static class FloatWriter
    {
        public static Writer Make(int index, NameWriter name)
        {
            return (reader, buffer, pos) =>
            {
                if (reader.IsDBNull(index)) return pos;
                
                Comma.Write(buffer, ref pos);
                name.Write(buffer, ref pos);

                var value = reader.GetFloat(index);
                Utf8Formatter.TryFormat(value, buffer.Slice(pos, 64), out var n);

                return pos + n;
            };
        }
    }
}