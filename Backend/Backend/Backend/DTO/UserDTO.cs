using System.ComponentModel.DataAnnotations;
using Backend.Backend.Model;

namespace Backend.Backend.DTOs
{
    public class GetUserDTO
    {
        public string? DocumentSeries { get; set; }
        public required string Full_Name { get; set; }
        public required string Email { get; set; }
        public string? Phone_Number { get; set; }
        public char? Sex { get; set; }
        public DateTime? Birth_Date { get; set; }
        public string? Address { get; set; }
    }

    public class AddUserDTO
    {
        public required string Full_Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? Phone_Number { get; set; }
        public char? Sex { get; set; }
        public DateTime? Birth_Date { get; set; }
        public string? Address { get; set; }
    }

    public class LoginUserDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class LoginResult
    {
        public bool isSuccess { get; set; }
        public string? Token { get; set; }
        public string? Detail { get; set; }
    }

    public class RegisterDTO
    {
        public int StatusCode { get; set; }
        public string? Token { get; set; }
        public string? Detail { get; set;  }
        public GetUserDTO? Data { get; set; }
    }
}
