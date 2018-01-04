using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Text;

namespace Beeline.Writers
{
    public class FieldWriters
    {
        private static readonly ReadOnlyDictionary<Type, Func<int, byte[], Func<DbDataReader, byte[], int, int>>> Makers =
            new ReadOnlyDictionary<Type, Func<int, byte[], Func<DbDataReader, byte[], int, int>>>(
                new Dictionary<Type, Func<int, byte[], Func<DbDataReader, byte[], int, int>>>
                {
                    { typeof(bool), BooleanWriter.Make },
                    { typeof(byte), ByteWriter.Make },
                    { typeof(short), Int16Writer.Make },
                    { typeof(int), Int32Writer.Make },
                    { typeof(long), Int64Writer.Make },
                    { typeof(float), FloatWriter.Make },
                    { typeof(double), DoubleWriter.Make },
                    { typeof(decimal), DecimalWriter.Make },
                    { typeof(string), StringWriter.Make },
                    { typeof(DateTime), DateTimeWriter.Make },
                    { typeof(DateTimeOffset), DateTimeOffsetWriter.Make },
                    { typeof(TimeSpan), TimeSpanWriter.Make },
                    { typeof(Guid), GuidWriter.Make },
                });
        
        private readonly Func<string, string> _nameFormatter;

        public FieldWriters() : this(name => name)
        {
            
        }

        public FieldWriters(bool camelCase)
        {
            if (camelCase)
            {
                this._nameFormatter = name => char.ToLowerInvariant(name[0]) + name.Substring(1);
            }
            else
            {
                this._nameFormatter = name => name;
            }
        }

        public FieldWriters(Func<string, string> nameFormatter)
        {
            _nameFormatter = nameFormatter ?? throw new ArgumentNullException(nameof(nameFormatter));
        }

        public Func<DbDataReader, byte[], int, int> Make(int index, Type columnType, string name)
        {
            name = _nameFormatter(name);
            var nameBytes = Encoding.UTF8.GetBytes($"\"{name}\": ");

            return Makers.TryGetValue(columnType, out var maker) ? maker(index, nameBytes) : null;
        }
    }
}