using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Net;
using System.Text;

namespace Beeline.Writers
{
    public class FieldWriters
    {
        private static readonly Func<DbDataReader, byte[], int, int> NoOp = (_, __, pos) => pos;
        
        private static readonly ReadOnlyDictionary<Type, Func<int, byte[], Func<DbDataReader, byte[], int, int>>> Makers =
            new ReadOnlyDictionary<Type, Func<int, byte[], Func<DbDataReader, byte[], int, int>>>(
                new Dictionary<Type, Func<int, byte[], Func<DbDataReader, byte[], int, int>>>
                {
                    { typeof(int), Int32Writer.Make },
                    { typeof(string), StringWriter.Make }
                });
        
        private readonly bool _camelCase;

        public FieldWriters(bool camelCase)
        {
            _camelCase = camelCase;
        }

        public Func<DbDataReader, byte[], int, int> Make(int index, Type columnType, string name)
        {
            if (_camelCase)
            {
                name = char.ToLowerInvariant(name[0]) + name.Substring(1);
            }
            var nameBytes = Encoding.UTF8.GetBytes($"\"{name}\": ");

            return Makers.TryGetValue(columnType, out var maker) ? maker(index, nameBytes) : null;
        }
    }
}