using System;
using Microsoft.EntityFrameworkCore;
using TheSleepSynopsisAPI.Domain.Models;

namespace TheSleepSynopsisAPI.Data
{
    public class DataContext : DbContext
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

        #region Tables
        public DbSet<User> Users { get; set; }
        public DbSet<UserAuth> UserAuth { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<SleepEntry> SleepEntries { get; set; }
        public DbSet<Dream> Dreams { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        #endregion

        #region Model Creating
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(user =>
            {
                user.HasKey(u => u.UserUUID);
                user.Ignore(u => u.AuthTokens);

                user
                    .HasMany(u => u.SleepEntries)
                    .WithOne(s => s.User)
                    .HasForeignKey(s => s.UserUUID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                user
                    .HasMany(u => u.Following)
                    .WithOne(f => f.Initiator)
                    .HasForeignKey(f => f.InitiatorUUID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
                user
                    .HasMany(u => u.Followers)
                    .WithOne(f => f.SecondUser)
                    .HasForeignKey(f => f.SecondUUID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                user
                    .HasMany(u => u.RefreshTokens)
                    .WithOne(r => r.User)
                    .HasForeignKey(r => r.UserUUID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
                user
                    .HasOne(u => u.UserAuth)
                    .WithOne(ua => ua.User)
                    .HasForeignKey<UserAuth>(ua => ua.UserUUID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                user
                    .Property(u => u.UserRole)
                    .HasConversion(
                        v => v.ToString(),
                        v => (UserRole)Enum.Parse(typeof(UserRole), v));
            });

            builder.Entity<UserAuth>(userAuth =>
            {
                userAuth.HasKey(u => u.UserUUID);
            });

            builder.Entity<UserRefreshToken>(token =>
            {
                token.HasKey(t => new { t.UserUUID, t.RefreshToken });
            });

            builder.Entity<Friendship>(friendship =>
            {
                friendship.HasKey(f => new { f.InitiatorUUID, f.SecondUUID });
            });

            builder.Entity<SleepEntry>(sleepEntry =>
            {
                sleepEntry.HasKey(s => s.SleepUUID);

                sleepEntry
                    .HasMany(s => s.Dreams)
                    .WithOne(d => d.SleepEntry)
                    .HasForeignKey(d => d.SleepEntryUUID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Dream>(dream =>
            {
                dream.HasKey(d => d.DreamUUID);
            });
        }

        #endregion
    }
}

