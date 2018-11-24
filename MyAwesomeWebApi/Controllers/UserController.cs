using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyAwesomeWebApi.Helpers;
using MyAwesomeWebApi.Models.Identity;
using MyAwesomeWebApi.Models.Requests;
using MyAwesomeWebApi.Models.Responses;

namespace MyAwesomeWebApi.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // GET api/user/userdata
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult> UserData()
        {
            var user = await _userManager.GetUserAsync(User);
            var userData = new UserDataResponse
            {
                Name = user.UserName,
                LastName = user.LastName,
                City = user.City,
                Email = user.Email
            };
            return Ok(userData);
        }

        // POST api/user/register
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]RegisterEntity model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { Name = model.Name, LastName = model.LastName, City = model.City, UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    var token = AuthenticationHelper.GenerateJwtToken(model.Email, user, _configuration);

                    var rootData = new SignUpResponse(token, user.UserName, user.Email);
                    return Created("api/v1/authentication/register", rootData);
                }
                return Ok(string.Join(",", result.Errors?.Select(error => error.Description)));
            }
            string errorMessage = string.Join(", ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            return BadRequest(errorMessage ?? "Bad Request");
        }


        // POST api/user/login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody]LoginEntity model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                if (result.Succeeded)
                {
                    var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                    var token = AuthenticationHelper.GenerateJwtToken(model.Email, appUser, _configuration);

                    var rootData = new LoginResponse(token, appUser.UserName, appUser.Email);
                    return Ok(rootData);
                }
                return StatusCode((int)HttpStatusCode.Unauthorized, "Bad Credentials");
            }
            string errorMessage = string.Join(", ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            return BadRequest(errorMessage ?? "Bad Request");
        }
    }
}
