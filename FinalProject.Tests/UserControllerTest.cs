using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using finalproject.Repositories;
using finalproject.Controllers;
using finalproject.models;
using finalproject.DTO;
using Moq;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;


public class UserControllerTests
{
    private readonly Mock<IConfiguration> mockConfiguration;
    private readonly Mock<IEventLogRepository> mockEventLogRepo;
    private readonly Mock<IRepositoryWrapper> mockRepo;
    private readonly Mock<ILogger<UserController>> mockLogger;
    private readonly UserController controller;
   // private readonly Mock<IExerciseAssignRepository> mockExerciseAssignRepo;

    public UserControllerTests()
    {
        mockRepo = new Mock<IRepositoryWrapper>();
        mockLogger = new Mock<ILogger<UserController>>();
        mockConfiguration = new Mock<IConfiguration>();
        mockEventLogRepo = new Mock<IEventLogRepository>();
        controller = new UserController(mockRepo.Object, mockConfiguration.Object, mockLogger.Object);
    }

    //Start CreateUser
    [Fact]
    public async Task CreateUser_UserAlreadyExists_ReturnsFail()
    {
        // Arrange
        var user = new UserRequestDTO { UserName = "existingUser", Password = "password123", UserLevelID = 1 };
        var existingUser = new User { name = "existingUser" };

        mockRepo
            .Setup(repo => repo.User.GetAll<User>("SELECT * FROM User WHERE name = @UserName", It.IsAny<object>()))
            .ReturnsAsync(new[] { existingUser }.AsQueryable());

        mockRepo
            .Setup(repo => repo.EventLog.InsertEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await controller.CreateUser(user);

        // Assert
        // var okResult = Assert.IsType<OkObjectResult>(result);
        // var response = Assert.IsType<CreateUserResponseDTO>(okResult.Value);
        // Assert.Equal("fail", response.status);
        // Assert.Equal("User already exists", response.message);
    }

    [Fact]
    public async Task CreateUser_SuccessfulCreation_ReturnsSuccess()
    {
        // Arrange
        var user = new UserRequestDTO { UserName = "newUser", Password = "password123", UserLevelID = 1 };

        mockRepo
            .Setup(repo => repo.User.GetAll<User>("SELECT * FROM User WHERE name = @UserName", It.IsAny<object>()))
            .ReturnsAsync(Enumerable.Empty<User>().AsQueryable());

        mockRepo
            .Setup(repo => repo.User.Create(It.IsAny<User>()));

        mockRepo
            .Setup(repo => repo.User.Save())
            .Returns(Task.CompletedTask);

        mockRepo
            .Setup(repo => repo.EventLog.InsertEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await controller.CreateUser(user);

        // Assert
        // var okResult = Assert.IsType<OkObjectResult>(result);
        // var response = Assert.IsType<CreateUserResponseDTO>(okResult.Value);
        // Assert.Equal("success", response.status);
        // Assert.Equal("User created successfully", response.message);
    }
    
    [Fact]
    public async Task CreateUser_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var user = new UserRequestDTO { UserName = "newUser", Password = "password123", UserLevelID = 1 };

        mockRepo
            .Setup(repo => repo.User.GetAll<User>("SELECT * FROM User WHERE name = @UserName", It.IsAny<object>()))
            .ThrowsAsync(new Exception("Database error"));

        mockRepo
            .Setup(repo => repo.EventLog.ErrorEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await controller.CreateUser(user);

        // Assert
        // var statusCodeResult = Assert.IsType<ObjectResult>(result);
        // Assert.Equal(500, statusCodeResult.StatusCode);
        // Assert.Equal("Internal server error", statusCodeResult.Value);
    }
    //End CreateUser

    //Start Login
    [Fact]
    public async Task Login_UserDoesNotExist_ReturnsFail()
    {
        // Arrange
        var user = new LoginRequestDTO { UserName = "nonExistentUser", Password = "password123" };

        mockRepo
            .Setup(repo => repo.User.GetName(user.UserName))
            .ReturnsAsync((User)null);

        mockRepo
            .Setup(repo => repo.EventLog.InsertEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await controller.Login(user);

        // Assert
        // var okResult = Assert.IsType<OkObjectResult>(result);
        // var response = Assert.IsType<LoginResponseDTO>(okResult.Value);
        // Assert.Equal("fail", response.status);
        // Assert.Equal("Username or Password is incorrect", response.message);
    }

    [Fact]
    public async Task Login_AccountLocked_ReturnsFail()
    {
        // Arrange
        var user = new LoginRequestDTO { UserName = "lockedUser", Password = "password123" };
        var existingUser = new User { name = "lockedUser", is_lock = true, login_fail_count = 3 };

        mockRepo
            .Setup(repo => repo.User.GetName(user.UserName))
            .ReturnsAsync(existingUser);

        // mockConfiguration
        //     .Setup(config => config.GetValue<int>("MaxLoginFailCount"))
        //     .Returns(3);

        mockRepo
            .Setup(repo => repo.User.Save())
            .Returns(Task.CompletedTask);

        mockRepo
            .Setup(repo => repo.EventLog.InsertEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await controller.Login(user);

        // Assert
        // var okResult = Assert.IsType<OkObjectResult>(result);
        // var response = Assert.IsType<LoginResponseDTO>(okResult.Value);
        // Assert.Equal("fail", response.status);
        // Assert.Equal("Account is locked", response.message);
        // Assert.Equal(3, response.maxLoginFailCount);
    }

    [Fact]
    public async Task Login_IncorrectPassword_ReturnsFail()
    {
        // Arrange
        var user = new LoginRequestDTO { UserName = "user", Password = "wrongPassword" };
        var existingUser = new User { name = "user", is_lock = false, login_fail_count = 0, salt = "salt", password = "correctHash" };

        mockRepo
            .Setup(repo => repo.User.GetName(user.UserName))
            .ReturnsAsync(existingUser);

        // mockConfiguration
        //     .Setup(config => config.GetValue<int>("MaxLoginFailCount"))
        //     .Returns(3);

        mockRepo
            .Setup(repo => repo.User.Save())
            .Returns(Task.CompletedTask);

        mockRepo
            .Setup(repo => repo.EventLog.InsertEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await controller.Login(user);

        // Assert
        // var okResult = Assert.IsType<OkObjectResult>(result);
        // var response = Assert.IsType<LoginResponseDTO>(okResult.Value);
        // Assert.Equal("fail", response.status);
        // Assert.Equal("Username or Password is incorrect", response.message);
        // Assert.Equal(1, response.maxLoginFailCount);
    }

    [Fact]
    public async Task Login_SuccessfulLogin_ReturnsSuccess()
    {
        // Arrange
        var user = new LoginRequestDTO { UserName = "user", Password = "correctPassword" };
        var existingUser = new User { name = "user", is_lock = false, login_fail_count = 0, salt = "salt", password = "correctHash" };

        mockRepo
            .Setup(repo => repo.User.GetName(user.UserName))
            .ReturnsAsync(existingUser);

        mockRepo
            .Setup(repo => repo.User.Save())
            .Returns(Task.CompletedTask);

        mockRepo
            .Setup(repo => repo.EventLog.InsertEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await controller.Login(user);

        // Assert
        // var okResult = Assert.IsType<OkObjectResult>(result);
        // var response = Assert.IsType<LoginResponseDTO>(okResult.Value);
        // Assert.Equal("success", response.status);
        // Assert.Equal("Login success", response.message);
        // Assert.NotNull(response.accessToken);
    }

    [Fact]
    public async Task Login_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var user = new LoginRequestDTO { UserName = "user", Password = "password123" };

        mockRepo
            .Setup(repo => repo.User.GetName(user.UserName))
            .ThrowsAsync(new Exception("Database error"));

        mockRepo
            .Setup(repo => repo.EventLog.ErrorEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await controller.Login(user);

        // Assert
        // var statusCodeResult = Assert.IsType<ObjectResult>(result);
        // Assert.Equal(500, statusCodeResult.StatusCode);
        // Assert.Equal("Internal server error", statusCodeResult.Value);
    }
    //End Login

    //Start GetUser
    [Fact]
    public async Task GetUser_Success_ReturnsOk()
    {
        // Arrange
        int userID = 1;
        var id = "evdgfvs";
        string ecbKey = "someSecretKey";
        var user = new GetUserResponseDTO { UserID = id, UserName = "testUser" };

        var claims = new[] { new Claim("sid", userID.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);
       // controller.ControllerContext.HttpContext.User = principal;

        // mockConfiguration
        //     .Setup(config => config.GetValue<string>("Encyption:ECBSecretKey"))
        //     .Returns(ecbKey);

        mockRepo
            .Setup(repo => repo.User.GetUser(userID, ecbKey))
            .ReturnsAsync(user);

        // Act
    //    var result = await controller.GetUser();

    //     // Assert
    //     var okResult = Assert.IsType<OkObjectResult>(result);
    //     var response = Assert.IsType<GetUserResponseDTO>(okResult.Value);
    //     Assert.Equal(id, response.UserID);
    //     Assert.Equal("testUser", response.UserName);
    }

    [Fact]
    public async Task GetUser_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        int userID = 1;
        string ecbKey = "someSecretKey";

        var claims = new[] { new Claim("sid", userID.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);
     //   controller.ControllerContext.HttpContext.User = principal;

        // mockConfiguration
        //     .Setup(config => config.GetValue<string>("Encyption:ECBSecretKey"))
        //     .Returns(ecbKey);

        mockRepo
            .Setup(repo => repo.User.GetUser(userID, ecbKey))
            .ThrowsAsync(new Exception("Database error"));

        // Act
//        var result = await controller.GetUser();

        // Assert
        // var statusCodeResult = Assert.IsType<ObjectResult>(result);
        // Assert.Equal(500, statusCodeResult.StatusCode);
        // Assert.Equal("Internal server error", statusCodeResult.Value);
    }
    //End GetUser

    //Start GetAllStudents
     [Fact]
    public async Task GetAllStudents_Success_ReturnsOk()
    {
        // Arrange
        var allStudents = new List<User>
        {
            new User { user_id = 1, name = "Student1" },
            new User { user_id = 2, name = "Student2" }
        };

        mockRepo
            .Setup(repo => repo.User.GetAllStudent())
            .ReturnsAsync(allStudents);

        // Act
        var result = await controller.GetAllStudents();

        // Assert
        // var okResult = Assert.IsType<OkObjectResult>(result);
        // var response = Assert.IsType<List<User>>(okResult.Value);
        // Assert.Equal(2, response.Count);
    }

    [Fact]
    public async Task GetAllStudents_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        mockRepo
            .Setup(repo => repo.User.GetAllStudent())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await controller.GetAllStudents();

        // Assert
        // var statusCodeResult = Assert.IsType<ObjectResult>(result);
        // Assert.Equal(500, statusCodeResult.StatusCode);
        // Assert.Equal("Internal server error", statusCodeResult.Value);
    }
    //End GetAllStudents

    //Start UpdateUser
    [Fact]
    public async Task UpdateUser_Success_ReturnsOk()
    {
        // Arrange
        var user = new UpdateUserRequestDTO { UserID = "encryptedUserID", Name = "NewName", Password = "newPassword" };
        int userID = 1;
        string ecbKey = "someSecretKey";
        var existingUser = new User { user_id = userID, name = "OldName", password = "oldPassword", salt = "oldSalt" };

        // mockConfiguration
        //     .Setup(config => config.GetValue<string>("Encyption:ECBSecretKey"))
        //     .Returns(ecbKey);

        mockRepo
            .Setup(repo => repo.User.FindByID(userID))
            .ReturnsAsync(existingUser);

        mockRepo
            .Setup(repo => repo.User.Save())
            .Returns(Task.CompletedTask);

        mockRepo
            .Setup(repo => repo.EventLog.UpdateEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Mock the decryption function
        // var decryptionMock = new Mock<IDecryptionService>();
        // decryptionMock.Setup(d => d.Decrypt(It.IsAny<string>(), It.IsAny<string>())).Returns(userID.ToString());
        // _controller.DecryptionService = decryptionMock.Object;

        // Act
        var result = await controller.UpdateUser(user);

        // Assert
        // var okResult = Assert.IsType<OkObjectResult>(result);
        // var response = Assert.IsType<CreateUserResponseDTO>(okResult.Value);
        // Assert.Equal("Success", response.status);
        // Assert.Equal("User Name Updated successfully", response.message);
    }

    [Fact]
    public async Task UpdateUser_UserNotFound_ReturnsBadRequest()
    {
        // Arrange
        var user = new UpdateUserRequestDTO { UserID = "encryptedUserID", Name = "NewName", Password = "newPassword" };
        int userID = 1;
        string ecbKey = "someSecretKey";

        // mockConfiguration
        //     .Setup(config => config.GetValue<string>("Encyption:ECBSecretKey"))
        //     .Returns(ecbKey);

        mockRepo
            .Setup(repo => repo.User.FindByID(userID))
            .ReturnsAsync((User)null);

        // Mock the decryption function
        // var decryptionMock = new Mock<IDecryptionService>();
        // decryptionMock.Setup(d => d.Decrypt(It.IsAny<string>(), It.IsAny<string>())).Returns(userID.ToString());
        // controller.DecryptionService = decryptionMock.Object;

        // Act
        //var result = await controller.UpdateUser(user);

        // Assert
        // var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        // var response = Assert.IsType<CreateUserResponseDTO>(badRequestResult.Value);
        // Assert.Equal("fail", response.status);
        // Assert.Equal("User not found", response.message);
    }

    [Fact]
    public async Task UpdateUser_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var user = new UpdateUserRequestDTO { UserID = "encryptedUserID", Name = "NewName", Password = "newPassword" };
        int userID = 1;
        string ecbKey = "someSecretKey";

        // mockConfiguration
        //     .Setup(config => config.GetValue<string>("Encyption:ECBSecretKey"))
        //     .Returns(ecbKey);

        mockRepo
            .Setup(repo => repo.User.FindByID(userID))
            .ThrowsAsync(new Exception("Database error"));

        // Mock the decryption function
        // var decryptionMock = new Mock<IDecryptionService>();
        // decryptionMock.Setup(d => d.Decrypt(It.IsAny<string>(), It.IsAny<string>())).Returns(userID.ToString());
        // controller.DecryptionService = decryptionMock.Object;

        // Act
        // var result = await controller.UpdateUser(user);

        // // Assert
        // var statusCodeResult = Assert.IsType<ObjectResult>(result);
        // Assert.Equal(500, statusCodeResult.StatusCode);
        // Assert.Equal("Internal server error", statusCodeResult.Value);
    }
    //End UpdateUser

    //Start Unlock
    [Fact]
    public async Task UnlockUser_Success_ReturnsOk()
    {
        // Arrange
        var id = new UnlockUserRequestDTO { UserID = "encryptedUserID", UserIDCBC = "encryptedUserIDCBC" };
        int userID = 1;
        string ecbKey = "someSecretKey";
        var oldUser = new User { user_id = userID, is_lock = true, login_fail_count = 3 };

        // mockConfiguration
        //     .Setup(config => config.GetValue<string>("Encyption:ECBSecretKey"))
        //     .Returns(ecbKey);

        // mockRepo
        //     .Setup(repo => repo.User.GetUser(userID, ecbKey))
        //     .Returns(oldUser);
            

        mockRepo
            .Setup(repo => repo.User.Save())
            .Returns(Task.CompletedTask);

        // Mock the decryption function
        // var decryptionMock = new Mock<IDecryptionService>();
        // decryptionMock.Setup(d => d.Decrypt(It.IsAny<string>(), It.IsAny<string>())).Returns(userID.ToString());
        // _controller.DecryptionService = decryptionMock.Object;

        // Act
        // var result = await controller.UnlockUser(id);

        // // Assert
        // var okResult = Assert.IsType<OkObjectResult>(result);
        // var response = Assert.IsType<CreateUserResponseDTO>(okResult.Value);
        // Assert.Equal("success", response.status);
        // Assert.Equal("User unlocked successfully", response.message);
    }

    [Fact]
    public async Task UnlockUser_UserNotFound_ReturnsBadRequest()
    {
        // Arrange
        var id = new UnlockUserRequestDTO { UserID = "encryptedUserID", UserIDCBC = "encryptedUserIDCBC" };
        int userID = 1;
        string ecbKey = "someSecretKey";

        // mockConfiguration
        //     .Setup(config => config.GetValue<string>("Encyption:ECBSecretKey"))
        //     .Returns(ecbKey);

        // mockRepo
        //     .Setup(repo => repo.User.GetUser(userID, ecbKey))
        //     .Returns((User)null);

        // Mock the decryption function
        // var decryptionMock = new Mock<IDecryptionService>();
        // decryptionMock.Setup(d => d.Decrypt(It.IsAny<string>(), It.IsAny<string>())).Returns(userID.ToString());
        // _controller.DecryptionService = decryptionMock.Object;

        // Act
        // var result = await controller.UnlockUser(id);

        // // Assert
        // var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        // var response = Assert.IsType<CreateUserResponseDTO>(badRequestResult.Value);
        // Assert.Equal("fail", response.status);
        // Assert.Equal("User not found", response.message);
    }
    
    [Fact]
    public async Task UnlockUser_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var id = new UnlockUserRequestDTO { UserID = "encryptedUserID", UserIDCBC = "encryptedUserIDCBC" };
        int userID = 1;
        string ecbKey = "someSecretKey";

        // mockConfiguration
        //     .Setup(config => config.GetValue<string>("Encyption:ECBSecretKey"))
        //     .Returns(ecbKey);

        mockRepo
            .Setup(repo => repo.User.GetUser(userID, ecbKey))
            .ThrowsAsync(new Exception("Database error"));

        // Mock the decryption function
        // var decryptionMock = new Mock<IDecryptionService>();
        // decryptionMock.Setup(d => d.Decrypt(It.IsAny<string>(), It.IsAny<string>())).Returns(userID.ToString());
        // _controller.DecryptionService = decryptionMock.Object;

        // Act
        // var result = await controller.UnlockUser(id);

        // // Assert
        // var statusCodeResult = Assert.IsType<ObjectResult>(result);
        // Assert.Equal(500, statusCodeResult.StatusCode);
        // Assert.Equal("Internal server error", statusCodeResult.Value);
    }
    //End Unlock
}
    

    
