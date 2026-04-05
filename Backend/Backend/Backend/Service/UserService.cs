using Microsoft.AspNetCore.Mvc;
using Backend.Backend.DTO;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Model;

namespace Backend.Backend.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<GetUserDTO>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new GetUserDTO
            {
                Full_Name = u.Full_Name,
                Email = u.Email,
                Phone_Number = u.Phone_Number,
                Gender = u.Gender,
                Birth_Date = u.Birth_Date,
                Address = u.Address,
                UserGroup_ID = u.UserGroup_ID,
                CreatedAt = u.CreatedAt,
                LastUpdatedAt = u.LastUpdatedAt,
                CreatedBy = u.CreatedBy,
                LastUpdatedBy = u.LastUpdatedBy
            });
        }

        public async Task<GetUserDTO?> GetByIdAsync(int id)
        {
            var u = await _userRepository.GetByIdAsync(id);
            if (u == null) return null;

            return new GetUserDTO
            {
                User_ID = u.User_ID,
                Full_Name = u.Full_Name,
                Email = u.Email,
                Phone_Number = u.Phone_Number,
                Gender = u.Gender,
                Birth_Date = u.Birth_Date,
                Address = u.Address,
                UserGroup_ID = u.UserGroup_ID,
                CreatedAt = u.CreatedAt,
                LastUpdatedAt = u.LastUpdatedAt,
                CreatedBy = u.CreatedBy,
                LastUpdatedBy = u.LastUpdatedBy
            };
        }

        public async Task<GetUserDTO> AddAsync(AddUserDTO userDto)
        {

            string Pass_Hash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            var user = new User
            {
                Full_Name = userDto.Full_Name!,
                Email = userDto.Email,
                PassHash = Pass_Hash,
                Phone_Number = userDto.Phone_Number,
                Gender = userDto.Gender,
                Birth_Date = userDto.Birth_Date,
                Address = userDto.Address,
                UserGroup_ID = userDto.UserGroup_ID,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                CreatedBy = userDto.LastUpdatedBy,
                LastUpdatedBy = userDto.LastUpdatedBy
            };

            await _userRepository.AddAsync(user);

            return new GetUserDTO
            {
                User_ID = user.User_ID,
                Full_Name = user.Full_Name,
                Email = user.Email,
                Phone_Number = user.Phone_Number,
                Gender = user.Gender,
                Birth_Date = user.Birth_Date,
                Address = user.Address,
                UserGroup_ID = user.UserGroup_ID,
                CreatedAt = user.CreatedAt,
                LastUpdatedAt = user.LastUpdatedAt,
                CreatedBy = user.CreatedBy,
                LastUpdatedBy = user.LastUpdatedBy
            };
        }

        public async Task<LoginResult> LoginAsync(LoginUserDto login)
        {
            var user = await _userRepository.GetByEmailOrUsernameAsync(login.Email);

            if (user == null) return new LoginResult
            {
                isSuccess = false,
                Detail = "User Cannot Be Found"
            };
            if (!BCrypt.Net.BCrypt.Verify(login.Password, user.PassHash))
            return new LoginResult
            {
                isSuccess = false,
                Detail = "Password Incorrect"
            };


            return new LoginResult
            {
                isSuccess = true,
                Detail = $"Welcome Back {user.Full_Name}"
            };
        }
        //public async Task<GetUserDTO?> UpdateAsync(int id, AddUserDTO userDto)
        //{
        //    var existing = await _userRepository.GetByIdAsync(id);
        //    if (existing == null) return null;

        //    existing.Full_Name = userDto.Full_Name!;
        //    existing.Email = userDto.Email;
        //    existing.PassHash = userDto.PassHash;
        //    existing.Phone_Number = userDto.Phone_Number;
        //    existing.Gender = userDto.Gender;
        //    existing.Birth_Date = userDto.Birth_Date;
        //    existing.Address = userDto.Address;
        //    existing.UserGroup_ID = userDto.UserGroup_ID;
        //    existing.LastUpdatedAt = DateTime.UtcNow;
        //    existing.LastUpdatedBy = userDto.LastUpdatedBy;

        //    await _userRepository.UpdateAsync(existing);

        //    return new GetUserDTO
        //    {
        //        User_ID = existing.User_ID,
        //        Full_Name = existing.Full_Name,
        //        Email = existing.Email,
        //        Phone_Number = existing.Phone_Number,
        //        Gender = existing.Gender,
        //        Birth_Date = existing.Birth_Date,
        //        Address = existing.Address,
        //        UserGroup_ID = existing.UserGroup_ID,
        //        CreatedAt = existing.CreatedAt,
        //        LastUpdatedAt = existing.LastUpdatedAt,
        //        CreatedBy = existing.CreatedBy,
        //        LastUpdatedBy = existing.LastUpdatedBy
        //    };
        //}

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _userRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _userRepository.DeleteAsync(existing);
            return true;
        }
    }
}