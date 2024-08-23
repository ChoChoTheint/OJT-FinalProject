using Dapper;
using Microsoft.EntityFrameworkCore;
using finalproject.DTO;
using finalproject.models;


namespace finalproject.Repositories
{
    public class ExerciseAssignRepository : RepositoryBase<ExerciseAssign>, IExerciseAssignRepository
    {
        public ExerciseAssignRepository(AppDB context,IConfiguration configuration) : base(context,configuration)
        {
        }

        public async Task<IEnumerable<StudentDetailResponseDTO>> GetStudedentDetail()
        {
             var mainQuery = from exerciseAssign in _context.ExerciseAssign
                        join exercise in _context.Exercise on exerciseAssign.exercise_id equals exercise.exercise_id
                        join user in _context.User on exerciseAssign.user_id equals user.user_id
                        select new StudentDetailResponseDTO
                        {
                            ExerciseAssignID = exerciseAssign.exercise_assign_id,
                            Grade = GetGrade(exerciseAssign.mark),
                            ExerciseNo = exercise.exercise_no,
                            UserName = user.name
                        };


            return await mainQuery.ToListAsync();
        }

        private static string GetGrade(int mark)
        {
            if (mark == 0)
                return "This student doesn't add mark";
            else if (mark >= 80 && mark <= 100)
                return "A";
            else if (mark >= 60 && mark <= 79)
                return "B";
            else if (mark >= 40 && mark <= 59)
                return "C";
            else if (mark > 0 && mark <= 39)
                return "D";
            else
                return "F";
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    
        public async Task<IEnumerable<StudentDetailResponseDTO>> GetStudent(int id)
        {
            var loggedInUserId = id;
            var mainQuery = from exerciseAssign in _context.ExerciseAssign
                    join exercise in _context.Exercise on exerciseAssign.exercise_id equals exercise.exercise_id
                    join user in _context.User on exerciseAssign.user_id equals user.user_id
                    where exerciseAssign.user_id == loggedInUserId
                    select new StudentDetailResponseDTO
                    {
                        ExerciseAssignID = exerciseAssign.exercise_assign_id,
                        Grade = GetGrade(exerciseAssign.mark),
                        ExerciseNo = exercise.exercise_no,
                        UserName = user.name
                    };

    return await mainQuery.ToListAsync();
        }
    }
}