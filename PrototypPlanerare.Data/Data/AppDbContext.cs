using Microsoft.EntityFrameworkCore;
using PrototypPlanerare.Models;
using System;
using System.IO;

namespace PrototypPlanerare.Data
{
    public class AppDbContext : DbContext
    {
        // Tables
        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<ItemComment> ItemComments { get; set; } = null!;
        public DbSet<EngineeringTask> EngineeringTasks { get; set; } = null!;

        // Resolved database path (what EF will use)
        public string DbPath { get; }

        // File name you want
        private const string DbFileName = "eco.db";

        /// <summary>
        /// Primary, easy-to-find data folder (outside MSIX container):
        /// %USERPROFILE%\AppData\Local\PrototypPlanerare
        /// </summary>
        public static string PreferredDataFolder
        {
            get
            {
                // Optional dev override (absolute file path to DB)
                var overridePath = Environment.GetEnvironmentVariable("ECO_DB_PATH");
                if (!string.IsNullOrWhiteSpace(overridePath))
                {
                    var dir = Path.GetDirectoryName(overridePath)!;
                    Directory.CreateDirectory(dir);
                    return dir;
                }

                // Build non-container Local path by hand (reliable even when packaged)
                var userProfile = Environment.GetEnvironmentVariable("USERPROFILE") ?? "";
                var folder = Path.Combine(userProfile, "AppData", "Local", "PrototypPlanerare");
                try
                {
                    Directory.CreateDirectory(folder);
                    return folder;
                }
                catch
                {
                    // Fall back to SpecialFolder (may be containerized in MSIX)
                    var fallback = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "PrototypPlanerare");
                    Directory.CreateDirectory(fallback);
                    return fallback;
                }
            }
        }

        /// <summary>
        /// Full default DB path (folder + file name).
        /// </summary>
        public static string DefaultDbPath => Path.Combine(PreferredDataFolder, DbFileName);

        // Kept for compatibility with any older call sites
        public static string GetDefaultDbPath() => DefaultDbPath;

        // Ctors
        public AppDbContext()
        {
            DbPath = DefaultDbPath;
            System.Diagnostics.Debug.WriteLine($"[EF] DbPath (ctor): {DbPath}");
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            DbPath = DefaultDbPath;
            System.Diagnostics.Debug.WriteLine($"[EF] DbPath (options-ctor): {DbPath}");
        }

        // Optional path-based ctor for custom locations
        public AppDbContext(string dbPath)
        {
            DbPath = string.IsNullOrWhiteSpace(dbPath) ? DefaultDbPath : dbPath;
            var dir = Path.GetDirectoryName(DbPath);
            if (!string.IsNullOrWhiteSpace(dir)) Directory.CreateDirectory(dir);
            System.Diagnostics.Debug.WriteLine($"[EF] DbPath (string-ctor): {DbPath}");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source={DbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
