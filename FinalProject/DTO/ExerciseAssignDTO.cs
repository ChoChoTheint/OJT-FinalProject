using System.Runtime.ConstrainedExecution;
using Swashbuckle.AspNetCore.Filters;

namespace finalproject.DTO
{
    public class ExerciseAssignRequestDTO
    {
        public required int ExerciseID { get; set; }
        public required int StudentID { get; set; }
        // public required int Mark{get;set;}
    }
    public class CreateExerciseAssignResponseDTO
    {
        public string status { get; set; }
        public string message { get; set; }
    }
    public class UpdateMarkRequestDTO
    {
        public int ExerciseAssignID{get;set;}
        public int Mark { get; set; }
    }
    public class StudentDetailResponseDTO
    {
        public int ExerciseAssignID{get;set;}
        public string ExerciseNo{get;set;}
        public string UserName { get; set; }
        public string Grade { get; set; }
    }
    

}


