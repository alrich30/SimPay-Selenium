using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimPay.Domain.Accounts
{
    public sealed class Account
    {
        private Account()
        {
        }

        public Account(
            Guid userId,
            string accountNumber,
            string currency)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException(
                    "The user identifier is required.",
                    nameof(userId));
            }
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                throw new ArgumentException(
                    "The account number is required.",
                    nameof(accountNumber));
            }

            if (string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentException(
                    "The currency is required.",
                    nameof(currency));
            }

            Id = Guid.NewGuid();
            UserId = userId;
            AccountNumber = accountNumber.Trim();
            Currency = currency.Trim().ToUpperInvariant();
            Balance = 0m;
            Status = AccountStatus.Active;
            CreatedAt = DateTime.UtcNow;

        }

        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public string AccountNumber { get; private set; } = string.Empty;

        public decimal Balance { get; private set; }

        public string Currency { get; private set; } = string.Empty;

        public AccountStatus Status { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public void Deposit(decimal amount)
        {
            EnsureActive();

            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    "The deposit amount must be greater than zero.");
            }

            Balance += amount;
        }

        public void Debit(decimal amount)
        {
            EnsureActive();

            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    "The debit amount must be greater than zero.");
            }

            if (Balance < amount)
            {
                throw new InvalidOperationException(
                    "The account has insufficient funds.");
            }

            Balance -= amount;
        }

        public void Suspend()
        {
            if (Status == AccountStatus.Closed)
            {
                throw new InvalidOperationException(
                    "A closed account cannot be suspended.");
            }

            Status = AccountStatus.Suspended;
        }

        public void Activate()
        {
            if (Status == AccountStatus.Closed)
            {
                throw new InvalidOperationException(
                    "A closed account cannot be activated.");
            }

            Status = AccountStatus.Active;
        }

        public void Close()
        {
            if (Balance != 0)
            {
                throw new InvalidOperationException(
                    "An account with an outstanding balance cannot be closed.");
            }

            Status = AccountStatus.Closed;
        }

        private void EnsureActive()
        {
            if (Status != AccountStatus.Active)
            {
                throw new InvalidOperationException(
                    "The account is not active.");
            }
        }
    }
}
