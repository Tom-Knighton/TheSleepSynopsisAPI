using System;
using Microsoft.EntityFrameworkCore;

namespace TheSleepSynopsisAPI.Data
{
    public class DataContext: DbContext
    {
        protected readonly IConfiguration _config;

        public DataContext(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            var connectString = _config.GetConnectionString("Database");
            builder.UseMySql(connectString, ServerVersion.AutoDetect(connectString));
        }
    }
}

