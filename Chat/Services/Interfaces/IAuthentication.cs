using ChatAPI.Dtos;
using ChatAPI.Types;
using System.Threading.Tasks;

namespace ChatAPI.Services.Interfaces
{
    public interface IAuthentication
    {
        Task<RegistrationMassages> RegisterAsync(RegisterDto model);
        Task<string> LoginAsync(LoginDto model);
    }
}
