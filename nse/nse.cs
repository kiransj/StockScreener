using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using TinyCsvParser;
using TinyCsvParser.Mapping;

/*
    Link for list of companies:
 */

namespace StockMarket
{
    public class CompanyInformation
    {
        [Required]
        public string symbol {get; set;}
        [Required]
        public string companyName{get; set;}
        [Required]
        public string series{get; set;}
        public DateTime dateOfListing{get; set;}
        public float paidUpvalue {get; set;}
        public int marketLot {get; set;}
        [Required]
        public string isinNumber{get; set;}
        public float faceValue{get; set;}
    }

    class CsvCompanyInformationMapping : CsvMapping<CompanyInformation>
    {
        public CsvCompanyInformationMapping() : base()
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

    public class DailyStockData
    {
        [Required]
        public string symbol  {get; set;}
        [Required]
        public string series  {get; set;}
        [Required]
        public float open { get; set;}
        [Required]
        public float high { get; set;}
        [Required]
        public float low { get; set;}
        [Required]
        public float close { get; set;}
        [Required]
        public float lastPrice { get; set;}
        [Required]
        public float prevClose { get; set;}
        [Required]
        public float totalTradedQty { get; set;}
        [Required]
        public float totalTradedValue { get; set;}
        [Required]
        public DateTime date{get; set;}
        [Required]
        public int totalTrades { get; set;}
        [Required]
        public string isinNumber{get; set;}
    }

    class CsvDailyStockDataMapping : CsvMapping<DailyStockData>
    {
        public CsvDailyStockDataMapping() : base()
        {
            MapProperty(0, x => x.symbol);
            MapProperty(1, x => x.series);
            MapProperty(2, x => x.open);
            MapProperty(3, x => x.high);
            MapProperty(4, x => x.low);
            MapProperty(5, x => x.close);
            MapProperty(6, x => x.lastPrice);
            MapProperty(7, x => x.prevClose);
            MapProperty(8, x => x.totalTradedQty);
            MapProperty(9, x => x.totalTradedValue);
            MapProperty(10, x => x.date);
            MapProperty(11, x => x.totalTrades);
            MapProperty(12, x => x.isinNumber);
        }
    }

    class NseStockMarket
    {
        static public List<CompanyInformation> parseListOfCompaniesFromCSV(string csvFile)
        {
            string filename = csvFile;

            // If the CSV file is empty then load the file from the nse website
            if(csvFile == null)
            {
                filename = Path.GetTempFileName();
                WebClient client = new WebClient();
                client.Headers["User-Agent"] = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.15) Gecko/20110303 Firefox/3.6.15";
                client.DownloadFile(Contstants.nse_listOfCompanies, filename);
            }

            // Set CSV file parsing options
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CsvCompanyInformationMapping csvMapper = new CsvCompanyInformationMapping();
            CsvParser<CompanyInformation> csvParser = new CsvParser<CompanyInformation>(csvParserOptions, csvMapper);

            // Parse the CSV file
            var result = csvParser
                .ReadFromFile(filename, Encoding.ASCII)
                .Select(x => x.Result)
                .ToList();

            return result;
        }

        static public List<DailyStockData> parseDailyStockDataFromCSV(string csvFile)
        {
            string filename = csvFile;
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CsvDailyStockDataMapping csvMapper = new CsvDailyStockDataMapping();
            CsvParser<DailyStockData> csvParser = new CsvParser<DailyStockData>(csvParserOptions, csvMapper);

            var result = csvParser
                .ReadFromFile(filename, Encoding.ASCII)
                .Select(x => x.Result)
                .ToList();

            return result;
        }
    }
}
