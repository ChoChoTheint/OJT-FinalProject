using finalproject.Models;
using finalproject.Repositories;
using Microsoft.EntityFrameworkCore;

namespace finalproject.Repositories
{
    public class UserLevelMenuRepository : RepositoryBase<UserLevelMenu>, IUserLevelMenuRepository
    {
        public UserLevelMenuRepository(AppDB context,IConfiguration configuration) : base(context,configuration)
        {
        }

        public async Task<UserLevelMenu?> GetUserLevelMenu(int userLevelID, string endpoint)
        {
            var checkResult = (from userlevelmenu in _context.UserLevelMenu
                               where userlevelmenu.userlevel_id == userLevelID && userlevelmenu.endpoint == endpoint
                               select userlevelmenu).FirstOrDefaultAsync();
            return await checkResult;
        }
    }
}