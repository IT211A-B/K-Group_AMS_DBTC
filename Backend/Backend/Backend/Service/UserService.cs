using Backend.Backend.Configuration;
using Backend.Backend.DTOs;
using Backend.Backend.Helper;
using Backend.Backend.Interface.ConfigureInterface;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PosStatus = Backend.Backend.Helper.Enum.PosEnum.PosStatus;

namespace Backend.Backend.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        /// <summary>
        /// Service used to generate claims for the authenticated user.
        /// </summary>
        private readonly IClaimService _claimService;

        /// <summary>
        /// Service used to generate JWT tokens.
        /// </summary>
        private readonly IJwtService _jwtService;

        public UserService(IUserRepository userRepository, UserManager<User> userManager, IClaimService claimService, IJwtService kwtService)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _claimService = claimService;
            _jwtService = kwtService;  
        }

        public async Task<ResponseDTO<IEnumerable<GetUserDTO>>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            if (users is null || !users.Any())
                return new ResponseDTO<IEnumerable<GetUserDTO>>
                {
                    Status_code= 404,
                    Data = Enumerable.Empty<GetUserDTO>()
                };

            var data = users.Select(u => new GetUserDTO
            {
                DocumentSeries = u.DocumentSeries,
                Full_Name = u.Full_Name,
                Email = u.Email!,
                Phone_Number = u.Phone_Number,
                Sex = u.Sex,
                Birth_Date = u.Birth_Date,
                Address = u.Address,
            });

            return new ResponseDTO<IEnumerable<GetUserDTO>>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetUserDTO>> GetByIdAsync(int id)
        {
            var u = await _userRepository.GetByIdAsync(id);
            if (u == null)
                return new ResponseDTO<GetUserDTO>
                {
                    Status_code = 404,
                    Data = null
                };

            var data = new GetUserDTO
            {
                DocumentSeries = u.DocumentSeries,
                Full_Name = u.Full_Name,
                Email = u.Email!,
                Phone_Number = u.Phone_Number,
                Sex = u.Sex,
                Birth_Date = u.Birth_Date,
                Address = u.Address,
            };

            return new ResponseDTO<GetUserDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<RegisterDTO> AddAsync(AddUserDTO userDto)
        {
            // This will check if the email exist
            User? DBEmails = await _userManager.FindByEmailAsync(userDto.Email);
            if (DBEmails != null)
            {
                return new RegisterDTO
                {
                    Token = null,
                    StatusCode = 422, //422 Unprocessable Content: Understands the request but cannot process ("Email is already used")
                    Data = null
                };
            }
            //Add Role According To Email
            PosStatus? role = AddRole.AddRoleAccordingToEmail(userDto.Email).role;
            int statcode_roleCheck = AddRole.AddRoleAccordingToEmail(userDto.Email).status_code;
            //If email does not match, return with 404 as not found and 403 as Access Denied
            if (statcode_roleCheck == 404)
            {
                return new RegisterDTO
                {
                    Token = null,
                    StatusCode = 403, //403 rolepermission_denied: You are not authorized to use the specific service with that account.
                    Data = null
                };
            } 
            // Get Year
            int year = DateTime.Now.Year;
            // Get the next student number used by sequence that we made early in migration and such
            long id = await _userRepository.GetNextUserNumberAsync();
            // Add Document Series
            string DocSer = $"{role}-{year}-{id}";


            var user = new User
            {
                DocumentSeries = DocSer,
                Full_Name = userDto.Full_Name,
                Email = userDto.Email,
                UserName = userDto.Email,
                Phone_Number = userDto.Phone_Number,
                Sex = userDto.Sex,
                Birth_Date = userDto.Birth_Date,
                Address = userDto.Address,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                CreatedBy = "Admin",
                LastUpdatedBy = "Admin"
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);

            // Add Role Base on Email

            // Extract Role from Document Series
            var extractedRole = ExtractDocuSer.ExtractDataFromDocumentSeries(DocSer);

            // Convert it into string
            var convertedRole = EnumRoletoStringRole.ConvertEnumRoletoStringRole(extractedRole.ExtractedPosition);

            // Give Role
            await _userManager.AddToRoleAsync(user, convertedRole);

            if (!result.Succeeded)
            {
                return new RegisterDTO
                {
                    Token = null,
                    StatusCode = 500,
                    Data = null,
                    Detail = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }
            var userDisplay = await _userRepository.GetByEmailAsync(userDto.Email);

            GetUserDTO? show = userDisplay != null ? 
                new GetUserDTO
            {
                DocumentSeries = userDisplay.DocumentSeries,
                Full_Name = userDisplay.Full_Name,
                Email = userDisplay.Email!,
                Phone_Number = userDisplay.Phone_Number,
                Sex = userDisplay.Sex,
                Birth_Date = userDisplay.Birth_Date,
                Address = userDisplay.Address,
            }
            : null;


            // collect user's identity
            var claims = await _claimService.GetClaimsAsync(user);

            // generate token
            var token = _jwtService.GenerateToken(claims);

            return new RegisterDTO
            {
                Token = token,
                StatusCode = 200,
                Data = show
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
        //    existing.Sex = userDto.Sex;
        //    existing.Birth_Date = userDto.Birth_Date;
        //    existing.Address = userDto.Address;
        //    existing.UserGroup_ID = userDto.UserGroup_ID;
        //    existing.LastUpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, manilaTimeZone);
        //    existing.LastUpdatedBy = userDto.LastUpdatedBy;

        //    await _userRepository.UpdateAsync(existing);

        //    return new GetUserDTO
        //    {
        //        User_ID = existing.User_ID,
        //        Full_Name = existing.Full_Name,
        //        Email = existing.Email,
        //        Phone_Number = existing.Phone_Number,
        //        Sex = existing.Sex,
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