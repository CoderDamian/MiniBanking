using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mini_Banking.Application.Contracts;
using Mini_Banking.Application.DTOs;

namespace Mini_Banking.APIRest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly IBankTransactionService _bankTransaction;

        public TransactionController(IBankTransactionService bankTransaction)
        {
            this._bankTransaction = bankTransaction;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] CreateDepositRequest request)
        {
            var key = HttpContext.Items["IdempotencyKey"]?.ToString();
            var requestHash = HttpContext.Items["RequestHash"]?.ToString();

            var result = await _bankTransaction.DepositAsync(request,
                                                             key ?? string.Empty,
                                                             requestHash ?? string.Empty,
                                                             new CancellationToken());

            return Ok(result);
        }

        [HttpPost("withdrawl")]
        public async Task<IActionResult> Withdrawal([FromBody] CreateWithdrawalDTO request)
        {
            var key = HttpContext.Items["IdempotencyKey"]?.ToString();
            var requestHash = HttpContext.Items["RequestHash"]?.ToString();

            var result = await _bankTransaction.WithdrawalAsync(request,
                                                                key ?? string.Empty,
                                                                requestHash ?? string.Empty);

            return Ok(result);
        }
    }
}
