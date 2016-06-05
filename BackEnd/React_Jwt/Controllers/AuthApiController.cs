using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using React_Jwt.Models;
using React_Jwt.Infra;
using React_Jwt.ViewModels;

namespace React_Jwt.Controllers
{
    public class AuthApiController : BaseApiController
    {
        /// <summary>
        /// Login
        /// </summary>
        /// <param name="userName">使用者帳戶</param>
        /// <param name="password">密碼</param>
        /// <returns>Token</returns>
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> GetLogin(string userName, string password)
        {
            var request = HttpContext.Current.Request;
            var tokenServiceUrl = request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath + "oauth2/token";
            using (var client = new HttpClient())
            {
                var requestParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", userName),
                new KeyValuePair<string, string>("password", password)
            };
                var requestParamsFormUrlEncoded = new FormUrlEncodedContent(requestParams);
                var tokenServiceResponse = await client.PostAsync(tokenServiceUrl, requestParamsFormUrlEncoded);
                var responseString = await tokenServiceResponse.Content.ReadAsStringAsync();
                var responseCode = tokenServiceResponse.StatusCode;
                var responseMsg = new HttpResponseMessage(responseCode)
                {
                    Content = new StringContent(responseString, Encoding.UTF8, "application/json")
                };
                return responseMsg;
            }
        }

        /// <summary>
        /// 依Id取得使用者資訊
        /// </summary>
        /// <param name="Id">使用者Id</param>
        /// <returns>使用者資訊</returns>
        [Authorize]
        [Route("user/{id:guid}",Name = "GetUserById")]
        [ResponseType(typeof(Audience))]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            Audience ad = null;

            try
            {
                ad = await this.AppUserManager.FindByIdAsync(Id);

                if (ad != null)
                {
                    return Ok(this.TheModelFactory.Create(ad));
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return NotFound();
        }

        /// <summary>
        /// 依使用者名稱取得使用者資訊
        /// </summary>
        /// <param name="username">使用者名稱</param>
        /// <returns>使用者資訊</returns>
        [Authorize]
        [Route("user/{username}")]
        [ResponseType(typeof(Audience))]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            Audience ad = null;

            try
            {
                ad = await this.AppUserManager.FindByNameAsync(username);

                if (ad != null)
                {
                    return Ok(this.TheModelFactory.Create(ad));
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return NotFound();
        }

        /// <summary>
        /// 創建使用者
        /// </summary>
        /// <param name="createUserModel">帳戶基本資訊</param>
        /// <returns>是否創建成功</returns>
        [AllowAnonymous]
        [Route("create")]
        [ResponseType(typeof(IHttpActionResult))]
        public async Task<IHttpActionResult> CreateUser(CreateUserVM createUserModel)
        {
            IdentityResult id = null;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new Audience()
            {
                UserName = createUserModel.Username,
                Email = createUserModel.Email,
                JoinDate = DateTime.Now.Date,
                PasswordHash = createUserModel.Password,
                EmailConfirmed = true
            };

            try
            {
                id = await AppUserManager.CreateAsync(user,user.PasswordHash);

                if (!id.Succeeded) return GetErrorResult(id);

                string code = await this.AppUserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                var callbackUrl = new Uri(Url.Link("confirmEmail", new { userId = user.Id, code = code }));

                await this.AppUserManager.SendEmailAsync(user.Id,
                                                        "Confirm your account",
                                                        "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
            }
            catch (Exception ex)
            {
                ex = ex.InnerException ?? ex;
                return InternalServerError(ex);
            }

            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id.ToString() }));

            return Created(locationHeader, TheModelFactory.Create(user));

        }

        [AllowAnonymous]
        [HttpGet]
        [Route("confirmEmail",Name = "confirmEmail")]
        public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id and Code are required");
                return BadRequest(ModelState);
            }

            IdentityResult result = await this.AppUserManager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
            {
                return Ok("已驗證");
            }
            else
            {
                return GetErrorResult(result);
            }
        }

        /// <summary>
        /// 修改帳戶密碼
        /// </summary>
        /// <param name="model">密碼資訊(舊,新)</param>
        /// <returns></returns>
        [Authorize]
        [Route("changePassword")]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string id = User.Identity.GetUserId();

            IdentityResult result = await this.AppUserManager.ChangePasswordAsync(User.Identity.GetUserId(),model.OldPwd, model.NewPwd);

            if (!result.Succeeded)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        /// <summary>
        /// 刪除使用者
        /// </summary>
        /// <param name="id">欲刪除帳戶的Id</param>
        /// <returns>是否刪除成功</returns>
        [Authorize]
        [Route("user")]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            var appUser = await this.AppUserManager.FindByIdAsync(id);

            if (appUser != null)
            {
                IdentityResult result = await this.AppUserManager.DeleteAsync(appUser);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                return Ok();

            }

            return NotFound();

        }

    }
}
