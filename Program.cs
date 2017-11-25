using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TinyCsvParser;
using TinyCsvParser.Mapping;

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
    public class CompanyInformation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string symbol {get; set;}
        [Required]
        public string companyName{get; set;}
        [Required]
        public string series{get; set;}
        [Required]
        public DateTime dateOfListing{get; set;}
        public float paidUpvalue {get; set;}
        public int marketLot {get; set;}
        [Required]
        public string isinNumber{get; set;}
        public float faceValue{get; set;}
    }

    public class stockData
    {
        public string symbol  {get; set;}
        public string series  {get; set;}
        public float open { get; set;}
        public float high { get; set;}
        public float low { get; set;}
        public float close { get; set;}
        public float lastPrice { get; set;}
        public float prevPrice { get; set;}
        public float totalTradedQty { get; set;}
        public float totalTradedValue { get; set;}
        public DateTime date{get; set;}

        public int totalTrades { get; set;}
        public string isinNumber{get; set;}
    }

    public class CsvCompanyMapping : CsvMapping<CompanyInformation>
    {
        public CsvCompanyMapping() : base()
        {
            MapProperty(0, x => x.symbol);
            MapProperty(1, x => x.companyName);
            MapProperty(2, x => x.series);
            MapProperty(3, x => x.dateOfListing);
            MapProperty(4, x => x.paidUpvalue);
            MapProperty(5, x => x.marketLot);
            MapProperty(6, x => x.isinNumber);
            MapProperty(7, x => x.faceValue);
        }
    }

    public class DataContext : DbContext
    {
        public DbSet<CompanyInformation> companyInformation { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=data.db");
            optionsBuilder.EnableSensitiveDataLogging(true);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CsvCompanyMapping csvMapper = new CsvCompanyMapping();
            CsvParser<CompanyInformation> csvParser = new CsvParser<CompanyInformation>(csvParserOptions, csvMapper);
            var result = csvParser
                .ReadFromFile(@"data/nse.csv", Encoding.ASCII)
                .ToList();

#if false
            using (var db = new DataContext())
            {
                foreach(var item in result)
                {
                    db.companyInformation.Add(item.Result);
                }
                var count = db.SaveChanges();
                Console.WriteLine("{0} records saved to database", count);
            }
#endif
            using(var db = new DataContext())
            {

                foreach(var company1 in db.companyInformation.OrderBy(x => x.companyName))
                {
                    Console.WriteLine("{0} --> {1}", company1.companyName, company1.dateOfListing.Day);
                }

                var company = db.companyInformation.Where(x => x.isinNumber == "INE647O01011").ToList();
                Console.WriteLine("Company : {0}", company[0].companyName);
            }
        }
    }
}
