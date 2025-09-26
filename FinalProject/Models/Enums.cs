namespace FinalProject.Models
{
    public enum Currencies
    {
        GEL = 1,
        EUR = 2,
        USD = 3
    }

    public enum OperationsEnum
    {
        CheckDeposit = 1,
        GetAmount = 2,
        GetLast5Transactions = 3,
        AddAmount = 4,
        ChangePIN = 5,
        ChangeAmount = 6
    }


    public enum GetOrAdd
    {
        Withdraw = 1,
        Deposit = 2
    }
}
