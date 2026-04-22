using Mini_Banking.Application.Contracts;
using Mini_Banking.Application.DTOs;
using Mini_Banking.Application.Exceptions;
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

#warning Store structured response
        public async Task<string> DepositAsync(CreateDepositDTO depositDTO, string key, string requestHash, CancellationToken cancellationToken = default)
        {
            if (String.IsNullOrEmpty(key) || String.IsNullOrEmpty(requestHash))
                throw new ApplicationServiceException(ApplicationErrorCode.IdempotencyInvalid, "key or request hash can not be null or empty");

            var idempotency = await _unitOfWork.IdempotencyRepository
                .GetByAsync(key, cancellationToken)
                .ConfigureAwait(false);

            if (idempotency is null)
            {
                idempotency = new Idempotency(key, requestHash);

                await _unitOfWork.BeginTransactionAsync(cancellationToken)
                        .ConfigureAwait(false);
                
                try
                {
                    // 1. Try to claim idempotency
                    await _unitOfWork.IdempotencyRepository
                        .CreateInProgressAsync(idempotency.Key, idempotency.RequestHash, cancellationToken)
                        .ConfigureAwait(false);

                    // 2. Execute business logic
                    var account = await GetAccountByAsync(depositDTO.ReceiverAccountID, cancellationToken).ConfigureAwait(false);

                    var bankTransaction = BankTransaction.FromDeposit(account, new Amount(depositDTO.Amount));
                    bankTransaction.Execute();

                    var newTransactionID = await _unitOfWork.BankTransactionRepository
                        .AddAsync(bankTransaction, cancellationToken)
                        .ConfigureAwait(false);

                    await _unitOfWork.AccountRepository
                        .UpdateBalanceAsync(account, cancellationToken)
                        .ConfigureAwait(false);

                    await _unitOfWork.BankTransactionRepository
                        .MarkAsCompletedAsync(newTransactionID)
                        .ConfigureAwait(false);

                    // 3. Build response (business result, NOT HTTP)
                    idempotency.SetResponseBody(newTransactionID.ToString());
                    idempotency.SetStatusCode(200);
                    idempotency.MarkAsCompleted();

                    // 4. Mark idempotency as completed
                    await _unitOfWork.IdempotencyRepository
                        .MarkAsCompletedAsync(idempotency, cancellationToken)
                        .ConfigureAwait(false);

                    await _unitOfWork.SaveChangesAsync(cancellationToken)
                        .ConfigureAwait(false);

                    await _unitOfWork.CommitTransactionAsync(cancellationToken)
                        .ConfigureAwait(false);

                    return newTransactionID.ToString();
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);
                    throw;
                }
            }
            else
            {
                if (idempotency.RequestHash != requestHash)
                    throw new ApplicationServiceException(ApplicationErrorCode.IdempotencyInvalid, "Request mismatch");

                if (idempotency.Status == Enums.IdempontecyStatus.Completed)
                    return idempotency.ResponseBody.ToString();
                else
                    throw new ApplicationServiceException(ApplicationErrorCode.IdempotencyInvalid, "Request already in progress");
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

        public async Task<string> WithdrawalAsync(CreateWithdrawalDTO withdrawalDTO, CancellationToken cancellationToken = default)
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

                return newTransactionID.ToString();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }
        }
    }
}
