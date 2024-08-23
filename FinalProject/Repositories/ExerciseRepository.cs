using Dapper;
using Microsoft.EntityFrameworkCore;
using finalproject.DTO;
using finalproject.models;


namespace finalproject.Repositories
{
    public class ExerciseRepository : RepositoryBase<Exercise>, IExerciseRepository
    {
        public ExerciseRepository(AppDB context,IConfiguration configuration) : base(context,configuration)
        {
        }

        public async Task<IEnumerable<models.ExerciseResponseDTO>> GetAllExercise(int pageNumber, int pageSize)
        {
             var mainQuery = from exercise in _context.Exercise
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    select new models.ExerciseResponseDTO
                    {
                        exercise_id = exercise.exercise_id,
                        exercise_no = exercise.exercise_no,
                        description = exercise.description,
                        exercise_content = exercise.exercise_content,
                        pageSize = pageSize,
                        pageCount = (int)Math.Ceiling((double)_context.Exercise.Count() / pageSize),
                        pageNumber = pageNumber
                    };

            return await mainQuery.ToListAsync();
        }
        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}