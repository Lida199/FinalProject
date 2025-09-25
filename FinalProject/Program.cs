using FinalProject;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Text.Json;
using NLog;


var filePath = @"C:\Users\lidashubitidze\source\repos\FinalProject\FinalProject\Details.json";
string jsonString = File.ReadAllText(filePath);


AccountDetails user = JsonSerializer.Deserialize<AccountDetails>(jsonString);


bool checkCardDetails()
{
Console.WriteLine("Enter Card Details:");
Console.Write("1.Card Number:");
var cardNumber = Console.ReadLine();
Console.Write("2.Expiration Date:");
var expirationDate = Console.ReadLine();
Console.Write("3.CVC:");
var cvc = Console.ReadLine();
return user != null && cardNumber == user.CardDetails.CardNumber &&
    expirationDate == user.CardDetails.ExpirationDate &&
    cvc == user.CardDetails.CVC;
}

bool result = checkCardDetails();

if (result)
{
checkPin();
}
else
{
Console.WriteLine("Data Is Not Valid. Please Provide Correct Data.");
}

bool checkPin()
{
Console.Write("Enter Pin:");
var pin = Console.ReadLine();
return user != null && user.PinCode == pin;
}

//ლიდა აქ უნდა შეამომწო ორჯერ რომ იძახებს
bool pinResult = checkPin();

if (pinResult)
{
getOperation();
}
else
{
Console.WriteLine("Pin Is Incorrect. Please Provide Correct Pin.");
}


void getOperation()
{
Console.WriteLine("Hello John Doe: \n1.Check Deposit\n2.Get Amount\n3.Get Last 5 Transactions\n4.Add Amount\n5.Change PIN\n6.Change Amount");
var operationType = Console.ReadLine();
switch (operationType)
{
case "1":
checkDeposit();
break;
case "2":
getAmount();
break;
case "3":
lastFiveTransactions();
break;
case "4":
addAmount();
break;
case "5":
changePin();
break;
case "6":
break;
default:
Console.WriteLine("Invalid Operation Type");
break;
}
}

void checkDeposit()
{
Console.WriteLine(
    $"Amount is:" +
    $"GEL : {user.AmountGEL} " +
    $"EUR : {user.AmountEUR} " +
    $"USD : {user.AmountUSD}"
    );

//ლიდა აქ უნდა დაალოგინო რომ იუზერმა შეამოწმა თავისი ბალანსი და ტრანზაქციებშიც ჩაამატო
addTransaction("CheckDeposit");
}

void getAmount()
{
Console.WriteLine(
    $"Please Select The Currency:" +
    $"1.GEL" +
    $"2.EUR" +
    $"3.USD"
    );
var currency = Console.ReadLine();
if (currency == "1" || currency == "2" || currency == "3")
{
Console.Write("Please Select The Amount:");
double amount;
bool isValidAmount = double.TryParse(Console.ReadLine(), out amount);
if (isValidAmount)
{
getOrAddAmount(currency, 1, amount);
}
else
{
Console.WriteLine("Invalid Amount.");
}
}
else
{
Console.WriteLine("Inavlid Currency Type!");
}
//ლიდა აქ უნდა დაალოგინო რომ იუზერმა გამოიტანა თანხა და ტრანსაქციებში ჩაამატო
}

void addAmount()
{
Console.WriteLine(
    $"Please Select The Currency:" +
    $"1.GEL" +
    $"2.EUR" +
    $"3.USD"
    );
var currency = Console.ReadLine();
if (currency == "1" || currency == "2" || currency == "3")
{
Console.Write("Please Insert The Amount:");
double amount;
bool isValidAmount = double.TryParse(Console.ReadLine(), out amount);
if (isValidAmount)
{
getOrAddAmount(currency, 2, amount);
}
else
{
Console.WriteLine("Invalid Amount.");
}
}
else
{
Console.WriteLine("Inavlid Currency Type!");
}
//ლიდა აქ უნდა დაალოგინო რომ იუზერმა გამოიტანა თანხა და ტრანსაქციებში ჩაამატო
}

void getOrAddAmount(string currency, int getOrAdd, double amount)
{
double userAmount = currency == "1" ? user.AmountGEL : currency == "2" ? user.AmountEUR : user.AmountUSD;
if (getOrAdd == 1)
{
if (amount > 0 && amount <= userAmount)
{
if (currency == "1")
user.AmountGEL -= amount;
else if (currency == "2")
user.AmountEUR -= amount;
else
user.AmountUSD -= amount;

var amountGEL = currency == "1" ? amount : 0;
var amountEUR = currency == "2" ? amount : 0;
var amountUSD = currency == "3" ? amount : 0;

addTransaction("GetAmount", amountGEL, amountUSD, amountEUR);
}
else if (amount > userAmount || amount <= 0)
{
Console.WriteLine(amount > userAmount ? "Insufficient Fund!" : "Invalid Amount");
}
}
else if (getOrAdd == 2)
{
if (amount > 0)
{
if (currency == "1")
user.AmountGEL += amount;
else if (currency == "2")
user.AmountEUR += amount;
else
user.AmountUSD += amount;

var amountGEL = currency == "1" ? amount : 0;
var amountEUR = currency == "2" ? amount : 0;
var amountUSD = currency == "3" ? amount : 0;

addTransaction("AddAmount", amountGEL, amountUSD, amountEUR);
}
else
{
Console.WriteLine("Invalid Amount!");
}
}
}


void lastFiveTransactions()
{
if (user.TransactionHistory == null || !user.TransactionHistory.Any())
{
Console.WriteLine("No Transactions Found.");
}
else
{
var lastFive = user.TransactionHistory
    .OrderByDescending(t => t.TransactionDate)
    .Take(5)
    .ToList();

foreach (var transaction in lastFive)
{
DateTime parsedDate = DateTime.Parse(transaction.TransactionDate);
string formatted = parsedDate.ToString("dd MMM yyyy, HH:mm");
Console.WriteLine($"{formatted}: {transaction.TransactionType} - GEL:{transaction.AmountGEL}, USD:{transaction.AmountUSD}, EUR:{transaction.AmountEUR}");
}
}
addTransaction("CheckLast5Transactions");
}


void addTransaction(string transactionType, double amountGEL = 0, double amountUSD = 0, double amountEUR = 0)
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
}


void changePin()
{
Console.Write("Please Input A New PIN:");
var newPin = Console.ReadLine();
if (newPin != null && newPin.Length == 4 && newPin.All(char.IsDigit))
{
user.PinCode = newPin;
addTransaction("PinChange");
}
else
{
Console.WriteLine("Please Check That You Are Only Using Digits And The Length Of The Pin Is 4.");
}
}


void changeAmount()
{
Console.WriteLine("Select The Currency To Convert From" +
    "1.GEL" +
    "2.EUR" +
    "3.USD");
var convertFrom = Console.ReadLine();

if(convertFrom != "1" || convertFrom != "2" || convertFrom != "3")
    {
        Console.WriteLine("Currency Not Found");
        return;
    }
 
   Console.WriteLine("Select The Currency To Convert To" +
    "1.GEL" +
    "2.EUR" +
    "3.USD");
    var convertTo = Console.ReadLine();

    if (convertTo != "1" || convertTo != "2" || convertTo != "3")
    {
        Console.WriteLine("Currency Not Found");
        return;
    }

    Console.WriteLine("Select The Amount To Be Converted");
    double convertedAmount;
    bool isAmountDouble =double.TryParse(Console.ReadLine(), out convertedAmount);

    if (isAmountDouble)
    {
    if (convertFrom == convertTo)
    {
        Console.WriteLine("You Have Selected Same Currencies");
    }
    else
    {
        var currencyFrom = int.Parse(convertFrom);
        var currencyTo = int.Parse(convertTo);
        var amountFrom = currencyFrom == 1 ? user.AmountGEL : currencyFrom == 2 ? user.AmountEUR : user.AmountUSD;
        var amountTo = currencyTo == 1 ? user.AmountGEL : currencyTo == 2 ? user.AmountEUR : user.AmountUSD;
        var total = convertedAmount * getRates((Currencies)currencyFrom, (Currencies)currencyTo);
        if(convertedAmount > 0 && convertedAmount <= amountFrom)
        {
                if(currencyFrom == 1)
                {
                   user.AmountGEL -= convertedAmount;
                }
                else if(currencyFrom == 2)
                {
                    user.AmountEUR -= convertedAmount;

                }
                else if(currencyFrom == 3)
                {
                    user.AmountUSD -= convertedAmount;
                }

                if (currencyTo == 1)
                {
                    user.AmountGEL += total;
                }
                else if (currencyTo == 2)
                {
                    user.AmountEUR += total;

                }
                else if (currencyTo == 3)
                {
                    user.AmountUSD += total;
                }
                var amountGEL = currencyFrom == 1 ? convertedAmount : currencyTo == 1 ? total : 0;
                var amountEUR = currencyFrom == 2 ? convertedAmount : currencyTo == 2 ? total : 0;
                var amountUSD = currencyFrom == 3 ? convertedAmount : currencyTo == 3 ? total : 0;
                addTransaction("ChangeAmount", amountGEL, amountUSD, amountEUR);
            }
        else
        {
                Console.WriteLine("Insufficient Funds!");
        }
    }
    }
    else
    {
        Console.WriteLine("Invalid Amount!");
    }
}

double getRates(Currencies from, Currencies to)
{
if (from == Currencies.GEL && to == Currencies.EUR) return 0.31;
if (from == Currencies.GEL && to == Currencies.USD) return 0.36;
if (from == Currencies.EUR && to == Currencies.GEL) return 3.1;
if (from == Currencies.EUR && to == Currencies.USD) return 1.14;
if (from == Currencies.USD && to == Currencies.EUR) return 0.83;
if (from == Currencies.USD && to == Currencies.GEL) return 2.7;
return 1;
}

public enum Currencies
{
    GEL = 1,
    EUR = 2,
    USD = 3
}

//Logger Logger = LogManager.GetCurrentClassLogger();

//Logger.Trace("This is a trace message.");
//Logger.Debug("This is a debug message.");
//Logger.Info("This is an informational message.");
//Logger.Warn("This is a warning message.");