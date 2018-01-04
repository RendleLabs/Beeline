using System;
using System.Data.Common;
using System.Text;
using Beeline.Writers;

namespace Beeline
{
    public class RowSerializer
    {
        private const byte Space = 32;
        private const byte OpenBrace = 123;
        private const byte CloseBrace = 125;
        private readonly Writer[] _writers;

        private RowSerializer(Writer[] writers)
        {
            _writers = writers;
        }

        public int Write(DbDataReader reader, Span<byte> buffer)
        {
            buffer[0] = OpenBrace;
            buffer[1] = Space;
            int pos = 2;
            for (int i = 0, l = _writers.Length; i < l; i++)
            {
                if (reader.IsDBNull(i) || _writers[i] == null) continue;
                pos = _writers[i](reader, buffer, pos);
            }

            if (pos == 2)
            {
                return 0;
            }

            buffer[pos++] = Space;
            buffer[pos++] = CloseBrace;

            return pos;
        }

        public static RowSerializer For(DbDataReader reader) => For(reader, name => name);

        public static RowSerializer For(DbDataReader reader, bool camelCase) => camelCase
            ? For(reader, name => char.ToLowerInvariant(name[0]) + name.Substring(1))
            : For(reader, name => name);

        public static RowSerializer For(DbDataReader reader, Func<string, string> nameFormatter)
        {
            if (nameFormatter == null) throw new ArgumentNullException(nameof(nameFormatter));
            
            var fieldWriters = new FieldWriters(nameFormatter);
            var writers = new Writer[reader.FieldCount];
            
            for (int i = 0; i < reader.FieldCount; i++)
            {
                writers[i] = fieldWriters.Make(i, reader.GetFieldType(i), reader.GetName(i));
            }

            return new RowSerializer(writers);
        }
    }
}