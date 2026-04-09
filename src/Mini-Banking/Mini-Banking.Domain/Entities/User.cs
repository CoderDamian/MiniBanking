namespace Mini_Banking.Domain.Entities
{
    internal class User: Entity
    {
        public string DNI { get; private set; }
        public string Nombres { get; private set; }
        public string Apellidos { get; private set; }
        public string Email { get; private set; }

        public User(string dni,
                    string nombres,
                    string apellidos,
                    string email)
        {
            this.DNI = dni;
            this.Nombres = nombres;
            this.Apellidos = apellidos;
            this.Email = email;
        }
    }
}
