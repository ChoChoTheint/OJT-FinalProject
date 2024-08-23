using finalproject.DTO;
using finalproject.models;


namespace finalproject.Repositories
{
    public interface IExerciseAssignRepository : IRepositoryBase<ExerciseAssign>
    {
        Task<IEnumerable<StudentDetailResponseDTO>> GetStudedentDetail();
        Task Save();
        Task<int> SaveAsync();
        Task<IEnumerable<StudentDetailResponseDTO>> GetStudent(int id);
    }
}