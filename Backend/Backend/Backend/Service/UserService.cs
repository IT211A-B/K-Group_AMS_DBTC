using Microsoft.AspNetCore.Mvc;
using Backend.Backend.DTO;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Model;
using Backend.Backend.Helper;
using PosStatus = Backend.Backend.Helper.Enum.PosEnum.PosStatus;

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

        public async Task<UserResponse> AddAsync(AddUserDTO userDto)
        {
            // This will check if the email exist
            User? DBEmails = await _userRepository.GetByEmailOrUsernameAsync(userDto.Email);
            if (DBEmails != null)
            {
                return new UserResponse
                {
                    Status_code = 422, //422 Unprocessable Content: Understands the request but cannot process ("Email is already used")
                    Data = null
                };
            }
            // Password Hashing shesh
            string Pass_Hash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            //Add Role According To Email
            PosStatus? role = AddRole.AddRoleAccordingToEmail(userDto.Email).role;
            int statcode_roleCheck = AddRole.AddRoleAccordingToEmail(userDto.Email).status_code;
            //If email does not match, return with 404 as not found and 403 as Access Denied
            if (statcode_roleCheck == 404)
            {
                return new UserResponse
                { 
                    Status_code = 403, //403 access_denied: You are not authorized to use the specific service with that account.
                    Data = null
                };
            }
            // Get Year
            int year = DateTime.Now.Year;
            // Get the next student number used by sequence that we made early in migration and such
            long id = await _userRepository.GetNextStudentNumberAsync();
            // Add Document Series
            string DocSer = $"{role}-{year}-{id}";

            var user = new User
            {
                DocumentSeries = DocSer,
                Full_Name = userDto.Full_Name,
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

            return new UserResponse
            {
                Status_code = 200,
                Data = user
            };
        }

        public async Task<LoginResult> LoginAsync(LoginUserDto login)
        {
            var user = await _userRepository.GetByEmailOrUsernameAsync(login.Email);

            // User Validation
            if (user == null) return new LoginResult
            {
                isSuccess = false,
                Detail = "User Cannot Be Found"
            };
            // Password validation
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