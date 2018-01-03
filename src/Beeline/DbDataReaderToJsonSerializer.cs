using System;
using System.Data.Common;
using System.Text;
using System.Threading;
using Beeline.Writers;

namespace Beeline
{
    public class DbDataReaderToJsonSerializer
    {
        private static readonly byte[] OpenBrace = Encoding.UTF8.GetBytes("{ ");
        private static readonly byte[] CloseBrace = Encoding.UTF8.GetBytes(" }");
        private readonly Func<DbDataReader, byte[], int, int>[] _writers;

        private DbDataReaderToJsonSerializer(Func<DbDataReader, byte[], int, int>[] writers)
        {
            _writers = writers;
        }

        public int Write(DbDataReader reader, byte[] buffer, CancellationToken ct = default)
        {
            OpenBrace.CopyTo(buffer, 0);
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

            CloseBrace.CopyTo(buffer, pos);

            return pos + 2;
        }

        public static DbDataReaderToJsonSerializer For(DbDataReader reader, bool camelCase)
        {
            var fieldWriters = new FieldWriters(camelCase);
            var writers = new Func<DbDataReader, byte[], int, int>[reader.FieldCount];
            
            for (int i = 0; i < reader.FieldCount; i++)
            {
                writers[i] = fieldWriters.Make(i, reader.GetFieldType(i), reader.GetName(i));
            }

            return new DbDataReaderToJsonSerializer(writers);
        }
    }
}