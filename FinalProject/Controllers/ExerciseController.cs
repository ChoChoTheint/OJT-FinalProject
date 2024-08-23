using finalproject.DTO;
using finalproject.models;
using finalproject.Repositories;
using finalproject.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
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
    public class ExerciseController : ControllerBase
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExerciseController> _logger;

        public ExerciseController(IRepositoryWrapper repositoryWrapper,IConfiguration configuration, ILogger<ExerciseController> logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _configuration = configuration;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("AddExercise", Name = "AddExercise")]
        public async Task<ActionResult<Exercise>> AddExercise(ExerciseRequestDTO exercise)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var  addExercise = new Exercise
                {
                    exercise_no = exercise.ExerciseNo,
                    description = exercise.Description,
                    exercise_content = exercise.ExerciseContent
                };
                _repositoryWrapper.Exercise.Create(addExercise);
                int effectedRows = await _repositoryWrapper.Exercise.SaveAsync();
                if (effectedRows > 0)
                {
                    string exerciseObjString = JsonConvert.SerializeObject(exercise);
                    await _repositoryWrapper.EventLog.InsertEventLog("ExerciseController", "AddExercise", exerciseObjString);
                    var successResponse = new CreateExerciseResponseDTO
                    {
                        status = "success",
                        message = "Exercise created successfully"
                    };
                    return Ok(successResponse);
                }
                else
                {
                    return StatusCode(500, "Failed to add exercise");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in AddExercise method");
                await _repositoryWrapper.EventLog.ErrorEventLog("ExerciseController", "AddExercise", "Failed to add exercise", ex.Message);
                return StatusCode(500, "Internal server error");
            }

        }

        [Authorize]
        [HttpGet("GetAllExercises/pageNumber/{pageNumber}/pageSize/{pageSize}", Name = "GetAllExercises")]
        public async Task<ActionResult<IEnumerable<models.ExerciseResponseDTO>>> GetAllExercises(int pageNumber,int pageSize)
        {
            _logger?.LogInformation("GetAllExercises method called");
            IEnumerable<models.ExerciseResponseDTO> exerciseList = null;
            try
            { 
                int rowCount = 10;
                int pageCount = rowCount / pageSize;
                if(rowCount % pageSize > 0)
                {
                    pageCount++;
                }
                if(pageNumber > pageCount)
                {
                    return BadRequest(new {Message = "Invalid PageNo"});
                }
                exerciseList = await _repositoryWrapper.Exercise.GetAllExercise(pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in GetAllExercises method");
                return StatusCode(500, "Internal server error");
            }
            return Ok(exerciseList);
        }
        
        [Authorize]
        [HttpPut("UpdateExercise", Name = "UpdateExercise")]
        public async Task<IActionResult> UpdateExercise(UpdateExerciseRequestDTO exercise)
        {
            _logger?.LogInformation("UpdateExercise method called");
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var exerciseUpdate = new Exercise
                {
                    exercise_id = exercise.ExerciseID,
                    exercise_no = exercise.ExerciseNo,
                    description = exercise.Description,
                    exercise_content = exercise.ExerciseContent
                };
                _repositoryWrapper.Exercise.Update(exerciseUpdate);
                int effectedRows = await _repositoryWrapper.Exercise.SaveAsync();
                if (effectedRows > 0)
                {
                    string exerciseObjString = JsonConvert.SerializeObject(exercise);
                    await _repositoryWrapper.EventLog.InsertEventLog("ExerciseController", "UpdateExercise", exerciseObjString);
                    var successResponse = new CreateExerciseResponseDTO
                    {
                        status = "success",
                        message = "Exercise update successfully"
                    };
                    return Ok(successResponse);
                }
                else
                {
                    return StatusCode(500, "Failed to update exercise");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in UpdateExercise method");
           //     await _repositoryWrapper.EventLog.ErrorEventLog("ExerciseController", "UpdateExercise", "Failed to update exercise", ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize]
        [HttpDelete("DeleteExercise/{exerciseID}", Name = "DeleteExercise")]
        public async Task<IActionResult> DeleteExercise(int exerciseID)
        {
             _logger?.LogInformation("DeleteExercise method called");
            try
            {
                var exercise = await _repositoryWrapper.Exercise.FindByID(exerciseID);
                if (exercise == null)
                {
                    return NotFound("Exercise not found");
                }
                _repositoryWrapper.Exercise.Delete(exercise);
                _repositoryWrapper.Exercise.SaveAsync();
                var successResponse = new CreateExerciseResponseDTO
                    {
                        status = "success",
                        message = "Exercise delete successfully"
                    };
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in DeleteExercise method");
                await _repositoryWrapper.EventLog.ErrorEventLog("ExerciseController", "DeleteExercise", "Failed to delete exercise", ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize]
        [HttpPost("ExerciseAssign", Name = "ExerciseAssign")]
        public async Task<ActionResult<CreateExerciseAssignResponseDTO>> ExerciseAssign(ExerciseAssignRequestDTO assign)
        {
            _logger?.LogInformation("ExerciseAssign method called");
            try
            {
                var exerciseAssign = new ExerciseAssign
                {
                    exercise_id = assign.ExerciseID,
                    user_id = assign.StudentID,
                    mark = 0
                };
                _repositoryWrapper.ExerciseAssign.Create(exerciseAssign);
                await _repositoryWrapper.ExerciseAssign.Save();

                var successResponse = new CreateExerciseAssignResponseDTO
                {
                    status = "success",
                    message = "Exercise assign created successfully"
                };

                string exerciseAssignObjString = JsonConvert.SerializeObject(exerciseAssign);
                await _repositoryWrapper.EventLog.InsertEventLog("ExerciseController", "ExerciseAssign", exerciseAssignObjString);

                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in ExerciseAssign method");
               // await _repositoryWrapper.EventLog.ErrorEventLog("ExerciseController", "ExerciseAssign", "Failed to assign exercise", ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize]
        [HttpPut("AddStudentMark", Name = "AddStudentMark")]
        public async Task<ActionResult<CreateExerciseAssignResponseDTO>> AddStudentMark(UpdateMarkRequestDTO mark)
        {
            try
            {
                var response = new CreateExerciseAssignResponseDTO();
                var assignmark = await _repositoryWrapper.ExerciseAssign.FindByID(mark.ExerciseAssignID);
                if (assignmark == null)
                {
                    response = new CreateExerciseAssignResponseDTO
                    {
                        status = "fail",
                        message = "Exercise assign ID not found"
                    };
                    return BadRequest(response);
                }
                assignmark.exercise_assign_id = mark.ExerciseAssignID;
                assignmark.mark = mark.Mark;
                await _repositoryWrapper.ExerciseAssign.SaveAsync();
                response = new CreateExerciseAssignResponseDTO
                    {
                        status = "success",
                        message = "Student mark assigned successfully"
                    };

                string exerciseObjString = JsonConvert.SerializeObject(assignmark);
                await _repositoryWrapper.EventLog.InsertEventLog("ExerciseController", "AddStudentMark", exerciseObjString);

                return Ok(response);
                
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in AddStudentMark method");
                await _repositoryWrapper.EventLog.ErrorEventLog("ExerciseController", "AddStudentMark", "Failed to add student mark", ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize]
        [HttpGet("GetAllStudentGrade", Name = "GetAllStudentGrade")]
        public async Task<ActionResult<IEnumerable<StudentDetailResponseDTO>>> GetAllStudentGrade()
        {
                IEnumerable<StudentDetailResponseDTO> studentDetail = null;
                try
                {
                    _logger?.LogInformation("StudentDetail method called success");
                    studentDetail = await _repositoryWrapper.ExerciseAssign.GetStudedentDetail();
                }
                catch(Exception ex)
                {
                   
                    _logger?.LogError(ex, "Error in StudentDetail method");
                    return StatusCode(500, "Internal server error");
                }
                return Ok(studentDetail);

        }


        [Authorize]
        [HttpGet("GetStudentGrade", Name = "GetStudentGrade")]
        public async Task<ActionResult<IEnumerable<StudentDetailResponseDTO>>> GetStudentGrade()
        {
                int userID = int.Parse(HttpContext.User?.FindFirst(x => x.Type == "sid")?.Value ?? "0");
                IEnumerable<StudentDetailResponseDTO> studentDetail = null;
                try
                {
                    _logger?.LogInformation("GetStudentGrade method called success");
                    studentDetail = await _repositoryWrapper.ExerciseAssign.GetStudent(userID);
                }
                catch(Exception ex)
                {
                   
                    _logger?.LogError(ex, "Error in GetStudentGrade method");
                    return StatusCode(500, "Internal server error");
                }
                return Ok(studentDetail);

        }

    }
}



