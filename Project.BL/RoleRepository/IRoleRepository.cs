﻿using Microsoft.AspNetCore.Identity;
using Project.BL.Response;
using Project.DAL.Dtos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Project.BL.RoleRepository
{
    public interface IRoleRepository
    {
        #region Roles
        Task<List<string>> GetRoles();
        Task<string> Create(string name);
        Task<string> Delete(string name);
        #endregion

        #region User
        List<IdentityUser> GetAllUsers();
        Task<IdentityResult> AddUser(IdentityUser user);
        Task<IdentityResult> UpdateUser(IdentityUser user);
        Task<IdentityResult> DeleteUser(string id);
        #endregion


        Task<string> AddUserToRole(string email, string roleName);
        Task<string> RemoveUserFromRole(string email, string roleName);

        Task<IEnumerable<Claim>> GetAllValidClaims(IdentityUser user);
        Task<string> VerifyAndGenerateToken(LoginUser user);
        Task<Response<TokenUser>> Login(LoginUser user);
    }
}
