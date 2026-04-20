using Microsoft.AspNetCore.Mvc;
using Mini_Banking.Application.Contracts;
using Mini_Banking.Application.DTOs;
using System.Runtime.InteropServices;

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

        [HttpPost]
        public async Task<IActionResult> Deposit([FromBody] CreateDepositDTO request)
        {
            var result = await _bankTransaction.DepositAsync(request);

            return Ok(result);
        }

        [HttpPost("withdrawl")]
        public async Task<IActionResult> Withdrawal([FromBody] CreateWithdrawalDTO request)
        {
            var result = await _bankTransaction.WithdrawalAsync(request);

            return Ok(result);
        }
    }
}
