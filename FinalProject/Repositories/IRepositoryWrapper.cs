

namespace finalproject.Repositories
{
    public interface IRepositoryWrapper
    {
        IEventLogRepository EventLog { get; }
        IUserRepository User { get; }
        IUserLevelMenuRepository UserLevelMenu { get; }
        IExerciseRepository Exercise{get;}
        IExerciseAssignRepository ExerciseAssign{get;}
        Task<int> SaveAsync();
    }
}