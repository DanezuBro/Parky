using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Route("api/Users")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpPost("Authenticate")]
        [AllowAnonymous]
        public IActionResult Authenticate([FromBody] AuthenticationModel model)
        {
            var user = _userRepo.Authenticate(model.Username,model.Password);
            if(user==null)
            {
                return BadRequest(new { message="Username or password is incorrect."});
            }
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register([FromBody] AuthenticationModel model)
        {
            bool ifUserNameUnique = _userRepo.isUniqueUser(model.Username);
            if(!ifUserNameUnique)
            {
                return BadRequest(new { message = "Username already exists."});
            }
             
            var user = _userRepo.Register(model.Username,model.Password);

            if(user==null)
            {
                return BadRequest(new { message = "Error while registering." });
            }
            return Ok();
        }

    }
}
