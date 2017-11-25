using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;

using TinyCsvParser;
using TinyCsvParser.Mapping;

/*
    Link for list of companies:
 */

namespace StockMarket
{

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


    class DailyStockDeliveryPosition
    {
        public int record { get; set; }
        public int serialNumber { get; set; }
        public string symbol { get; set; }
        public string series { get; set; }
        public long qtyTraded { get; set; }
        public long deliverableQty { get; set; }
        public float deliveryPercentage { get; set; }
    }

    class CsvDailyStockDeviveryPositionMapping : CsvMapping<DailyStockDeliveryPosition>
    {
        public CsvDailyStockDeviveryPositionMapping() : base()
        {
            MapProperty(0, x => x.record);
            MapProperty(1, x => x.serialNumber);
            MapProperty(2, x => x.symbol);
            MapProperty(3, x => x.series);
            MapProperty(4, x => x.qtyTraded);
            MapProperty(5, x => x.deliverableQty);
            MapProperty(6, x => x.deliveryPercentage);
        }
    }

    class NseStockMarket
    {
        static public List<CompanyInformation> parseListOfCompaniesFromCSV(string csvFile)
        {
            string filename = csvFile;

            // If the CSV file is empty then load the file from the nse website
            if (csvFile == null)
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

        // Parse the closing price for all the stock for a given day
        static public List<DailyStockData> parseBhavFile(string csvFile)
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

        // parse the deliverables volumes and percentables for a given day
        static public List<DailyStockDeliveryPosition> parseMTOFile(string csvFile)
        {
            string filename = csvFile;
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CsvDailyStockDeviveryPositionMapping csvMapper = new CsvDailyStockDeviveryPositionMapping();
            CsvParser<DailyStockDeliveryPosition> csvParser = new CsvParser<DailyStockDeliveryPosition>(csvParserOptions, csvMapper);

            var result = csvParser
                .ReadFromFile(filename, Encoding.ASCII)
                .Select(x => x.Result)
                .ToList();

            return result;
        }

        static public List<DailyStockData> parseDailyStockInformation(string bhavFile, string mtoFile)
        {
            var stockPrices = NseStockMarket.parseBhavFile(bhavFile);
            var deliverablesQty = NseStockMarket.parseMTOFile(mtoFile);

            foreach (var stock in stockPrices)
            {
                if (stock.series == "BE")
                {
                    // In BE series all the trades will be delivered
                    stock.deliverableQty = stock.totalTradedQty;
                    stock.deliveryPercentage = 100;
                }
                else
                {
                    var results = deliverablesQty.Where(x => x.series == stock.series && x.symbol == stock.symbol && x.qtyTraded == stock.totalTradedQty).ToList();

                    if (results.Count >= 1)
                    {
                        var res = results.First();
                        stock.deliverableQty = res.deliverableQty;
                        stock.deliveryPercentage = res.deliveryPercentage;
                    }
                }
            }

            return stockPrices;
        }

        public static void loadCompaniesInformationToDB()
        {
            var result = NseStockMarket.parseListOfCompaniesFromCSV(@"data/nse.csv");

            using (var db = new StockDataContext())
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
        public static void loadDailyStockDataToDB(string bhavFile, string deliverablesFile)
        {
            var stockData = NseStockMarket.parseDailyStockInformation(bhavFile, deliverablesFile);
            using (var db = new StockDataContext())
            {
                foreach (var stock in stockData)
                {
                    db.stockData.Add(stock);
                }
                var count = db.SaveChanges();
                Console.WriteLine("{0} records saved to database", count);
            }
        }
    }
}
