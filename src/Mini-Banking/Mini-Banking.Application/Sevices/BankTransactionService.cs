using Mini_Banking.Application.Contracts;
using Mini_Banking.Application.DTOs;
using Mini_Banking.Domain.Entities;
using Mini_Banking.Domain.ValueObjects;

namespace Mini_Banking.Application.Sevices
{
    internal class BankTransactionService : IBankTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BankTransactionService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<Guid> DepositAsync(CreateDepositDTO depositDTO, CancellationToken cancellationToken = default)
        { 
            var account = await GetAccountByAsync(depositDTO.ReceiverAccountID, cancellationToken).ConfigureAwait(false);

            var bankTransaction = BankTransaction.FromDeposit(account, new Amount(depositDTO.Amount));
            bankTransaction.Execute();

            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken)
                        .ConfigureAwait(false);

                var newTransactionID = await _unitOfWork.BankTransactionRepository
                    .AddAsync(bankTransaction, cancellationToken)
                    .ConfigureAwait(false);

                await _unitOfWork.AccountRepository
                    .UpdateBalanceAsync(account, cancellationToken)
                    .ConfigureAwait(false);

                await _unitOfWork.BankTransactionRepository
                    .MarkAsCompletedAsync(newTransactionID)
                    .ConfigureAwait(false);

                await _unitOfWork.CommitTransactionAsync(cancellationToken)
                    .ConfigureAwait(false);

                await _unitOfWork.SaveChangesAsync(cancellationToken)
                    .ConfigureAwait(false);

                return newTransactionID;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }
        }

        private async Task<Account> GetAccountByAsync(int accountID, CancellationToken cancellationToken)
        {
            var account = await _unitOfWork.AccountRepository
                .GetByAsync(accountID, cancellationToken)
                .ConfigureAwait(false);

            if (account is null)
                throw new ArgumentNullException();

            return account;
        }

        //public Guid Transfer(AccountDTO sender, AccountDTO receiver, decimal amount)
        //{
        //    var amountToTransfer = new Amount(amount);

        //    var senderAccount = GetAnAccountFrom(sender);
        //    var receiverAccount = GetAnAccountFrom(receiver);

        //    var transfer = BankTransaction.FromTransfer(senderAccount, receiverAccount, amountToTransfer);

        //    transfer.Execute();

        //    return transfer.ID;
        //}

        public async Task<Guid> WithdrawalAsync(CreateWithdrawalDTO withdrawalDTO, CancellationToken cancellationToken = default)
        {
            var account = await GetAccountByAsync(withdrawalDTO.SenderAccountID, cancellationToken).ConfigureAwait(false);

            var bankTransaction = BankTransaction.FromWithdrawal(account, new Amount(withdrawalDTO.Amount));
            bankTransaction.Execute();

            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken)
                        .ConfigureAwait(false);

                var newTransactionID = await _unitOfWork.BankTransactionRepository
                    .AddAsync(bankTransaction, cancellationToken)
                    .ConfigureAwait(false);

                await _unitOfWork.AccountRepository
                    .UpdateBalanceAsync(account, cancellationToken)
                    .ConfigureAwait(false);

                await _unitOfWork.BankTransactionRepository
                    .MarkAsCompletedAsync(newTransactionID)
                    .ConfigureAwait(false);

                await _unitOfWork.CommitTransactionAsync(cancellationToken)
                    .ConfigureAwait(false);

                await _unitOfWork.SaveChangesAsync(cancellationToken)
                    .ConfigureAwait(false);

                return newTransactionID;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }
        }
    }
}
