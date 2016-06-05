using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using React_Jwt.Models;
using System.Threading.Tasks;

namespace React_Jwt.Infra
{
    public class ApplicationUserManager : UserManager<Audience>
    {
        public ApplicationUserManager(IUserStore<Audience> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var appDbContext = context.Get<AuthContext>();
            var appUserManager = new ApplicationUserManager(new UserStore<Audience>(appDbContext));

            // Configure validation logic for usernames
            appUserManager.UserValidator = new UserValidator<Audience>(appUserManager)
            {
                AllowOnlyAlphanumericUserNames = true,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            appUserManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = false,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            appUserManager.EmailService = new EmailService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                appUserManager.UserTokenProvider = new DataProtectorTokenProvider<Audience>(dataProtectionProvider.Create("ASP.NET Identity"))
                {
                    //Code for email confirmation and reset password life time
                    TokenLifespan = TimeSpan.FromHours(6)
                };
            }

            return appUserManager;
        }

        internal Task<IdentityResult> ChangePasswordAsync(string v, object oldPwd, object newPwd)
        {
            throw new NotImplementedException();
        }
    }
}