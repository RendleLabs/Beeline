using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Beeline.Benchmarks
{
    [Table("Values")]
    public class ValueModel
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class ValueContext : DbContext
    {
        public ValueContext(DbContextOptions options) : base(options)
        {
        }
        
        public DbSet<ValueModel> Values { get; set; }
    }
}