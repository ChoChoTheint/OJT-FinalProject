using finalproject.DTO;
using finalproject.models;
using finalproject.Repositories;
using finalproject.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Security.Claims;

namespace finalproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserController> _logger;

        public UserController(IRepositoryWrapper repositoryWrapper,IConfiguration configuration, ILogger<UserController> logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _configuration = configuration;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("CreateUser", Name = "CreateUser")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(CreateUserResponseDTOExample), Description = "Success case")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(CreateUserResponseDTOExample), Description = "Error case")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CreateUserResponseDTOExample))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(CreateUserErrorResponseDTOExample))]
        public async Task<ActionResult<CreateUserResponseDTO>> CreateUser(UserRequestDTO user)
        {
            try
            {
                _logger?.LogInformation("CreateUser method called");

                var oldUser = (await _repositoryWrapper.User.GetAll<User>("SELECT * FROM User WHERE name = @UserName", new { UserName = user.UserName })).FirstOrDefault();
                if (oldUser != null)
                {
                    var createUserResponse = new CreateUserResponseDTO
                    {
                        status = "fail",
                        message = "User already exists"
                    };

                    string userObjString = JsonConvert.SerializeObject(user);
                    await _repositoryWrapper.EventLog.InsertEventLog("UserController", "CreateUser", userObjString);

                    return Ok(createUserResponse);
                }
                var salt = finalproject.Util.GlobalFunction.GenerateSalt();
                var hashedPassword = finalproject.Util.GlobalFunction.ComputeHash(salt, user.Password);

                var newUser = new User
                {
                    name = user.UserName,
                    password = hashedPassword,
                    salt = salt,
                    login_fail_count = 0,
                    is_lock = false,
                    userlevel_id = user.UserLevelID
                };

                // Save new user to the database
                _repositoryWrapper.User.Create(newUser);
                await _repositoryWrapper.User.Save();

                var successResponse = new CreateUserResponseDTO
                {
                    status = "success",
                    message = "User created successfully"
                };

                string newUserObjString = JsonConvert.SerializeObject(user);
                await _repositoryWrapper.EventLog.InsertEventLog("UserController", "CreateUser", newUserObjString);

                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in CreateUser method");
                await _repositoryWrapper.EventLog.ErrorEventLog("UserController", "CreateUser", "Failed to CreateUser", ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("Login", Name = "Login")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(LoginResponseDTO), Description = "Success case")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(LoginResponseDTO), Description = "Error case")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(LoginResponseDTOExample))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(LoginErrorResponseDTOExample))]
        public async Task<ActionResult<LoginResponseDTO>> Login(LoginRequestDTO user)
        {
            try
            {
                 _logger?.LogInformation("Login method called success");
                LoginResponseDTO loginResponseDTO = new LoginResponseDTO();
                var oldUser = await _repositoryWrapper.User.GetName(user.UserName);
                if (oldUser == null)
                {
                    loginResponseDTO = new LoginResponseDTO
                    {
                        status = "fail",
                        message = "Username or Password is incorrect"
                    };
                }
                else
                {
                    if (oldUser.is_lock)
                    {
                        int maxLoginFailCount = _configuration.GetValue<int>("MaxLoginFailCount");
                            oldUser.login_fail_count += 1;

                            if (oldUser.login_fail_count >= maxLoginFailCount)
                            {
                                oldUser.is_lock = true;
                            }

                            await _repositoryWrapper.User.Save();

                        loginResponseDTO = new LoginResponseDTO
                        {
                            status = "fail",
                            message = "Account is locked",
                            maxLoginFailCount = oldUser.login_fail_count 
                        };
                    }
                    else
                    {
                        var hash = finalproject.Util.GlobalFunction.ComputeHash(oldUser.salt, user.Password);
                        if (hash == oldUser.password)
                        {
                            Claim[] claims = GlobalFunction.CreateClaim(oldUser.user_id, new DateTimeOffset(DateTime.UtcNow).ToUniversalTime().ToUnixTimeSeconds().ToString());
                            var token = GlobalFunction.CreateJWTToken(claims);
                            loginResponseDTO = new LoginResponseDTO
                            {
                                status = "success",
                                message = "Login success",
                                accessToken = token
                            };
                        }
                        else
                        {
                            int maxLoginFailCount = _configuration.GetValue<int>("MaxLoginFailCount");
                            oldUser.login_fail_count += 1;

                            if (oldUser.login_fail_count >= maxLoginFailCount)
                            {
                                oldUser.is_lock = true;
                            }

                            await _repositoryWrapper.User.Save();

                            loginResponseDTO = new LoginResponseDTO
                            {
                                status = "fail",
                                message = "Username or Password is incorrect",
                                maxLoginFailCount = oldUser.login_fail_count 
                            };
                        }
                    }
                }

                string userObjString = JsonConvert.SerializeObject(user);
                await _repositoryWrapper.EventLog.InsertEventLog("UserController", "Login", userObjString);
                return Ok(loginResponseDTO);
                
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in Login method");
                await _repositoryWrapper.EventLog.ErrorEventLog("UserController", "Login", "Failed to login", ex.Message);
                return StatusCode(500, "Internal server error");
            }
            
        }

        [Authorize] 
        [HttpGet("GetUser", Name = "GetUser")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(GetUserResponseDTOExample), Description = "Success case")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetUserResponseDTOExample))]
        public async Task<ActionResult<List<GetUserResponseDTO>>> GetUser()
        {
                int userID = int.Parse(HttpContext.User?.FindFirst(x => x.Type == "sid")?.Value ?? "0");
                string ecbKey = _configuration.GetValue<string>("Encyption:ECBSecretKey") ?? "";
                GetUserResponseDTO? user=null;
                try
                {
                    _logger?.LogInformation("GetUser method called success");
                    user=await _repositoryWrapper.User.GetUser(userID,ecbKey);
                }
                catch(Exception ex)
                {
                   
                    _logger?.LogError(ex, "Error in getuser method");
                    return StatusCode(500, "Internal server error");
                }
                return Ok(user);

        }

        [Authorize]
        [HttpPut("UpdateUser", Name = "UpdateUser")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(CreateUserResponseDTO), Description = "Success case")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UpdateUserResponseDTOExample))]

        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(CreateUserResponseDTO), Description = "Error case")]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(UpdateUserErrorResponseDTOExample))]
        public async Task<ActionResult<CreateUserResponseDTO>> UpdateUser(UpdateUserRequestDTO user)
        {
            CreateUserResponseDTO createUserResponse = new CreateUserResponseDTO();
            try
            {
                _logger?.LogInformation("UpdateUser method called success");
                var encryptedKey = user.UserID;
                string ecbKey = _configuration.GetValue<string>("Encyption:ECBSecretKey") ?? "";
                int userID = int.Parse(Encryption.AES_Decrypt_ECB_128(encryptedKey, ecbKey));
               
                var existingUser = await _repositoryWrapper.User.FindByID(userID);
                if (existingUser == null)
                {
                    createUserResponse = new CreateUserResponseDTO
                    {
                        status = "fail",
                        message = "User not found"
                    };
                    return BadRequest(createUserResponse);
                }else
                {
                    existingUser.name = user.Name;
                    
                    var salt = finalproject.Util.GlobalFunction.GenerateSalt();
                    var hashedPassword = finalproject.Util.GlobalFunction.ComputeHash(salt, user.Password);
                    existingUser.password = hashedPassword;
                    existingUser.salt = salt;
        
                    await _repositoryWrapper.User.Save();
                    createUserResponse = new CreateUserResponseDTO
                    {
                        status = "Success",
                        message = "User Name Updated successfully"
                    };
                    string userObjString = JsonConvert.SerializeObject(user);
                    await _repositoryWrapper.EventLog.UpdateEventLog("UserController", "UpdateUser", userObjString);
                    return createUserResponse;
                }
                
            }
            catch (Exception ex)
            {   
                _logger?.LogError(ex, "Error in UpdateUser method");
                await _repositoryWrapper.EventLog.ErrorEventLog("UserController", "UpdateUser", "Failed to update user", ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize]
        [HttpGet("GetAllStudents", Name = "GetAllStudents")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllStudents()
        {
            IEnumerable<User> allStudent = null;
            try
            {
              _logger?.LogInformation("GetAllStudents method called success");
              allStudent = await _repositoryWrapper.User.GetAllStudent();
              
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting students");
                return StatusCode(500, "Internal server error");
            }
            return Ok(allStudent);
        }
    
    }
}



