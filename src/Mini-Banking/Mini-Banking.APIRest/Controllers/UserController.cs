using Microsoft.AspNetCore.Mvc;
using Mini_Banking.Application.Contracts;
using Mini_Banking.Application.DTOs;

namespace Mini_Banking.APIRest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            this._userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDTO request, CancellationToken cancellationToken = default)
        {
            var result = await _userService.CreateUserAsync(request, cancellationToken).ConfigureAwait(false);

            return Ok(result);
        }
    }
}
