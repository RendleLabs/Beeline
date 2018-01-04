using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Beeline.Tests
{
    public class SqlServerTests
    {
        [Fact]
        public async Task SqlDataReader_RowSerializer_Works()
        {
            int written = 0;
            byte[] buffer = new byte[8192];
            using (var cn = new SqlConnection("data source=tcp:localhost;initial catalog=Beeline;integrated security=false;user=sa;password=S3cr3tSqu1rr3l"))
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT Id, Name, ByteValue, ShortValue, LongValue, DecimalValue, FloatValue, DateTimeValue, DateTime2Value, DateTimeOffsetValue, UnicodeName, LongStringValue FROM Test";
                cmd.CommandType = CommandType.Text;
                await cn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var actual = RowSerializer.For(reader);
                    while (await reader.ReadAsync())
                    {
                        written = actual.Write(reader, buffer);
                    }
                }
            }
            Assert.NotEqual(0, written);
            var json = Encoding.UTF8.GetString(buffer, 0, written);
            var jobj = JObject.Parse(json);
            Assert.Equal(1, jobj.Value<int>("Id"));
            Assert.Equal("Bob", jobj.Value<string>("Name"));
            Assert.Equal(255, jobj.Value<byte>("ByteValue"));
            Assert.Equal(12345, jobj.Value<short>("ShortValue"));
            Assert.Equal(123456789, jobj.Value<long>("LongValue"));
            Assert.Equal(12345678m, jobj.Value<decimal>("DecimalValue"));
            Assert.InRange(jobj.Value<double>("FloatValue"), 12345678d, 12345679d); // Serializer doesn't do all the digits
            Assert.Equal(new DateTime(2018, 1, 4, 0, 0, 0), jobj.Value<DateTime>("DateTimeValue"));
            Assert.Equal(new DateTime(2018, 1, 4, 0, 0, 0), jobj.Value<DateTime>("DateTime2Value"));
            Assert.Equal(new DateTimeOffset(2018, 1, 4, 0, 0, 0, TimeSpan.Zero), jobj.Value<DateTime>("DateTimeOffsetValue"));
            Assert.Equal("Bob", jobj.Value<string>("UnicodeName"));
            Assert.Equal("We love Bob", jobj.Value<string>("LongStringValue"));
        }
    }
}
