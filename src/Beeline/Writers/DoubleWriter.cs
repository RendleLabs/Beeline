using System;
using System.Buffers.Text;
using System.Data.Common;

namespace Beeline.Writers
{
    public static class DoubleWriter
    {
        public static Writer Make(int index, NameWriter name)
        {
            return (reader, buffer, pos) =>
            {
                if (reader.IsDBNull(index)) return pos;
                
                Comma.Write(buffer, ref pos);
                name.Write(buffer, ref pos);

                var value = reader.GetDouble(index);
                Utf8Formatter.TryFormat(value, buffer.Slice(pos, 128), out var n);

                return pos + n;
            };
        }
    }
}