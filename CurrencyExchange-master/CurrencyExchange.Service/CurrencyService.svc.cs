using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;

namespace CurrencyExchange.Service
{
    public class CurrencyService : ICurrencyService
    {
        private static Dictionary<string, string> _users = new Dictionary<string, string>();
        private static Dictionary<string, Dictionary<string, double>> _balances = new Dictionary<string, Dictionary<string, double>>();
        private static List<Transaction> _transactions = new List<Transaction>();
        private static int _txCounter = 1;

        public string HelloWorld()
        {
            return "Currency Exchange Service is running!";
        }

        public double GetExchangeRate(string currencyCode)
        {
            try
            {
                string url = "http://api.nbp.pl/api/exchangerates/rates/a/" + currencyCode + "/?format=json";
                using (var client = new WebClient())
                {
                    string json = client.DownloadString(url);
                    var obj = JObject.Parse(json);
                    return obj["rates"][0]["mid"].Value<double>();
                }
            }
            catch
            {
                return -1;
            }
        }

        public List<CurrencyRate> GetAllRates()
        {
            var result = new List<CurrencyRate>();
            try
            {
                string url = "http://api.nbp.pl/api/exchangerates/tables/a/?format=json";
                using (var client = new WebClient())
                {
                    string json = client.DownloadString(url);
                    var arr = JArray.Parse(json);
                    var rates = arr[0]["rates"] as JArray;
                    foreach (var r in rates)
                    {
                        result.Add(new CurrencyRate
                        {
                            Code = r["code"].ToString(),
                            Name = r["currency"].ToString(),
                            Mid = r["mid"].Value<double>()
                        });
                    }
                }
            }
            catch { }
            return result;
        }

        public string RegisterUser(string username, string password)
        {
            if (_users.ContainsKey(username))
                return "ERROR: Username already exists";
            _users[username] = password;
            _balances[username] = new Dictionary<string, double>();
            _balances[username]["PLN"] = 0;
            return "OK";
        }

        public bool Login(string username, string password)
        {
            if (!_users.ContainsKey(username))
                return false;
            return _users[username] == password;
        }

        public double GetUserBalance(string username, string currency)
        {
            if (!_balances.ContainsKey(username))
                return -1;
            var bal = _balances[username];
            if (bal.ContainsKey(currency))
                return bal[currency];
            return 0;
        }

        public bool TopUpAccount(string username, double amount)
        {
            if (!_balances.ContainsKey(username) || amount <= 0)
                return false;
            _balances[username]["PLN"] = _balances[username]["PLN"] + amount;
            return true;
        }

        public string BuyCurrency(string username, string currencyCode, double amount)
        {
            if (!_balances.ContainsKey(username))
                return "ERROR: User not found";
            double rate = GetExchangeRate(currencyCode);
            if (rate < 0)
                return "ERROR: Could not get exchange rate";
            double cost = amount * rate;
            if (_balances[username]["PLN"] < cost)
                return "ERROR: Insufficient PLN balance";
            _balances[username]["PLN"] = _balances[username]["PLN"] - cost;
            if (!_balances[username].ContainsKey(currencyCode))
                _balances[username][currencyCode] = 0;
            _balances[username][currencyCode] = _balances[username][currencyCode] + amount;
            _transactions.Add(new Transaction
            {
                Id = _txCounter,
                Type = "BUY",
                Currency = currencyCode,
                Amount = amount,
                Rate = rate,
                PlnValue = cost,
                Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            });
            _txCounter = _txCounter + 1;
            return "OK: Bought " + amount + " " + currencyCode + " for " + cost.ToString("F2") + " PLN at rate " + rate.ToString("F4");
        }

        public string SellCurrency(string username, string currencyCode, double amount)
        {
            if (!_balances.ContainsKey(username))
                return "ERROR: User not found";
            if (!_balances[username].ContainsKey(currencyCode) || _balances[username][currencyCode] < amount)
                return "ERROR: Insufficient currency balance";
            double rate = GetExchangeRate(currencyCode);
            if (rate < 0)
                return "ERROR: Could not get exchange rate";
            double earned = amount * rate;
            _balances[username][currencyCode] = _balances[username][currencyCode] - amount;
            _balances[username]["PLN"] = _balances[username]["PLN"] + earned;
            _transactions.Add(new Transaction
            {
                Id = _txCounter,
                Type = "SELL",
                Currency = currencyCode,
                Amount = amount,
                Rate = rate,
                PlnValue = earned,
                Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            });
            _txCounter = _txCounter + 1;
            return "OK: Sold " + amount + " " + currencyCode + " for " + earned.ToString("F2") + " PLN at rate " + rate.ToString("F4");
        }

        public List<Transaction> GetUserTransactions(string username)
        {
            return _transactions;
        }

        public List<HistoricalRate> GetHistoricalRates(string currencyCode, string startDate, string endDate)
        {
            var result = new List<HistoricalRate>();
            try
            {
                string url = "http://api.nbp.pl/api/exchangerates/rates/a/" + currencyCode + "/" + startDate + "/" + endDate + "/?format=json";
                using (var client = new WebClient())
                {
                    string json = client.DownloadString(url);
                    var obj = JObject.Parse(json);
                    foreach (var r in obj["rates"])
                    {
                        result.Add(new HistoricalRate
                        {
                            Date = r["effectiveDate"].ToString(),
                            Rate = r["mid"].Value<double>()
                        });
                    }
                }
            }
            catch { }
            return result;
        }
    }
}