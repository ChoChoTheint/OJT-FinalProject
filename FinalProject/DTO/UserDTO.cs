using System.Runtime.ConstrainedExecution;
using finalproject.models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace finalproject.DTO
{
    public class UserRequestDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int UserLevelID{get;set;}
    }
    public class CreateUserResponseDTO
    {
        public string status { get; set; }
        public string message { get; set; }
    }
    public class LoginRequestDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class LoginResponseDTO
    {
        public string status { get; set; }
        public string message { get; set; }
        public string accessToken { get; set; }
        public int maxLoginFailCount{get; set;}
    }
    public class UnlockUserRequestDTO
    {
        public string UserID { get; set; }
        public string UserIDCBC { get; set; }
    }
    public class GetUserResponseDTO
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Password{get;set;}
        public string IsLock { get; set; }
        public string FailCount { get; set; }
        public string UserLevel {get;set;}
        public string Salt{get;set;}
    }
    public class UpdateUserRequestDTO
    {
        public string UserID{get;set;}
        public string Name { get; set; }
        public string Password { get; set; }
    }
    public class GetStudentsResponeDTO{
        public int UserID{get;set;}
        public string UserName { get; set; }
        public int UserLevelID { get; set; }
        public string UserLevelName { get; set; }
    }
   
    public class CreateUserResponseDTOExample : IMultipleExamplesProvider<CreateUserResponseDTO>
    {
        public IEnumerable<SwaggerExample<CreateUserResponseDTO>> GetExamples()
        {
            yield return SwaggerExample.Create(
                "Default",
                "Example when the UnlockUser operation is called with specific parameters",
                new CreateUserResponseDTO
                {
                    status = "success",
                    message = "User created successfully"
                }
            );
        }
    }
    public class CreateUserErrorResponseDTOExample : IMultipleExamplesProvider<CreateUserResponseDTO>
    {
        public IEnumerable<SwaggerExample<CreateUserResponseDTO>> GetExamples()
        {
            yield return SwaggerExample.Create(
                "Default",
                "Error Example when the UnlockUser operation is called with specific parameters",
                new CreateUserResponseDTO
                {
                    status = "fail",
                    message = "User created successfully"
                }
            );
        }
    }
    public class LoginResponseDTOExample : IMultipleExamplesProvider<LoginResponseDTO>
    {
        public IEnumerable<SwaggerExample<LoginResponseDTO>> GetExamples()
        {
            yield return SwaggerExample.Create(
                "Success",
                "Example of a successful login response",
                new LoginResponseDTO
                {
                    status = "success",
                    message = "Login success",
                    accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...", // Example token
                    maxLoginFailCount = 0 // No lock or failure in a successful login
                }
            );

            yield return SwaggerExample.Create(
                "Failed - Incorrect Credentials",
                "Example when login fails due to incorrect username or password",
                new LoginResponseDTO
                {
                    status = "fail",
                    message = "Username or Password is incorrect",
                    maxLoginFailCount = 1 // Example after one failed attempt
                }
            );

            yield return SwaggerExample.Create(
                "Failed - Account Locked",
                "Example when login fails because the account is locked",
                new LoginResponseDTO
                {
                    status = "fail",
                    message = "Account is locked",
                    maxLoginFailCount = 5 // Example when max failed attempts are reached
                }
            );
        }
    }
    public class LoginErrorResponseDTOExample : IMultipleExamplesProvider<CreateUserResponseDTO>
    {
        public IEnumerable<SwaggerExample<CreateUserResponseDTO>> GetExamples()
        {
            yield return SwaggerExample.Create(
                "Default",
                "Error Example when login fail",
                new CreateUserResponseDTO
                {
                    status = "fail",
                    message = "Login Fail."
                }
            );
        }
    }
    public class GetUserResponseDTOExample : IMultipleExamplesProvider<GetUserResponseDTO>
    {
        public IEnumerable<SwaggerExample<GetUserResponseDTO>> GetExamples()
        {
            yield return SwaggerExample.Create(
                "Standard User",
                "Example of a standard user with minimal fail count and not locked",
                new GetUserResponseDTO
                {
                    UserID = "1",
                    UserName = "jane.doe",
                    Password = "hashed_password_value", // Example hashed password
                    IsLock = "false",
                    FailCount = "0",
                    UserLevel = "User",
                    Salt = "random_salt_value"
                }
            );

            yield return SwaggerExample.Create(
                "Admin User - Locked",
                "Example of an admin user whose account is locked due to multiple failed login attempts",
                new GetUserResponseDTO
                {
                    UserID = "2",
                    UserName = "admin.user",
                    Password = "hashed_password_value", // Example hashed password
                    IsLock = "true",
                    FailCount = "5", // Max fail count reached
                    UserLevel = "Admin",
                    Salt = "random_salt_value"
                }
            );

            yield return SwaggerExample.Create(
                "Locked User",
                "Example of a user who is locked out due to exceeding the maximum number of failed login attempts",
                new GetUserResponseDTO
                {
                    UserID = "3",
                    UserName = "locked.user",
                    Password = "hashed_password_value",
                    IsLock = "true",
                    FailCount = "3",
                    UserLevel = "User",
                    Salt = "random_salt_value"
                }
            );
        }
    }
    public class UpdateUserResponseDTOExample : IMultipleExamplesProvider<CreateUserResponseDTO>
    {
        public IEnumerable<SwaggerExample<CreateUserResponseDTO>> GetExamples()
        {
            yield return SwaggerExample.Create(
                "Success",
                "Example response when the user is successfully updated",
                    new CreateUserResponseDTO
                    {
                        status = "Success",
                        message = "User Name Updated successfully"
                    }
                
            );
        }
    }
    public class UpdateUserErrorResponseDTOExample : IMultipleExamplesProvider<CreateUserResponseDTO>
    {
        public IEnumerable<SwaggerExample<CreateUserResponseDTO>> GetExamples()
        {
            yield return SwaggerExample.Create(
                "Failure - User Not Found",
                "Example response when the user to be updated is not found",
                
                    new CreateUserResponseDTO
                    {
                        status = "fail",
                        message = "User not found"
                    }
            );
        }
    }
}


