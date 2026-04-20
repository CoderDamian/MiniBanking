using Mini_Banking.Application.Contracts;
using Mini_Banking.Application.DTOs;
using Mini_Banking.Domain.Entities;

namespace Mini_Banking.Application.Sevices
{
    internal class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<int> CreateUserAsync(CreateUserDTO createUserDTO, CancellationToken cancellationToken = default)
        {
            if (createUserDTO is null)
                throw new ArgumentNullException(nameof(createUserDTO));

            var user = new User(createUserDTO.DNI,
                                createUserDTO.Nombres,
                                createUserDTO.Apellidos,
                                createUserDTO.Email);
            
            var newUserID = await _unitOfWork.UserRepository
                .AddUserAsync(user, cancellationToken)
                .ConfigureAwait(false);

            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return newUserID();
        }
    }
}
