using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Text;

namespace Beeline.Writers
{
    public delegate Span<byte> Writer(DbDataReader reader, Span<byte> buffer);
    
    public class FieldWriters
    {
        private static readonly ReadOnlyDictionary<Type, Func<int, NameWriter, Writer>> Makers =
            new ReadOnlyDictionary<Type, Func<int, NameWriter, Writer>>(
                new Dictionary<Type, Func<int, NameWriter, Writer>>
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

        public Writer Make(int index, Type columnType, string name)
        {
            name = _nameFormatter(name);

            return Makers.TryGetValue(columnType, out var maker) ? maker(index, new NameWriter(name)) : null;
        }
    }
}