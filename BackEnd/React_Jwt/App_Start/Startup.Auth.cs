using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Configuration;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using React_Jwt.Infra;
using React_Jwt.Models;

namespace React_Jwt
{
    public partial class Startup
    {
        public void ConfigureOAuthTokenConsumption(IAppBuilder app)
        {
            app.CreatePerOwinContext(()=>new AuthContext());
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            var issuer = ConfigurationManager.AppSettings["as:Domain"];
            var audience = ConfigurationManager.AppSettings["as:AudienceId"];
            var secret = ConfigurationManager.AppSettings["as:AudienceSecret"];
            byte[] arSecrect = TextEncodings.Base64Url.Decode(secret);

            #region Jwt驗證組態

            JwtBearerAuthenticationOptions options = new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                AllowedAudiences = new[] { audience },
                IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[] { new SymmetricKeyIssuerSecurityTokenProvider(issuer, arSecrect) }
            };

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(options);

            #endregion Jwt驗證組態
        }

        public void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            var issuer = ConfigurationManager.AppSettings["as:Domain"];

            #region Server註冊

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                //For Dev enviroment only (on production should be AllowInsecureHttp = false)
#if DEBUG
                AllowInsecureHttp = true,
#endif
                TokenEndpointPath = new PathString("/oauth2/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                Provider = new CustomOAuthProvider(),
                AccessTokenFormat = new CustomJwtFormat(issuer)
            };

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);

            #endregion Server註冊
        }
    }
}
