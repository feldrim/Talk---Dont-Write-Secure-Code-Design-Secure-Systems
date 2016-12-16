﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SecureFrameworkDemo.Framework.PropertyAccessAuthorization;

namespace SecureFrameworkDemo.Models {

	public class ApplicationUser : IdentityUser {

		public virtual ICollection<Permission> Permissions { get; set; }

		public virtual ICollection<PermissionEnum> PermissionEnums {
			get {
				return Permissions.Select(x => x.Id).Cast<PermissionEnum>().ToList();
			}
		}

		[MaskedValue(PermissionEnum.ViewSSN)]
		public string SSN { get; set; }

		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager) {
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			
			// Add custom user claims here
			return userIdentity;
		}

		public ApplicationUser() {
			Permissions = new List<Permission>();
		}

		public bool HasPermission(PermissionEnum perm) {
			return Permissions.Any(p => p.Id == (int)perm);
		}
	}
}