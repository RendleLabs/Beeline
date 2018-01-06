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

        private static readonly byte[] OpenSpace = new byte[] { OpenBrace, Space };
        private static readonly byte[] SpaceClose = new byte[] { Space, CloseBrace };
        private readonly Writer[] _writers;

        private RowSerializer(Writer[] writers)
        {
            _writers = writers;
        }

        public int Write(DbDataReader reader, Span<byte> buffer)
        {
            var currentBufferSize = buffer.Length;
            OpenSpace.AsReadOnlySpan().CopyTo(buffer);

            buffer = buffer.Slice(2);
            for (int i = 0, l = _writers.Length; i < l; i++)
            {
                if (reader.IsDBNull(i) || _writers[i] == null) continue;
                buffer = _writers[i](reader, buffer);
            }

            if (currentBufferSize - buffer.Length  == 2)
            {
                return 0;
            }

            SpaceClose.AsSpan().CopyTo(buffer);
            
            return (currentBufferSize - buffer.Length ) + 2;
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