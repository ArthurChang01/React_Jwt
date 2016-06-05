using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace React_Jwt.Models
{
    public class Audience : IdentityUser
    {
        [Required]
        public DateTime JoinDate { get; set; }

        public bool Active { get; set; }

        public ClaimsIdentity GenerateUserIdentityAsync()
        {
            var identity = new ClaimsIdentity("JWT");

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, this.Id));
            identity.AddClaim(new Claim(ClaimTypes.Name, this.UserName));
            identity.AddClaim(new Claim("sub", this.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, "User"));

            return identity;
        }

    }
}