using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.BL.Response;
using Project.BL.RoleRepository;
using Project.DAL.Dtos;
using System.Data;
using System.Xml.Linq;

namespace Project.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleController(IMapper mapper
                             , IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        #region user
        [HttpGet("GetAllUsers")]
        public List<IdentityUser> GetAllUsers() => _roleRepository.GetAllUsers();

        [HttpPost("CreateUser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Super")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IdentityResult> CreateUser(CreateUserDto user) => await _roleRepository.AddUser(_mapper.Map<IdentityUser>(user));

        [HttpPut("UpdateUser")]
        public async Task<IdentityResult> UpdateUser(UpdateUserDto user) => await _roleRepository.UpdateUser(_mapper.Map<IdentityUser>(user));
        
        [HttpDelete("DeleteUser")]
        public async Task<IdentityResult> DeleteUser(string id) => await _roleRepository.DeleteUser(id);
        #endregion

        #region Role
        [HttpPost("CreateRole")]
        public async Task<string> CreateRole(string name) => await _roleRepository.Create(name);
        [HttpDelete("DeleteRole")]
        public async Task<string> DeleteRole(string name) => await _roleRepository.Delete(name);
        [HttpPost("AddUserToRole")]
        public async Task<string> AddUserToRole(string email, string name) => await _roleRepository.AddUserToRole(email, name);
        [HttpPost("RemoveUserFromRole")]
        public async Task<string> RemoveUserFromRole(string email, string name) => await _roleRepository.RemoveUserFromRole(email, name);
        #endregion

        #region Login
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<Response<TokenUser>> Login(LoginUser user) => await _roleRepository.Login(user);
        #endregion

    }
}
