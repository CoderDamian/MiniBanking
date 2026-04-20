namespace Mini_Banking.Application.DTOs
{
    public record CreateUserDTO
    {
        public string DNI { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
