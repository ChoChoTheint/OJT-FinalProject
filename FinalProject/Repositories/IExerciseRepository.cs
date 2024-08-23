using finalproject.DTO;
using finalproject.models;


namespace finalproject.Repositories
{
    public interface IExerciseRepository : IRepositoryBase<Exercise>
    {
        Task<IEnumerable<models.ExerciseResponseDTO>> GetAllExercise(int pageNumber, int pageSize);
        Task<int> SaveAsync();
    }
}