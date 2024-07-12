using final_task_api.models;
using final_task_api.packages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace final_task_api.Controllers
{
    [Route("api/")]
    [ApiController]
    [EnableCors("MyPolicy")]

    public class Controller : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly Package _package;

        public Controller(Package Package, IConfiguration configuration)
        {
            _package = Package;
            _configuration = configuration;

        }

        [HttpPost("CreateAdmin")]
        public async Task<IActionResult> CreateAdmin([FromBody] RegisterModel model)
        {
            try
            {
                await _package.CreateAdmin(model);
                return Ok(new { message = "user created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }


        [HttpPost("LoginAdmin")]
        public async Task<IActionResult> LoginUser([FromBody] LoginModel model)
        {
            try
            {
                LoginModel user = await _package.LoginAdmin(model);
                if (user == null)
                {
                    return BadRequest(new { meesage = "wrong credentials" });
                }
                else
                {
                    var authClaims = new List<Claim>
                    {
                        new Claim("JWTID",Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, "admin")
                    };
                    var token = GenerateNewJsonWebToken(authClaims);
                    return Ok(new { message = user, token });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating admin: {ex.Message}");
            }
        }

        private string GenerateNewJsonWebToken(List<Claim> claims)
        {
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.MaxValue,
                claims: claims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
            );
            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
            return token;
        }

        [HttpPost("AddQuestion")]
        public IActionResult AddQustion([FromBody] List<QuestionModel> model)
        {
            try
            {
                 _package.Addquestions(model);
                return Ok(new { message = "question created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }

        [HttpGet("GetAllquestions")]
        public IActionResult GetQuestions()
        {
            try
            {
                var questions = _package.GetAllQuestionsAndAnswers();
                return Ok(questions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving questions: {ex.Message}");
            }
        }

        [HttpPost("AddAnswers")]
        public IActionResult AddAnswers([FromBody] List<AnswerModel> model)
        {
            try
            {
                _package.AddAnswers(model);
                return Ok(new { message = "answer created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }

        [HttpPut("UpdateQuestions/{id}")]
        public IActionResult UpdateQuestions([FromBody] QuestionModel model,int id)
        {
            try
            {
                _package.UpdateQuestion(id,model);
                return Ok(new { message = "answer created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }

        [HttpGet("GetQuestions/{id}")]
        public IActionResult GetAnswers(int id)
        {
            try
            {
                var questions = _package.GetAnswers(id);
                return Ok(questions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving questions: {ex.Message}");
            }
        }


    }
}
