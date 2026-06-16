using System.Collections.Generic;
using System.ServiceModel;

namespace CurrencyExchange.Service
{
    [ServiceContract]
    public interface ICurrencyService
    {
        [OperationContract] string HelloWorld();
        [OperationContract] double GetExchangeRate(string currencyCode);
        [OperationContract] List<CurrencyRate> GetAllRates();
        [OperationContract] string RegisterUser(string username, string password);
        [OperationContract] bool Login(string username, string password);
        [OperationContract] double GetUserBalance(string username, string currency);
        [OperationContract] bool TopUpAccount(string username, double amount);
        [OperationContract] string BuyCurrency(string username, string currencyCode, double amount);
        [OperationContract] string SellCurrency(string username, string currencyCode, double amount);
        [OperationContract] List<Transaction> GetUserTransactions(string username);
        [OperationContract] List<HistoricalRate> GetHistoricalRates(string currencyCode, string startDate, string endDate);
    }

    [System.Runtime.Serialization.DataContract]
    public class CurrencyRate
    {
        [System.Runtime.Serialization.DataMember] public string Code { get; set; }
        [System.Runtime.Serialization.DataMember] public string Name { get; set; }
        [System.Runtime.Serialization.DataMember] public double Mid { get; set; }
    }

    [System.Runtime.Serialization.DataContract]
    public class Transaction
    {
        [System.Runtime.Serialization.DataMember] public int Id { get; set; }
        [System.Runtime.Serialization.DataMember] public string Type { get; set; }
        [System.Runtime.Serialization.DataMember] public string Currency { get; set; }
        [System.Runtime.Serialization.DataMember] public double Amount { get; set; }
        [System.Runtime.Serialization.DataMember] public double Rate { get; set; }
        [System.Runtime.Serialization.DataMember] public double PlnValue { get; set; }
        [System.Runtime.Serialization.DataMember] public string Date { get; set; }
    }

    [System.Runtime.Serialization.DataContract]
    public class HistoricalRate
    {
        [System.Runtime.Serialization.DataMember] public string Date { get; set; }
        [System.Runtime.Serialization.DataMember] public double Rate { get; set; }
    }
}       