using Microsoft.Data.SqlClient;

namespace Multikino.Data
{
    public static class DatabaseInitializer
    {
        public static void Seed(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var connectionString = config.GetConnectionString("DefaultConnection");

            var sql = File.ReadAllText("SeedData.sql");

            using var connection = new SqlConnection(connectionString);
            connection.Open();

            using var command = new SqlCommand(sql, connection);
            command.ExecuteNonQuery();
        }
    }
}
