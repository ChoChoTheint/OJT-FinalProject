using Dapper;
using Microsoft.EntityFrameworkCore;
using finalproject.DTO;
using finalproject.models;


namespace finalproject.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(AppDB context,IConfiguration configuration) : base(context,configuration)
        {
        }
        public async Task<GetUserResponseDTO?> GetUser(int id,string key)
        {
            
            int userID = id;
            string ecbKey = key;

            var mainQuery = from user in _context.User
                            join userLevel in _context.UserLevel on user.userlevel_id equals userLevel.userlevel_id
                            where user.user_id == userID
                            select new GetUserResponseDTO
                            {
                                UserID = Encryption.AES_Encrypt_ECB_128(user.user_id.ToString(), ecbKey),
                                UserName = user.name,
                                Password = user.password,
                                IsLock = user.is_lock.ToString(),
                                FailCount = user.login_fail_count.ToString(),
                                UserLevel = userLevel.userlevel_name,
                                Salt = user.salt
                            };

            return await mainQuery.FirstOrDefaultAsync();

        }

        public async Task<User?> GetName(string name)
        {
            var oldUser=_context.User.Where(a=>a.name==name).FirstOrDefault();
            return oldUser;
        }

        public async Task<IEnumerable<User>> GetAllStudent()
        {
             var mainQuery = from user in _context.User
                    join userLevel in _context.UserLevel on user.userlevel_id equals userLevel.userlevel_id
                    where userLevel.userlevel_name == "student"
                    select new User
                    {
                        user_id = user.user_id,
                        name = user.name,
                        password = user.password,
                        salt = user.salt,
                        login_fail_count =user.login_fail_count,
                        is_lock = user.is_lock,
                        userlevel_id = user.userlevel_id,
                        userlevel = new UserLevel 
                        {
                            userlevel_id = userLevel.userlevel_id,
                            userlevel_name = userLevel.userlevel_name
                        }
                    };

            return await mainQuery.ToListAsync();
        }

    }
}