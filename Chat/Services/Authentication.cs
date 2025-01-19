using ChatAPI.Dtos;
using ChatAPI.Helpers;
using ChatAPI.Helpers.Interfaces;
using ChatAPI.Models;
using ChatAPI.Repos.Interfaces;
using ChatAPI.Services.Interfaces;
using ChatAPI.Types;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace ChatAPI.Services
{
    public class Authentication : IAuthentication
    {
        public IUserManager _userManager { get; }
        public IPasswordManager _passwordManager { get; }
        public JWTHelper _jWTHelper { get; }

        public Authentication(IUserManager userManager,
            IPasswordManager passwordManager, JWTHelper jWTHelper )
        {
            _userManager = userManager;
            _passwordManager = passwordManager;
            _jWTHelper = jWTHelper;
        }

        public async Task<string> LoginAsync(LoginDto model)
        {
            try
            {
                var hashedPassword = await _passwordManager.Hash(model.Password);

                var user = await _userManager.GetUserAsync(model.UserName, hashedPassword);
                if (user == null)
                    return null;


                return await _jWTHelper.GenerateAcessToken(await _jWTHelper.GenerateUserClaims(user));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<RegistrationMassages> RegisterAsync(RegisterDto model)
        {
            try
            {
                if (await IsRegisterModelValidAsync(model) == RegistrationMassages.UserNameNotExists)
                {
                    var user = new User
                    {
                        Id = Guid.NewGuid(),
                        Password = await _passwordManager.Hash(model.Password),
                        Name = model.UserName
                    };

                     
                    await _userManager.CreateUserAsync(user);
                    
                    return RegistrationMassages.Succeeded;
                }
                return RegistrationMassages.UserNameAlreadyExists;
            }
            catch (Exception ex)
            {
                return RegistrationMassages.Failed;
            }
        }

        private async Task<RegistrationMassages> IsRegisterModelValidAsync(RegisterDto model)
        {
            try
            {
                if (await _userManager.UserNameExistsAsync(model.UserName))
                    return RegistrationMassages.UserNameAlreadyExists;
                return RegistrationMassages.UserNameNotExists;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
