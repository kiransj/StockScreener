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

    dotnet ef migrations add InitialCreate
    dotnet ef database update

    Microsoft.EntityFrameworkCore.Design
    Microsoft.EntityFrameworkCore.Sqlite
    <ItemGroup>
        <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="2.0.0" />
    </ItemGroup>

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
            modelBuilder.Entity<CompanyInformation>()
                .HasIndex(x => new { x.symbol, x.isinNumber} );
            modelBuilder.Entity<CompanyInformation>()
                .HasKey(x => new { x.symbol, x.series, x.isinNumber});

            modelBuilder.Entity<DailyStockData>()
                .HasIndex(x => new { x.symbol, x.isinNumber, x.date } );

            modelBuilder.Entity<DailyStockData>()
                .HasKey(x => new { x.isinNumber, x.date, x.series});
        }
    }
    class Program
    {
        static void loadCompaniesInformation()
        {
            var result = NseStockMarket.parseListOfCompaniesFromCSV(@"data/nse.csv");

            using (var db = new DataContext())
            {
                foreach(var item in result)
                {
                    db.companyInformation.Add(item);
                }
                var count = db.SaveChanges();
                Console.WriteLine("{0} companies are saved to database", count);
            }
        }
        static void Main(string[] args)
        {
            loadCompaniesInformation();
            for(var i = 1; i < 31; i++)
            {
                var filename = string.Format("C:\\software\\cygwin64\\home\\ksj\\tmp\\bhav\\cm{0:00}NOV2017bhav.csv", i);
                if(!File.Exists(filename)) continue;
                Console.WriteLine("Loading file {0}", filename);
                var result = NseStockMarket.parseDailyStockDataFromCSV(filename);
                using (var db = new DataContext())
                {
                    foreach(var item in result)
                    {
                        db.stockData.Add(item);
                    }
                    var count = db.SaveChanges();
                    Console.WriteLine("{0} records saved to database", count);
                }
            }
        }
    }
}
