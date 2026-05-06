using Mini_Banking.Application.Contracts;
using Mini_Banking.Application.DTOs;
using Mini_Banking.Application.Exceptions;
using Mini_Banking.Domain.Entities;
using Mini_Banking.Domain.Enums;
using Mini_Banking.Domain.ValueObjects;
using System.Text.Json;

namespace Mini_Banking.Application.Sevices
{
    internal class BankTransactionService : IBankTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BankTransactionService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<CreateDepositResponse> DepositAsync(CreateDepositRequest depositDTO, string key, string requestHash, CancellationToken cancellationToken = default)
        {
            ValidateKeyRequestHash(key, requestHash);

            Idempotency? idempotency = await GetIdempotencyBy(key, cancellationToken).ConfigureAwait(false);

            if (idempotency is not null)
            {
                ValidateIdempotency(requestHash, idempotency);
                return JsonSerializer.Deserialize<CreateDepositResponse>(idempotency.ResponseBody);
            }

            idempotency = new Idempotency(key, requestHash);

            await _unitOfWork.BeginTransactionAsync(cancellationToken)
                    .ConfigureAwait(false);

            bool idempotencyClaimed = false;

            try
            {
                // Claim idempotency for this request
                idempotencyClaimed = await ClaimIdempotencyAsync(idempotency, cancellationToken).ConfigureAwait(false);

                // Execute business logic and persist changes
                var newTransactionID = await ExecuteDepositBusinessLogic(depositDTO, cancellationToken).ConfigureAwait(false);
                var response = new CreateDepositResponse(newTransactionID);

                // Persist idempotency success and commit
                var responseBody = JsonSerializer.Serialize(response);
                await PersistIdempotencySuccessAsync(idempotency, responseBody, cancellationToken).ConfigureAwait(false);

                await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                await _unitOfWork.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);

                return response;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);

                // Mark idempotency as failed only if it was claimed earlier
                await MarkIdempotencyFailedIfClaimedAsync(key, idempotencyClaimed, cancellationToken).ConfigureAwait(false);

                throw;
            }
        }

        private async Task<Idempotency?> GetIdempotencyBy(string key, CancellationToken cancellationToken)
        {
            return await _unitOfWork.IdempotencyRepository
                            .GetByAsync(key, cancellationToken)
                            .ConfigureAwait(false);
        }

        private static void ValidateKeyRequestHash(string key, string requestHash)
        {
            if (String.IsNullOrEmpty(key) || String.IsNullOrEmpty(requestHash))
                throw new ApplicationServiceException(ApplicationErrorCode.IdempotencyInvalid, "key or request hash can not be null or empty");
        }

        private async Task<bool> ClaimIdempotencyAsync(Idempotency idempotency, CancellationToken cancellationToken)
        {
            await _unitOfWork.IdempotencyRepository
                .CreateInProgressAsync(idempotency.Key, idempotency.RequestHash, cancellationToken)
                .ConfigureAwait(false);

            return true;
        }

        private async Task<Guid> ExecuteDepositBusinessLogic(CreateDepositRequest depositDTO, CancellationToken cancellationToken)
        {
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

            return newTransactionID;
        }

        private async Task PersistIdempotencySuccessAsync(Idempotency idempotency, string responseBody, CancellationToken cancellationToken)
        {
            // 3. Build response (business result, NOT HTTP)
            idempotency.SetResponseBody(responseBody);
            idempotency.SetStatusCode(200);
            idempotency.MarkAsCompleted();

            // 4. Mark idempotency as completed
            await _unitOfWork.IdempotencyRepository
                .MarkAsCompletedAsync(idempotency, cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task MarkIdempotencyFailedIfClaimedAsync(string key, bool claimed, CancellationToken cancellationToken)
        {
            if (!claimed)
                return;

            try
            {
                await _unitOfWork.IdempotencyRepository
                    .MarkAsFailedAsync(key, cancellationToken)
                    .ConfigureAwait(false);

                await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // Swallow to avoid masking the original exception; consider logging here.
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

        public async Task<string> WithdrawalAsync(CreateWithdrawalDTO withdrawalDTO, string key, string requestHash, CancellationToken cancellationToken = default)
        {
            ValidateKeyRequestHash(key, requestHash);

            Idempotency? idempotency = await GetIdempotencyBy(key, cancellationToken).ConfigureAwait(false);

            if (idempotency is not null)
            {
                ValidateIdempotency(requestHash, idempotency);
                return JsonSerializer.Deserialize<string>(idempotency.ResponseBody);
            }

            bool idempotencyClaimed = false;

            try
            {
                idempotency = new Idempotency(key, requestHash);

                await _unitOfWork.BeginTransactionAsync(cancellationToken)
                        .ConfigureAwait(false);

                // Claim idempotency for this request
                idempotencyClaimed = await ClaimIdempotencyAsync(idempotency, cancellationToken).ConfigureAwait(false);

                Guid newTransactionID = await ExecuteWithdrawalBusinessLogic(withdrawalDTO, cancellationToken).ConfigureAwait(false);

                var responseBody = JsonSerializer.Serialize(newTransactionID.ToString());
                await PersistIdempotencySuccessAsync(idempotency, responseBody, cancellationToken).ConfigureAwait(false);

                await _unitOfWork.SaveChangesAsync(cancellationToken)
                    .ConfigureAwait(false);

                await _unitOfWork.CommitTransactionAsync(cancellationToken)
                    .ConfigureAwait(false);

                return newTransactionID.ToString();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);

                // Mark idempotency as failed only if it was claimed earlier
                await MarkIdempotencyFailedIfClaimedAsync(key, idempotencyClaimed, cancellationToken).ConfigureAwait(false);

                throw;
            }
        }

        private static void ValidateIdempotency(string requestHash, Idempotency idempotency)
        {
            if (idempotency.RequestHash != requestHash)
                throw new ApplicationServiceException(ApplicationErrorCode.IdempotencyConflict, "Request mismatch");

            if (idempotency.Status == IdempotencyStatus.InProgress)
                throw new ApplicationServiceException(ApplicationErrorCode.IdempotencyConflict, "Idempotency in progress");

            if (idempotency.Status == IdempotencyStatus.Failed)
                throw new ApplicationServiceException(ApplicationErrorCode.IdempotencyConflict, "Idempotency failed");
        }

        private async Task<Guid> ExecuteWithdrawalBusinessLogic(CreateWithdrawalDTO withdrawalDTO, CancellationToken cancellationToken)
        {
            var account = await GetAccountByAsync(withdrawalDTO.SenderAccountID, cancellationToken).ConfigureAwait(false);

            var bankTransaction = BankTransaction.FromWithdrawal(account, new Amount(withdrawalDTO.Amount));
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

            throw new Exception();

            //return newTransactionID;
        }
    }
}
