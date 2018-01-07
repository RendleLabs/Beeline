using System;
using Npgsql;
using NpgsqlTypes;

namespace Beeline.AspNetCoreBenchmarks.Data
{
    public static class DatabaseInitializer
    {
	    private const string DropTable = "drop table if exists world";
	    private const string CreateTable = @"create table world
(
	id integer not null
		constraint world_pkey
			primary key,
	randomnumber integer
)";

	    private const string Insert = "INSERT INTO world (id, randomnumber) VALUES (@id, @randomnumber)";

	    public static void Initialize(string connectionString)
	    {
		    var random = new Random(42);
		    using (var connection = new NpgsqlConnection(connectionString))
			using (var cmd = connection.CreateCommand())
		    {
			    connection.Open();
			    cmd.CommandText = DropTable;
			    cmd.ExecuteNonQuery();
			    cmd.CommandText = CreateTable;
			    cmd.ExecuteNonQuery();
			    cmd.CommandText = Insert;
			    var @id = cmd.Parameters.Add("id", NpgsqlDbType.Integer);
			    var @randomnumber = cmd.Parameters.Add("randomnumber", NpgsqlDbType.Integer);
			    cmd.Prepare();
			    for (int i = 1; i < 10001; i++)
			    {
				    @id.Value = i;
				    @randomnumber.Value = random.Next(1, 999_999_999);
				    cmd.ExecuteNonQuery();
			    }
		    }
	    }
    }
}