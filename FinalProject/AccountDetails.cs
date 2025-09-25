using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FinalProject
{
    internal class AccountDetails
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }


        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("cardDetails")]
        public Details CardDetails { get; set; }

        [JsonPropertyName("pinCode")]
        public string PinCode { get; set; }

        [JsonPropertyName("amountGEL")]
        public double AmountGEL { get; set; }
        [JsonPropertyName("amountUSD")]
        public double AmountUSD { get; set; }
        [JsonPropertyName("amountEUR")]
        public double AmountEUR { get; set; }

        [JsonPropertyName("transactionHistory")]
        public List<Transaction> TransactionHistory { get; set; }

        public class Details
        {
            [JsonPropertyName("cardNumber")]
            public string CardNumber { get; set; }
            [JsonPropertyName("expirationDate")]
            public string ExpirationDate { get; set; }
            public string CVC { get; set; }
        }

        public class Transaction
        {
            [JsonPropertyName("transactionDate")]
            public string TransactionDate { get; set; }

            [JsonPropertyName("transactionType")]
            public string TransactionType { get; set; }
            [JsonPropertyName("amountGEL")]
            public double AmountGEL { get; set; }
            [JsonPropertyName("amountUSD")]
            public double AmountUSD { get; set; }
            [JsonPropertyName("amountEUR")]
            public double AmountEUR { get; set; }
        }
    }
}
