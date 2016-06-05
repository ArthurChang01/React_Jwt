using Microsoft.Owin.Security.OAuth;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using React_Jwt.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace React_Jwt.Infra
{
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {
        AuthContext ctx = new AuthContext();
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var allowedOrigin = "*";

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            Audience user = null;

            try
            {
                using (AuthContext ctx = new AuthContext())
                using (ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<Audience>(ctx)))
                {
                    user = await userManager.FindAsync(context.UserName, context.Password);
                }

            }
            catch (Exception ex)
            {
                throw;
            }

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            if (!user.EmailConfirmed)
            {
                context.SetError("invalid_grant", "User did not confirm email.");
                return;
            }

            ClaimsIdentity oAuthIdentity = user.GenerateUserIdentityAsync();

            var ticket = new AuthenticationTicket(oAuthIdentity, null);

            context.Validated(ticket);
        }
    }
}