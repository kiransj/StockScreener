using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace StockMarket
{
    public class CompanyInformation
    {
        [Required]
        public string symbol { get; set; }
        [Required]
        public string companyName { get; set; }
        [Required]
        public string series { get; set; }
        public DateTime dateOfListing { get; set; }
        public float paidUpvalue { get; set; }
        public int marketLot { get; set; }
        [Required]
        public string isinNumber { get; set; }
        public float faceValue { get; set; }
        public bool star { get; set; }
        public string notes { get; set; }
        public string industry { get; set; }
    }

    public class CircuitBreaker
    {
        [Required]
        public string symbol;
        [Required]
        public string series;
        [Required]
        public char high_low;
        [Required]
        public DateTime date;
    }

    public class StockPortfolio
    {
        [Required]
        public string symbol { get; set; }
        [Required]
        public string series { get; set; }
        [Required]
        public string isinNumber { get; set; }
        [Required]
        public DateTime buyDate { get; set; }
        [Required]
        public float buyPrice { get; set; }
        public string notes { get; set; }
    }

    public class DailyStockData
    {
        [Required]
        public string symbol { get; set; }
        [Required]
        public string series { get; set; }
        [Required]
        public float open { get; set; }
        [Required]
        public float high { get; set; }
        [Required]
        public float low { get; set; }
        [Required]
        public float close { get; set; }
        [Required]
        public float lastPrice { get; set; }
        [Required]
        public float prevClose { get; set; }
        [Required]
        public long totalTradedQty { get; set; }
        [Required]
        public float totalTradedValue { get; set; }
        [Required]
        public DateTime date { get; set; }
        [Required]
        public long totalTrades { get; set; }
        [Required]
        public string isinNumber { get; set; }
        [Required]
        public long deliverableQty { get; set; }
        [Required]
        public float deliveryPercentage { get; set; }
    }
    public class StockDataContext : DbContext
    {
        public DbSet<CompanyInformation> companyInformation { get; set; }
        public DbSet<DailyStockData> stockData { get; set; }
        public DbSet<StockPortfolio> portfolio { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=data.db");
            optionsBuilder.EnableSensitiveDataLogging(true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Set the composite primary keys for each table
            modelBuilder.Entity<CompanyInformation>().HasKey(x => new { x.symbol, x.series, x.isinNumber });
            modelBuilder.Entity<DailyStockData>().HasKey(x => new { x.isinNumber, x.date, x.series });
            modelBuilder.Entity<StockPortfolio>().HasKey(x => new {x.isinNumber, x.series});

            // Set the index for faster query operations
            modelBuilder.Entity<CompanyInformation>().HasIndex(x => new { x.symbol, x.isinNumber });
            modelBuilder.Entity<DailyStockData>().HasIndex(x => new { x.symbol, x.isinNumber, x.date, x.series });
            modelBuilder.Entity<StockPortfolio>().HasIndex(x => x.isinNumber);


        }
    }
}