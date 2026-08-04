using SimPay.Domain.Accounts;

namespace SimPay.Tests.Accounts;

public class AccountTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateAccount()
    {
        Guid userId = Guid.NewGuid();

        var account = new Account(
            userId,
            "ACC-0001",
            "eur");

        Assert.NotEqual(Guid.Empty, account.Id);
        Assert.Equal(userId, account.UserId);
        Assert.Equal("ACC-0001", account.AccountNumber);
        Assert.Equal("EUR", account.Currency);
        Assert.Equal(0m, account.Balance);
        Assert.Equal(AccountStatus.Active, account.Status);
    }

    [Fact]
    public void Deposit_WithValidAmount_ShouldIncreaseBalance()
    {
        var account = CreateAccount();

        account.Deposit(500m);

        Assert.Equal(500m, account.Balance);
    }

    [Fact]
    public void Deposit_WithNegativeAmount_ShouldThrowException()
    {
        var account = CreateAccount();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => account.Deposit(-100m));
    }

    [Fact]
    public void Debit_WithEnoughFunds_ShouldDecreaseBalance()
    {
        var account = CreateAccount();
        account.Deposit(500m);

        account.Debit(200m);

        Assert.Equal(300m, account.Balance);
    }

    [Fact]
    public void Debit_WithInsufficientFunds_ShouldThrowException()
    {
        var account = CreateAccount();
        account.Deposit(100m);

        Assert.Throws<InvalidOperationException>(
            () => account.Debit(200m));
    }

    private static Account CreateAccount()
    {
        return new Account(
            Guid.NewGuid(),
            "ACC-0001",
            "EUR");
    }
}