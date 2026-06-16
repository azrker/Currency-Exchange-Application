using System;
using CurrencyExchange.TestClient.CurrencyServiceRef;

namespace CurrencyExchange.TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new CurrencyServiceClient();
            Console.WriteLine(client.HelloWorld());
            Console.WriteLine("USD rate: " + client.GetExchangeRate("USD").ToString("F4"));
            Console.WriteLine("EUR rate: " + client.GetExchangeRate("EUR").ToString("F4"));
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}