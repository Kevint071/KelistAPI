namespace Application.Users.Dtos
{
    public record UserDTO(
           Guid Id,
           string Name,
           string LastName,
           string Email
       )
    {
        public string FullName => $"{Name} {LastName}";
    }
}
