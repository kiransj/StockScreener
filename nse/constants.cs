namespace StockMarket
{
    class Contstants
    {
        const string listOfCompanies = @"https://www.nseindia.com/content/equities/EQUITY_L.csv";
        public static string nse_listOfCompanies {
            get {
                return listOfCompanies;
            }
        }
    }
}