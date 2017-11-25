using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TinyCsvParser;
using TinyCsvParser.Mapping;


using StockMarket;
using System.IO;

/*
    Help: https://docs.microsoft.com/en-us/ef/core/get-started/netcore/new-db-sqlite

    dotnet ef migrations add InitialCreate &&  dotnet ef database update

 */

namespace screener
{
    public class DataContext : DbContext
    {
        public DbSet<CompanyInformation> companyInformation { get; set; }
        public DbSet<DailyStockData> stockData { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=data.db");
            optionsBuilder.EnableSensitiveDataLogging(true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Set the composite primary keys for each table
            modelBuilder.Entity<CompanyInformation>().HasKey(x => new { x.symbol, x.series, x.isinNumber});
            modelBuilder.Entity<DailyStockData>().HasKey(x => new { x.isinNumber, x.date, x.series });

            // Set the index for faster query operations
            modelBuilder.Entity<CompanyInformation>().HasIndex(x => new { x.symbol, x.isinNumber});
            modelBuilder.Entity<DailyStockData>().HasIndex(x => new { x.symbol, x.isinNumber, x.date, x.series});
        }
    }
    class Program
    {
        // Load list of companies traded in NSE
        static void loadCompaniesInformation()
        {
            var result = NseStockMarket.parseListOfCompaniesFromCSV(@"data/nse.csv");

            using (var db = new DataContext())
            {
                foreach (var item in result)
                {
                    db.companyInformation.Add(item);
                }
                var count = db.SaveChanges();
                Console.WriteLine("{0} companies are saved to database", count);
            }
        }

        // Populate the DB with closing price and deliverables for each
        // BhavFile has the closing prices and deliverablesFile has the delivery Qty and Percentage for each stock
        static void loadDailyStockData(string bhavFile, string deliverablesFile)
        {
            var stockData = NseStockMarket.parseDailyStockInformation(bhavFile, deliverablesFile);
            using (var db = new DataContext())
            {
                foreach (var stock in stockData)
                {
                    db.stockData.Add(stock);
                }
                var count = db.SaveChanges();
                Console.WriteLine("{0} records saved to database", count);
            }
        }
        static void Main(string[] args)
        {
            for (var i = 1; i < 31; i++)
            {
                string bhavFile = string.Format("C:\\software\\cygwin64\\home\\ksj\\tmp\\data\\cm{0:00}NOV2017bhav.csv", i);
                string deliverablesFile = string.Format("C:\\software\\cygwin64\\home\\ksj\\tmp\\data\\MTO_{0:00}112017.DAT", i);

                if (File.Exists(bhavFile) == false || File.Exists(deliverablesFile) == false)
                {
                    Console.WriteLine("Bhav File: {0}\nMTO File: {1}\n does not exists.\n\n", bhavFile, deliverablesFile);
                    continue;
                }

                loadDailyStockData(bhavFile, deliverablesFile);
            }
        }
    }
}
