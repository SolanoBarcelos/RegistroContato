using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Core.Base.Utils.Env;
using Microsoft.AspNetCore.Builder;

namespace Core.Configuration.Database
{
    public static class DatabaseConfiguration
    {
        public static void ConfigureDatabase(WebApplicationBuilder builder)
        {
            var dbConnectionString = EnvUtils.GetEnv("DB_CONNECTION_STRING");

            builder.Services.AddScoped<IDbConnection>(sp => new NpgsqlConnection(dbConnectionString));
        }
    }
}
