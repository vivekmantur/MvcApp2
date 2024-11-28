using Azure.Core;
using Microsoft.AspNetCore.Identity;
using System.Collections.Concurrent;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModel;

namespace WebApplication1.Repository
{
    public class AccountRepository
    {
        private readonly UserManager<UserRegistration> userManager;
        private readonly SignInManager<UserRegistration> signInManager;
        public EmailService emailService;
        public WebApplication1Context context;
        private static readonly ConcurrentDictionary<string, (string Otp, DateTime Expiration)> _otps = new();
        public AccountRepository(UserManager<UserRegistration> userManager,
            SignInManager<UserRegistration> signInManager, EmailService emailService, WebApplication1Context context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailService = emailService;
            this.context = context;
        }

        public async Task<IdentityResult> RegisterAsync(UserViewModel request)
        {
            var user = new UserRegistration
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
            };

            var result = await userManager.CreateAsync(user, request.Password);
            var res = await userManager.AddToRoleAsync(user, "User");
            return res;
        }

        //public async Task<IdentityResult> Login(UserLogin model)
        //{
            
        //}

       
    }
}
