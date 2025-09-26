using System.Text.Json;
using FinalProject.Models;
using NLog;

namespace FinalProject
{
    internal class Operations
    {
        private Logger Logger = LogManager.GetCurrentClassLogger();

        public void checkDeposit(AccountDetails user, string filePath)
        {
            if (user == null) return;
            Logger.Info("User Checking Deposit");
            Console.WriteLine($"Amount is:\nGEL : {user.AmountGEL}\nEUR : {user.AmountEUR}\nUSD : {user.AmountUSD}");

            Logger.Info("User Checked Deposit");
            AddTransaction(user, filePath, "CheckDeposit");
        }

        public void GetLastFiveTransactions(AccountDetails user, string filePath)
        {
            Logger.Info("User Checking Last Transactions");

            if (user.TransactionHistory == null || !user.TransactionHistory.Any())
            {
                Console.WriteLine("No Transactions Found.");
                Logger.Warn("User Has No Transactions");
            }
            else
            {
                var lastFive = user.TransactionHistory
                    .OrderByDescending(t => t.TransactionDate)
                    .Take(5);

                foreach (var transaction in lastFive)
                {
                    DateTime parsedDate = DateTime.Parse(transaction.TransactionDate);
                    string formatted = parsedDate.ToString("dd MMM yyyy, HH:mm");
                    Console.WriteLine($"{formatted}: {transaction.TransactionType} - GEL:{transaction.AmountGEL}, USD:{transaction.AmountUSD}, EUR:{transaction.AmountEUR}");
                }
            }
            Logger.Info("Transaction History Checked.");
            AddTransaction(user,filePath,"CheckLast5Transactions");
        }

        public void ChangePIN(AccountDetails user, string filePath)
        {
            Logger.Info("Changing PIN.");

            Console.Write("Please Input A New PIN:");
            var newPin = Console.ReadLine();
            if (newPin != null && newPin.Length == 4 && newPin.All(char.IsDigit) && user.PinCode != newPin)
            {
                user.PinCode = newPin;
                Console.WriteLine("PIN Has Changed Successfully!");
                Logger.Info("Changing PIN Successfully.");
                AddTransaction(user, filePath, "PinChange");
            }
            else if(user.PinCode == newPin)
            {
                Console.WriteLine("New PIN Should Be Different From The Old PIN!");
                Logger.Warn("Changing PIN Unsuccesful, Same As Previous One.");
                ChangePIN(user, filePath);
            }else
            {
                Logger.Warn("Changing PIN Unsuccesful, Incorrect Syntax.");
                Console.WriteLine("Please Check That You Are Only Using Digits And The Length Of The Pin Is 4.");
                ChangePIN(user, filePath);
            }
        }

        public void ChangeAmount(AccountDetails user, string filePath)
        {
            Console.WriteLine("Select The Currency To Convert From:\n1.GEL\n2.EUR\n3.USD");
            Logger.Info("Currency Conversion Start.");
            int convertFrom;
            bool isCurrencyFromInteger = int.TryParse(Console.ReadLine(),out convertFrom);

            if (!isCurrencyFromInteger || !IsCurrencyCorrect(convertFrom))
            {
                Console.WriteLine("Currency Not Found.");
                Logger.Warn("Currency Conversion- Currency To Convert From Not Found.");
                ChangeAmount(user, filePath);
                return;
            }

            Console.WriteLine("Select The Currency To Convert To:\n1.GEL\n2.EUR\n3.USD");
            int convertTo;
            bool isCurrencyToInteger = int.TryParse(Console.ReadLine(), out convertTo);

            if (!isCurrencyToInteger || !IsCurrencyCorrect(convertTo))
            {
                Console.WriteLine("Currency Not Found.");
                Logger.Warn("Currency Conversion- Currency To Convert To Not Found.");
                ChangeAmount(user, filePath);
                return;
            }

            if (convertFrom == convertTo)
            {
                Console.WriteLine("You Have Selected Same Currencies");
                Logger.Warn("Currency Conversion- Same Currencies.");
                ChangeAmount(user, filePath);
                return;
            }

            Console.Write("Select The Amount To Be Converted:");
            double convertedAmount;
            bool isAmountDouble = double.TryParse(Console.ReadLine(), out convertedAmount);

            if (isAmountDouble)
            {
                var amountFrom = convertFrom == 1 ? user.AmountGEL : convertFrom == 2 ? user.AmountEUR : user.AmountUSD;
                var amountTo = convertTo == 1 ? user.AmountGEL : convertTo == 2 ? user.AmountEUR : user.AmountUSD;
                var total = convertedAmount * GetRates((Currencies)convertFrom, (Currencies)convertTo);
                if (convertedAmount > 0 && convertedAmount <= amountFrom)
                {
                    if ((Currencies)convertFrom == Currencies.GEL)
                    {
                        user.AmountGEL -= convertedAmount;
                    }
                    else if ((Currencies)convertFrom == Currencies.EUR)
                    {
                        user.AmountEUR -= convertedAmount;

                    }
                    else if ((Currencies)convertFrom == Currencies.USD)
                    {
                        user.AmountUSD -= convertedAmount;
                    }

                    if ((Currencies)convertTo == Currencies.GEL)
                    {
                        user.AmountGEL += total;
                    }
                    else if ((Currencies)convertTo == Currencies.EUR)
                    {
                        user.AmountEUR += total;

                    }
                    else if ((Currencies)convertTo == Currencies.USD)
                    {
                        user.AmountUSD += total;
                    }
                    var amountGEL = convertFrom == 1 ? convertedAmount : convertTo == 1 ? total : 0;
                    var amountEUR = convertFrom == 2 ? convertedAmount : convertTo == 2 ? total : 0;
                    var amountUSD = convertFrom == 3 ? convertedAmount : convertTo == 3 ? total : 0;
                    Console.WriteLine("Amount Converted Successfully!");
                    Logger.Info("Currency Conversion End Successfully.");
                    AddTransaction(user,filePath,"ChangeAmount", amountGEL, amountUSD, amountEUR);
                    }
                    else
                    {
                    Console.WriteLine("Insufficient Funds! Please Select Amount Within Range.");
                    Logger.Warn("Currency Conversion- Insufficient Funds.");
                    ChangeAmount(user, filePath);
                    }
            }
            else
            {
                Console.WriteLine("Invalid Amount!");
                Logger.Warn("Currency Conversion- Invalid Amount.");
                ChangeAmount(user,filePath);
            }
        }


        public void GetAmount(AccountDetails user, string filePath)
        {
            CheckCurrencyOnWithdrawalAndDeposit(user, filePath, 1);
        }

        public void AddAmount(AccountDetails user, string filePath)
        {
            CheckCurrencyOnWithdrawalAndDeposit(user, filePath, 2);
        }

        private void GetOrAddAmount(AccountDetails user,string filePath, int currency, int getOrAdd, double amount)
        {
            double userAmount = (Currencies)currency == Currencies.GEL ? user.AmountGEL : (Currencies)currency == Currencies.EUR ? user.AmountEUR : user.AmountUSD;
            if (amount <= 0) {
                Console.WriteLine("Invalid Amount!");
                CheckCurrencyOnWithdrawalAndDeposit(user, filePath, getOrAdd);
                return;
            }else if((GetOrAdd)getOrAdd == GetOrAdd.Withdraw && amount > userAmount)
            {
                Console.WriteLine("Insufficient funds!");
                CheckCurrencyOnWithdrawalAndDeposit(user, filePath, getOrAdd);
                return;
            }
            else
            {
                if ((Currencies)currency == Currencies.GEL)
                    if ((GetOrAdd)getOrAdd == GetOrAdd.Withdraw) user.AmountGEL -= amount;
                    else user.AmountGEL += amount;
                else if ((Currencies)currency == Currencies.EUR)
                    if ((GetOrAdd)getOrAdd == GetOrAdd.Withdraw) user.AmountEUR -= amount;
                    else user.AmountEUR += amount;
                else
                    if ((GetOrAdd)getOrAdd == GetOrAdd.Withdraw) user.AmountUSD -= amount;
                    else user.AmountUSD += amount;
                
                var amountGEL = currency == 1 ? amount : 0;
                var amountEUR = currency == 2 ? amount : 0;
                var amountUSD = currency == 3 ? amount : 0;

                string transactionType = (GetOrAdd)getOrAdd == GetOrAdd.Withdraw ? "GetAmount" : "AddAmount";
                Console.WriteLine("Operation Was Successful.");
                Logger.Info($"{transactionType} Operation Successful.");
                AddTransaction(user, filePath, transactionType, amountGEL, amountUSD, amountEUR);
            }
        }

        private void CheckCurrencyOnWithdrawalAndDeposit(AccountDetails user, string filePath, int depositOrWithdrawal)
        {
            Console.WriteLine("Please Select The Currency:\n1.GEL\n2.EUR\n3.USD");
            int currency;
            bool isCurrencyInteger = int.TryParse(Console.ReadLine(), out currency);
            if (isCurrencyInteger && IsCurrencyCorrect(currency))
            {
                Console.Write("Please Insert The Amount:");
                double amount;
                bool isValidAmount = double.TryParse(Console.ReadLine(), out amount);
                if (isValidAmount)
                {
                    GetOrAddAmount(user, filePath, currency, 2, amount);
                }
                else
                {
                    Console.WriteLine("Invalid Amount.");
                    if((GetOrAdd)depositOrWithdrawal == GetOrAdd.Withdraw)
                    {
                        this.GetAmount(user, filePath);
                    }
                    else
                    {
                        this.AddAmount(user,filePath);
                    }
                }
            }
            else
            {
                Console.WriteLine("Inavlid Currency Type!");
                Logger.Warn("Inavlid Currency Type!");
                if ((GetOrAdd)depositOrWithdrawal == GetOrAdd.Withdraw)
                {
                    this.GetAmount(user, filePath);
                }
                else
                {
                    this.AddAmount(user, filePath);
                }
            }
        }

        private double GetRates(Currencies from, Currencies to)
        {
            if (from == Currencies.GEL && to == Currencies.EUR) return 0.31;
            else if (from == Currencies.GEL && to == Currencies.USD) return 0.36;
            else if (from == Currencies.EUR && to == Currencies.GEL) return 3.1;
            else if (from == Currencies.EUR && to == Currencies.USD) return 1.14;
            else if (from == Currencies.USD && to == Currencies.EUR) return 0.83;
            else return 2.7;
        }

        private void AddTransaction(AccountDetails user, string filePath, string transactionType, double amountGEL = 0, double amountUSD = 0, double amountEUR = 0)
        {
            user.TransactionHistory.Add(new AccountDetails.Transaction
            {
                TransactionDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                TransactionType = transactionType,
                AmountGEL = amountGEL,
                AmountUSD = amountUSD,
                AmountEUR = amountEUR
            });

            string updatedJson = JsonSerializer.Serialize(user, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, updatedJson);
            Logger.Info("Transaction History Updated Successfully.");
        }

        private bool IsCurrencyCorrect(int Currency)
        {
            return (Currencies)Currency == Currencies.GEL || (Currencies)Currency == Currencies.EUR || (Currencies)Currency == Currencies.USD;
        }
    }
}
