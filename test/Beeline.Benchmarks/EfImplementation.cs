using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Beeline.Benchmarks
{
    public static class EfImplementation
    {
        private static readonly DbContextOptions<ValueContext> Options =
            new DbContextOptionsBuilder<ValueContext>().UseSqlServer(Database.ConnectionString).Options;

        private static readonly JsonSerializer Serializer = new JsonSerializer
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        
        public static async Task RunList(MemoryStream stream, int from, int to)
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            using (var context = new ValueContext(Options))
            {
                var values = await context.Values.Where(v => v.Id >= from && v.Id <= to).ToListAsync();
                Serializer.Serialize(writer, values);
            }
        }
    }
}