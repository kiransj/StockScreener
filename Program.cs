using System;
using System.IO;

using StockMarket;
/*
    Help: https://docs.microsoft.com/en-us/ef/core/get-started/netcore/new-db-sqlite

    dotnet ef migrations add InitialCreate &&  dotnet ef database update

 */
namespace screener
{
    class Program
    {
        // Load list of companies traded in NSE
        static void Main(string[] args)
        {
            //NseStockMarket.loadCompaniesInformationToDB();
            for (var i = 1; i < 31; i++)
            {
                string bhavFile = string.Format("C:\\software\\cygwin64\\home\\ksj\\tmp\\data\\cm{0:00}NOV2017bhav.csv", i);
                string deliverablesFile = string.Format("C:\\software\\cygwin64\\home\\ksj\\tmp\\data\\MTO_{0:00}112017.DAT", i);

                if (File.Exists(bhavFile) == false || File.Exists(deliverablesFile) == false)
                {
                    Console.WriteLine("Bhav File: {0}\nMTO File: {1}\n does not exists.\n\n", bhavFile, deliverablesFile);
                    continue;
                }

                NseStockMarket.loadDailyStockDataToDB(bhavFile, deliverablesFile);
            }
        }
    }
}
