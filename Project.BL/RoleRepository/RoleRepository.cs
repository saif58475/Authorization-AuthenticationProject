using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Project.BL.Response;
using Project.DAL.AppDBContext;
using Project.DAL.Dtos;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Project.BL.RoleRepository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ProjectDBContext _context;
        private readonly ILogger<RoleRepository> _logger;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _rolemanager;
        private readonly UserManager<IdentityUser> _usermanager;

        public RoleRepository( ProjectDBContext context
                             , ILogger<RoleRepository> logger
                             , IConfiguration config
                             , IMapper mapper
                             , RoleManager<IdentityRole> rolemanager
                             , UserManager<IdentityUser> usermanager)
        {
            _context = context;
            _logger = logger;
            _config = config;
            _mapper = mapper;
            _rolemanager = rolemanager;
            _usermanager = usermanager;
        }

        #region User
        public List<IdentityUser> GetAllUsers()
        {
            return _usermanager.Users.ToList();
        }
        public async Task<IdentityResult> AddUser(IdentityUser user)
        {
            return await _usermanager.CreateAsync(user);
        }
        public async Task<IdentityResult> UpdateUser(IdentityUser user)
        {
            return await _usermanager.UpdateAsync(user);
        }
        public async Task<IdentityResult> DeleteUser(string id)
        {
            return await _usermanager.DeleteAsync(await _usermanager.FindByIdAsync(id));
        }
        #endregion

        #region Role
        public async Task<string> Create(string name)
        {
            if (!await _rolemanager.RoleExistsAsync(name)) await _rolemanager.CreateAsync(new IdentityRole(name));
            return (await _rolemanager.RoleExistsAsync(name)) ? "Added Successfully" : "Failed to Create This Role";
        }

        public async Task<string> Delete(string name)
        {
            if (await _rolemanager.RoleExistsAsync(name)) await _rolemanager.DeleteAsync(new IdentityRole(name));
            return (!await _rolemanager.RoleExistsAsync(name)) ? "Role Removed Successfully" : "Failed To Remove The Role";
        }

        public Task<List<string>> GetRoles()
        {
            throw new NotImplementedException();
        }


        //public async Task<List<string>> GetRoles()
        //{
        //    return await _rolemanager.Roles.ToList();
        //}
        #endregion

        #region User With Role
        public async Task<string> AddUserToRole(string email, string roleName)
        {
            IdentityUser user = await _usermanager.FindByEmailAsync(email);
            if (user != null)
            {
                if (await _rolemanager.RoleExistsAsync(roleName) != null)
                    await _usermanager.AddToRoleAsync(user, roleName);
                return "User Added To Role Successfully";
            }
            else
                return "Failed To Add User To Role";
        }
        public async Task<string> RemoveUserFromRole(string email, string roleName)
        {
            IdentityUser user = await _usermanager.FindByEmailAsync(email);
            if (user != null)
            {
                if (await _rolemanager.RoleExistsAsync(roleName) != null)
                    await _usermanager.RemoveFromRoleAsync(user, roleName);
                return "User Removed From Role Successfully";
            }
            else
                return "Failed To Remove User from Role";
        }
        #endregion

        //This will give the token to be authorized all over the application
        public async Task<Response<TokenUser>> Login(LoginUser user)
        {
            return new Response<TokenUser>() { Message = "Success", StatusCode = StatusCodes.Status200OK, Data = new TokenUser { Token = await this.GenerateTokenString(user) }, Success = true };
        }
        //This is the method that generates the token 
        public async Task<string> GenerateTokenString(LoginUser user)
        {
            SymmetricSecurityKey securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Jwt:Key").Value));
            SigningCredentials signingCred = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha512);
            IEnumerable<Claim> claims = await this.GetAllValidClaims(await _usermanager.FindByEmailAsync(user.Email));
            JwtSecurityToken securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                issuer: _config.GetSection("Jwt:Issuer").Value,
                audience: _config.GetSection("Jwt:Issuer").Value,
                signingCredentials: signingCred);
            string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return tokenString;
        }
        //This will get all the roles and claims related to the user 
        public async Task<IEnumerable<Claim>> GetAllValidClaims(IdentityUser user)
        {
            var _options = new IdentityOptions();
            var claims = new List<Claim>
            {
                new Claim("Guid", user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            //Getting the claims that we have assigned to the user 
            var userClaims = await _usermanager.GetClaimsAsync(user);
            claims.AddRange(userClaims);
            //Getting the user role and add it to the claims
            var userRoles = await _usermanager.GetRolesAsync(user);

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("Role", userRole));
                var role = await _rolemanager.FindByNameAsync(userRole);
                if(role != null)
                {
                    var roleClaims = await _rolemanager.GetClaimsAsync(role);
                    foreach(var roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }
            IEnumerable<Claim> ClaimsOfUser = new List<Claim>(claims);
            return ClaimsOfUser;
        }


    }
}
