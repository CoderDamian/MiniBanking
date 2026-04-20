using Mini_Banking.Domain.Exceptions;

namespace Mini_Banking.Domain.Entities
{
    public class User : Entity
    {
        public string DNI { get; private set; } = string.Empty;
        public string Nombres { get; private set; } = string.Empty;
        public string Apellidos { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;

        private User()
        {

        }

        public User(string dni,
                    string nombres,
                    string apellidos,
                    string email)
        {
            if (string.IsNullOrWhiteSpace(dni))
                throw new DomainException(DomainErrorCodes.EntityInvalidData, "DNI is required.", nameof(dni));

            if (string.IsNullOrWhiteSpace(nombres))
                throw new DomainException(DomainErrorCodes.EntityInvalidData, "Nombres is required.", nameof(nombres));

            if (string.IsNullOrWhiteSpace(apellidos))
                throw new DomainException(DomainErrorCodes.EntityInvalidData, "Apellidos is required.", nameof(apellidos));

            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException(DomainErrorCodes.EntityInvalidData, "Email is required.", nameof(email));

            //const string emailPattern = "^[^@\s]+@[^@\s]+\.[^@\s]+$";
            //if (!Regex.IsMatch(user.Email, emailPattern, RegexOptions.IgnoreCase))
            //    throw new ArgumentException("Email is not valid.", nameof(user.Email));

            this.DNI = dni;
            this.Nombres = nombres;
            this.Apellidos = apellidos;
            this.Email = email;
        }
    }
}
