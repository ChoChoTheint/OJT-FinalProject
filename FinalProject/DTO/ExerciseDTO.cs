using System.Runtime.ConstrainedExecution;
using Swashbuckle.AspNetCore.Filters;

namespace finalproject.DTO
{
    public class ExerciseRequestDTO
    {
        public required string ExerciseNo { get; set; }
        public required string Description { get; set; }
        public required string ExerciseContent{get;set;}
    }
    public class CreateExerciseResponseDTO
    {
        public string status { get; set; }
        public string message { get; set; }
    }
    public class UpdateExerciseRequestDTO
    {
        public int ExerciseID {get;set;}
        public string ExerciseNo { get; set; }
        public string Description { get; set; }
        public string ExerciseContent{get;set;}
    }
    
    public class ExerciseResponseDTO
    {
        public int exercise_id{get;set;}
        public  string exercise_no { get; set; }
        public  string description { get; set; }
        public  string exercise_content { get; set; }
        public  int pageSize { get; set; }
        public  int pageNumber { get; set; }
        public  int pageCount {get;set;}
    }
}


