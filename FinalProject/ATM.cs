using System.Text.Json;
using FinalProject.Models;
using NLog;

namespace FinalProject
{
    public class ATM
    {
        private AccountDetails User { get; set; }
        private string FilePath { get; set; }
        private Operations Operations = new Operations();
        private Logger Logger = LogManager.GetCurrentClassLogger();

        public ATM(string filePath)
        {
            string jsonString = File.ReadAllText(filePath);
            this.FilePath = filePath;
            this.User = JsonSerializer.Deserialize<AccountDetails>(jsonString);
        }

        public void Authorize()
        {
            if (User == null) return;

            Logger.Info("Authorization started.");
            Console.WriteLine("Enter Card Details:");
            Console.Write("1.Card Number:");
            var cardNumber = Console.ReadLine();
            Console.Write("2.Expiration Date:");
            var expirationDate = Console.ReadLine();
            Console.Write("3.CVC:");
            var cvc = Console.ReadLine();

            bool areDetailsCorrect = cardNumber == User.CardDetails.CardNumber &&
                expirationDate == User.CardDetails.ExpirationDate &&
                cvc == User.CardDetails.CVC;

            if (areDetailsCorrect)
            {
                Logger.Info("Authorization successful.");
                CheckPin();
            }
            else
            {
                Console.WriteLine("Data Is Not Valid. Please Provide Correct Data.");
                Logger.Warn("Authorization failed.");
                Authorize();
            }
        }



        private void CheckPin()
        {
            if (User == null) return;
            Logger.Info("Checking PIN code.");
            Console.Write("Enter Pin:");
            var pin = Console.ReadLine();
            bool isPinCorrect = User.PinCode == pin;

            if (isPinCorrect)
            {
                Logger.Info("PIN Check Successful.");
                GetOperation();
            }
            else
            {
                Console.WriteLine("Pin Is Incorrect. Please Provide Correct Pin.");
                Logger.Warn("PIN Not Correct");
                Authorize();
            }
        }

        private void GetOperation()
        {
            if (User == null) return;
            Logger.Info("User Choosing Operation Type");
            Console.WriteLine($"Hello {User.FirstName} {User.LastName}: \n1.Check Deposit\n2.Get Amount\n3.Get Last 5 Transactions\n4.Add Amount\n5.Change PIN\n6.Change Amount");
            int operationType;
            bool isOperationInt = int.TryParse(Console.ReadLine(), out operationType);
            
            if (!isOperationInt)
            {
                Console.WriteLine("Please Choose The Correct Type");
                Logger.Warn("Incorrect Operation Type");
                GetOperation();
                return;
            }

            switch ((OperationsEnum)operationType)
            {
                case OperationsEnum.CheckDeposit:
                    Operations.checkDeposit(User, FilePath);
                    break;
                case OperationsEnum.GetAmount:
                    Operations.GetAmount(User, FilePath);
                    break;
                case OperationsEnum.GetLast5Transactions:
                    Operations.GetLastFiveTransactions(User, FilePath);
                    break;
                case OperationsEnum.AddAmount:
                    Operations.AddAmount(User, FilePath);
                    break;
                case OperationsEnum.ChangePIN:
                    Operations.ChangePIN(User, FilePath);
                    break;
                case OperationsEnum.ChangeAmount:
                    Operations.ChangeAmount(User, FilePath);
                    break;
                default:
                    Console.WriteLine("Invalid Operation Type. Please Try Again!");
                    Logger.Warn("Incorrect Operation Type");
                    GetOperation();
                    break;
            }
        }
    }
}
