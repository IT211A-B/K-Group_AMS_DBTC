using System.ComponentModel.DataAnnotations;

namespace Backend.Backend.DTO
{
    public class GetUserDTO
    {
        public int User_ID { get; set; }
        public string? Full_Name { get; set; }
        public required string Email { get; set; }
        public string? Phone_Number { get; set; }
        public char? Gender { get; set; }
        public DateTime? Birth_Date { get; set; }
        public string? Address { get; set; }
        public int? UserGroup_ID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
    }

    public class AddUserDTO
    {
        public int User_ID { get; set; }
        public string? Full_Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? Phone_Number { get; set; }
        public char? Gender { get; set; }
        public DateTime? Birth_Date { get; set; }
        public string? Address { get; set; }
        public int? UserGroup_ID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
    }

    public class LoginUserDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class LoginResult
    {
        public bool isSuccess { get; set; }  
        public string? Detail { get; set; }
    }
}
