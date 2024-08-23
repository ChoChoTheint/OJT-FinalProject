using finalproject.Repositories;


namespace finalproject.Repositories
{
    public class RepositoryWrapper(AppDB context,IConfiguration configuration) : IRepositoryWrapper
    {
        readonly AppDB _context = context;
        readonly IConfiguration _configuration = configuration;
        
        
        
        private IEventLogRepository? _eventLog;
        public IEventLogRepository EventLog
        {
            get
            {
                _eventLog ??= new EventLogRepository(_context,_configuration);
                return _eventLog;
            }
        }

        private IUserRepository? _user;
        public IUserRepository User
        {
            get
            {
                _user ??= new UserRepository(_context,_configuration);
                return _user;
            }
        }

        private IExerciseRepository? _exercise;
        public IExerciseRepository Exercise
        {
            get
            {
                _exercise ??= new ExerciseRepository(_context,configuration);
                return _exercise;
            }
        }

        private IExerciseAssignRepository? _exerciseAssign;
        public IExerciseAssignRepository ExerciseAssign
        {
            get
            {
                _exerciseAssign ??= new ExerciseAssignRepository(_context,_configuration);
                return _exerciseAssign;
            }
        }

        private IUserLevelMenuRepository? _userLevelMenu;
        public IUserLevelMenuRepository UserLevelMenu
        {
            get
            {
                _userLevelMenu ??= new UserLevelMenuRepository(_context,_configuration);
                return _userLevelMenu;
            }
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}