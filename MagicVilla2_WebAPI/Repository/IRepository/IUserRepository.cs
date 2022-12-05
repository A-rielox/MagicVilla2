using MagicVilla2_WebAPI.Models;
using MagicVilla2_WebAPI.Models.Dto;

namespace MagicVilla2_WebAPI.Repository.IRepository;

public interface IUserRepository
{
    bool IsUniqueUser(string username);
    Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
    Task<LocalUser> Register(RegisterationRequestDTO registerationRequestDTO);
}
