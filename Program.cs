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
        static void Main1(string[] args)
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

        static void Main(string[] args)
        {
            string bhavFile = "", deliverablesFile = "";
            for(int i = 0; i < args.Length; i++)
            {
                if(args[i] == "-S")
                {
                    bhavFile = args[++i];
                    if(!File.Exists(bhavFile)) {Console.WriteLine("{0} does not exists", bhavFile); Environment.Exit(0);}
                }
                else if(args[i] == "-D")
                {
                    deliverablesFile = args[++i];
                    if(!File.Exists(deliverablesFile)) {Console.WriteLine("{0} does not exists", deliverablesFile); Environment.Exit(0);}
                }
                else 
                {
                    Console.WriteLine("Usage ./run -S stocks_ltp_file -D deliverablesFile");
                    Environment.Exit(0);
                }
            }

            if(bhavFile == "" || deliverablesFile == "")
            {
                Console.WriteLine("Usage ./run -S stocks_ltp_file -D deliverables_file");
                Environment.Exit(0);
            }

            NseStockMarket.loadDailyStockDataToDB(bhavFile, deliverablesFile);
        }
    }
}
