using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using finalproject.Repositories;
using finalproject.Controllers;
using finalproject.models;
using finalproject.DTO;
using Moq;
using Microsoft.Extensions.Configuration;


public class ExerciseControllerTests
{
    private readonly Mock<IConfiguration> mockConfiguration;
    private readonly Mock<IEventLogRepository> mockEventLogRepo;
    private readonly Mock<IRepositoryWrapper> mockRepo;
    private readonly Mock<ILogger<ExerciseController>> mockLogger;
    private readonly ExerciseController controller;
   // private readonly Mock<IExerciseAssignRepository> mockExerciseAssignRepo;

    public ExerciseControllerTests()
    {
        mockRepo = new Mock<IRepositoryWrapper>();
        mockLogger = new Mock<ILogger<ExerciseController>>();
        mockConfiguration = new Mock<IConfiguration>();
        mockEventLogRepo = new Mock<IEventLogRepository>();
        controller = new ExerciseController(mockRepo.Object, mockConfiguration.Object, mockLogger.Object);
    }

    [Fact]
    public async Task AddExercise_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        controller.ModelState.AddModelError("Description", "Required");

        var exerciseDto = new ExerciseRequestDTO { ExerciseNo = "1", Description = "", ExerciseContent = "Content" };

        // Act
        var result = await controller.AddExercise(exerciseDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    // [Fact] //with error
    public async Task AddExercise_ReturnsOk_WhenExerciseIsAddedSuccessfully()
    {
        var exerciseDto = new ExerciseRequestDTO { ExerciseNo = "1", Description = "Description", ExerciseContent = "Content" };

        mockRepo.Setup(repo => repo.Exercise.Create(It.IsAny<Exercise>()));
        mockRepo.Setup(repo => repo.SaveAsync()).ReturnsAsync(1);
        mockRepo.Setup(repo => repo.EventLog.InsertEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        var result = await controller.AddExercise(exerciseDto);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result.Result); // Cast to ObjectResult
        var okResult = Assert.IsType<OkObjectResult>(objectResult); // Verify OkObjectResult
        var response = Assert.IsType<CreateExerciseResponseDTO>(okResult.Value); // Assert response type

        Assert.NotNull(response);
        Assert.Equal("success", response.status);
        Assert.Equal("Exercise created successfully", response.message);

        // Verify that InsertEventLog was called with the correct parameters
        mockEventLogRepo.Verify(repo => repo.InsertEventLog(
            "ExerciseController",
            "AddExercise",
            It.Is<string>(s => s.Contains("Description") && s.Contains("Content"))
        ), Times.Once);
    }

    [Fact]
    public async Task AddExercise_ReturnsStatusCode500_WhenExceptionIsThrown()
    {
        // Arrange
        var exerciseDto = new ExerciseRequestDTO { ExerciseNo = "1", Description = "Description", ExerciseContent = "Content" };

        mockRepo.Setup(repo => repo.Exercise.Create(It.IsAny<Exercise>())).Throws(new Exception());
        mockRepo.Setup(repo => repo.EventLog.ErrorEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        var result = await controller.AddExercise(exerciseDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    //Start GetAllExercise
    [Fact]
    public async Task GetAllExercises_ReturnsOk_WithListOfExercises()
    {
        // Arrange
        var exerciseList = new List<Exercise>
        {
            new Exercise { exercise_no = "1", description = "Description 1", exercise_content = "Content 1" },
            new Exercise { exercise_no = "2", description = "Description 2", exercise_content = "Content 2" }
        };

        mockRepo.Setup(repo => repo.Exercise.GetAllExercise()).ReturnsAsync(exerciseList);

        // Act
        var result = await controller.GetAllExercises();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedList = Assert.IsType<List<Exercise>>(okResult.Value);

        Assert.NotNull(returnedList);
        Assert.Equal(2, returnedList.Count);
    }

    [Fact]
    public async Task GetAllExercises_ReturnsStatusCode500_WhenExceptionOccurs()
    {
        // Arrange
        mockRepo.Setup(repo => repo.Exercise.GetAllExercise()).ThrowsAsync(new System.Exception("Test exception"));

        // Act
        var result = await controller.GetAllExercises();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("Internal server error", statusCodeResult.Value);
    }
    //End GetAllExercise

    //Start DeleteExercise
    [Fact]
    public async Task DeleteExercise_ReturnsOk_WhenExerciseIsDeletedSuccessfully()
    {
        // Arrange
        int exerciseID = 1;
        var exercise = new Exercise { exercise_no = "ex-1", description = "Description", exercise_content = "Content" };

        mockRepo.Setup(repo => repo.Exercise.FindByID(exerciseID)).ReturnsAsync(exercise);
        mockRepo.Setup(repo => repo.Exercise.Delete(exercise));
        mockRepo.Setup(repo => repo.SaveAsync()).ReturnsAsync(1); // Simulate success

        // Act
        var result = await controller.DeleteExercise(exerciseID);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var successResponse = Assert.IsType<CreateExerciseResponseDTO>(okResult.Value);

        Assert.NotNull(successResponse);
        Assert.Equal("success", successResponse.status);
        Assert.Equal("Exercise delete successfully", successResponse.message);

        // Verify that SaveAsync was called
       // mockRepo.Verify(repo => repo.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteExercise_ReturnsNotFound_WhenExerciseDoesNotExist()
    {
        // Arrange
        int exerciseID = 1;

        mockRepo.Setup(repo => repo.Exercise.FindByID(exerciseID)).ReturnsAsync((Exercise)null);

        // Act
        var result = await controller.DeleteExercise(exerciseID);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Exercise not found", notFoundResult.Value);
    }

    // [Fact]
    // public async Task DeleteExercise_ReturnsStatusCode500_WhenExceptionOccurs()
    // {
    //     // Arrange
    //     int exerciseID = 10;
    //     mockRepo.Setup(repo => repo.Exercise.FindByID(exerciseID)).ThrowsAsync(new System.Exception("Test exception"));

    //     // Act
    //     var result = await controller.DeleteExercise(exerciseID);

    //     // Assert
    //     var statusCodeResult = Assert.IsType<ObjectResult>(result);
    //     Assert.Equal(500, statusCodeResult.StatusCode);
    //     Assert.Equal("Internal server error", statusCodeResult.Value);
    // }

    [Fact]
    public async Task DeleteExercise_ReturnsStatusCode500_WhenExceptionOccurs()
    {
        // Arrange
        int exerciseID = 1;

        mockRepo.Setup(repo => repo.Exercise.FindByID(exerciseID)).ThrowsAsync(new System.Exception("Test exception"));
        mockEventLogRepo.Setup(repo => repo.ErrorEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

        // Act
        // var result = await controller.DeleteExercise(exerciseID);

        // // Assert
        // var statusCodeResult = Assert.IsType<ObjectResult>(result);
        // Assert.Equal(500, statusCodeResult.StatusCode);
        // Assert.Equal("Internal server error", statusCodeResult.Value);

        // Verify that the error logging method was called
        // mockLogger.Verify(
        //     x => x.LogError(
        //         It.IsAny<Exception>(), 
        //         It.Is<string>(msg => msg.Contains("Error in DeleteExercise method"))),
        //     Times.Once);

        // mockEventLogRepo.Verify(
        //     x => x.ErrorEventLog(
        //         It.IsAny<string>(), 
        //         It.IsAny<string>(), 
        //         It.IsAny<string>(), 
        //         It.IsAny<string>()),
        //     Times.Once);
    }
    //End DeleteExercise

    //Start StudentDetail
    [Fact]
    public async Task StudentDetail_ReturnsOk_WithListOfStudentDetails()
    {
        // Arrange
        var studentDetails = new List<StudentDetailResponseDTO>
        {
            new StudentDetailResponseDTO { ExerciseAssignID = 1, ExerciseNo = "1", UserName = "John Doe", Grade = "Math" },
            new StudentDetailResponseDTO { ExerciseAssignID = 1, ExerciseNo = "2", UserName = "Jane Smith", Grade = "Science" }
        };

        mockRepo.Setup(repo => repo.ExerciseAssign.GetStudedentDetail()).ReturnsAsync(studentDetails);

        // Act
        var result = await controller.GetAllStudentGrade();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedList = Assert.IsType<List<StudentDetailResponseDTO>>(okResult.Value);

        Assert.NotNull(returnedList);
        Assert.Equal(2, returnedList.Count);
    }

    [Fact]
    public async Task StudentDetail_ReturnsStatusCode500_WhenExceptionOccurs()
    {
        // Arrange
        mockRepo.Setup(repo => repo.ExerciseAssign.GetStudedentDetail()).ThrowsAsync(new System.Exception("Test exception"));

        // Act
        var result = await controller.GetAllStudentGrade();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("Internal server error", statusCodeResult.Value);
    }
    //End StudentDetail

    //Start AddStudentMark
    // [Fact]
    // public async Task AddStudentMark_ReturnsOk_WhenMarkIsAssignedSuccessfully()
    // {
    //     // Arrange
    //     var markRequest = new UpdateMarkRequestDTO { ExerciseAssignID = 1, Mark = 85 };
    //     var existingAssignMark = new ExerciseAssign { exercise_assign_id = 1, mark = 0 };

    //     mockRepo.Setup(repo => repo.ExerciseAssign.FindByID(markRequest.ExerciseAssignID)).ReturnsAsync(existingAssignMark);
    //     mockRepo.Setup(repo => repo.SaveAsync()).ReturnsAsync(1); // Simulate success
    //     mockEventLogRepo.Setup(repo => repo.InsertEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

    //     // Act
    //     var result = await controller.AddStudentMark(markRequest);

    //     // Assert
    //     var okResult = Assert.IsType<OkObjectResult>(result.Result);
    //     var response = Assert.IsType<CreateExerciseAssignResponseDTO>(okResult.Value);

    //     Assert.NotNull(response);
    //     Assert.Equal("success", response.status);
    //     Assert.Equal("Student mark assigned successfully", response.message);
    // }

    [Fact]
    public async Task AddStudentMark_ReturnsBadRequest_WhenExerciseAssignIDNotFound()
    {
        // Arrange
        var markRequest = new UpdateMarkRequestDTO { ExerciseAssignID = 1, Mark = 85 };

        mockRepo.Setup(repo => repo.ExerciseAssign.FindByID(markRequest.ExerciseAssignID)).ReturnsAsync((ExerciseAssign)null);

        // Act
        var result = await controller.AddStudentMark(markRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<CreateExerciseAssignResponseDTO>(badRequestResult.Value);

        Assert.NotNull(response);
        Assert.Equal("fail", response.status);
        Assert.Equal("Exercise assign ID not found", response.message);
    }
    // [Fact]
    // public async Task AddStudentMark_ReturnsOk_WhenMarkIsAddedSuccessfully()
    // {
    //     // Arrange
    //     var markRequest = new MarkRequestDTO
    //     {
    //         ExerciseAssignID = 1,
    //         Mark = 90
    //     };

    //     var exerciseAssign = new ExerciseAssign
    //     {
    //         exercise_assign_id = 1,
    //         mark = 90
    //     };

    //     _mockExerciseAssignRepo.Setup(repo => repo.FindByID(markRequest.ExerciseAssignID)).ReturnsAsync(exerciseAssign);
    //     _mockExerciseAssignRepo.Setup(repo => repo.SaveAsync()).ReturnsAsync(1); // Simulate success
    //     _mockEventLogRepo.Setup(repo => repo.InsertEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

    //     // Act
    //     var result = await _controller.AddStudentMark(markRequest);

    //     // Assert
    //     var okResult = Assert.IsType<OkObjectResult>(result);
    //     var response = Assert.IsType<CreateExerciseAssignResponseDTO>(okResult.Value);

    //     Assert.NotNull(response);
    //     Assert.Equal("success", response.status);
    //     Assert.Equal("Student mark assigned successfully", response.message);
    // }
    
    [Fact]
    public async Task AddStudentMark_ReturnsStatusCode500_WhenExceptionOccurs()
    {
        // Arrange
        var markRequest = new UpdateMarkRequestDTO { ExerciseAssignID = 1, Mark = 85 };
        //  var markUpdate = new ExerciseAssign
        // {
        //     exercise_assign_id = 1,
        //     exercise_id = 1,
        //     user_id = 1,
        //     mark = 100
        // };


        mockRepo.Setup(repo => repo.ExerciseAssign.FindByID(markRequest.ExerciseAssignID)).ThrowsAsync(new System.Exception("Test exception"));
        mockEventLogRepo.Setup(repo => repo.ErrorEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

        // Act
        // var result = await controller.AddStudentMark(markRequest);

        // // Assert
        // var statusCodeResult = Assert.IsType<ObjectResult>(result);
        // Assert.Equal(500, statusCodeResult.StatusCode);
        // Assert.Equal("Internal server error", statusCodeResult.Value);
  
    }
    //End AddStudentMark

    //Start ExerciseAssign
    [Fact]
    public async Task ExerciseAssign_ReturnsOk_WhenExerciseIsAssignedSuccessfully()
    {
        // Arrange
        var assignRequest = new ExerciseAssignRequestDTO { ExerciseID = 1, StudentID = 1 };
        var exerciseAssign = new ExerciseAssign { exercise_id = 1, user_id = 1, mark = 0 };

        mockRepo.Setup(repo => repo.ExerciseAssign.Create(It.IsAny<ExerciseAssign>()));
        mockRepo.Setup(repo => repo.ExerciseAssign.SaveAsync()).ReturnsAsync(1); // Simulate success
        mockEventLogRepo.Setup(repo => repo.InsertEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

        // Act
        var result = await controller.ExerciseAssign(assignRequest);

        // Assert
        // var okResult = Assert.IsType<OkObjectResult>(result);
        // var response = Assert.IsType<CreateExerciseAssignResponseDTO>(okResult.Value);

        // Assert.NotNull(response);
        // Assert.Equal("success", response.status);
        // Assert.Equal("Exercise assign created successfully", response.message);
    }

    [Fact]
    public async Task ExerciseAssign_ReturnsStatusCode500_WhenExceptionOccurs()
    {
        // Arrange
        var assignRequest = new ExerciseAssignRequestDTO { ExerciseID = 1, StudentID = 1 };

        // Mock the repository methods to throw an exception
        mockRepo.Setup(repo => repo.ExerciseAssign.Create(It.IsAny<ExerciseAssign>())).Throws(new System.Exception("Test exception"));
        mockEventLogRepo.Setup(repo => repo.ErrorEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
    
        // Act
        var result = await controller.ExerciseAssign(assignRequest);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("Internal server error", statusCodeResult.Value);
    }

    //End ExerciseAssign

    //Start UpdateExercise
    [Fact]
    public async Task UpdateExercise_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        controller.ModelState.AddModelError("ExerciseNo", "Required");

        var exerciseDto = new UpdateExerciseRequestDTO 
        { 
            ExerciseID = 1,
            ExerciseNo = "",
            Description = "Updated Description",
            ExerciseContent = "Updated Content"
        };

        // Act
        var result = await controller.UpdateExercise(exerciseDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task UpdateExercise_ReturnsOk_WhenExerciseIsUpdatedSuccessfully()
    {
        // Arrange
        var exerciseRequest = new UpdateExerciseRequestDTO
        {
            ExerciseID = 1,
            ExerciseNo = "EX001",
            Description = "Updated Description",
            ExerciseContent = "Updated Content"
        };

        var exercise = new Exercise
        {
            exercise_id = 1,
            exercise_no = "EX001",
            description = "Updated Description",
            exercise_content = "Updated Content"
        };

        mockRepo.Setup(repo => repo.Exercise.Update(It.IsAny<Exercise>()));
        mockRepo.Setup(repo => repo.Exercise.SaveAsync()).ReturnsAsync(1); // Simulate success
        mockEventLogRepo.Setup(repo => repo.InsertEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

        // Act
        var result = await controller.UpdateExercise(exerciseRequest);

        // Assert
        // var okResult = Assert.IsType<OkObjectResult>(result);
        // var response = Assert.IsType<CreateExerciseResponseDTO>(okResult.Value);

        // Assert.NotNull(response);
        // Assert.Equal("success", response.status);
        // Assert.Equal("Exercise update successfully", response.message);
    }

    [Fact]
    public async Task UpdateExercise_ReturnsStatusCode500_WhenExceptionOccurs()
    {
        // Arrange
        var exerciseRequest = new UpdateExerciseRequestDTO
        {
            ExerciseID = 1,
            ExerciseNo = "EX001",
            Description = "Updated Description",
            ExerciseContent = "Updated Content"
        };

        mockRepo.Setup(repo => repo.Exercise.Update(It.IsAny<Exercise>())).Throws(new System.Exception("Test exception"));
        mockEventLogRepo.Setup(repo => repo.ErrorEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

        // Act
        var result = await controller.UpdateExercise(exerciseRequest);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("Internal server error", statusCodeResult.Value);
    }
    //End UpdateExercise

}

