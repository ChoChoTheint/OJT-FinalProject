using finalproject.Models;
using finalproject.Repositories;

namespace finalproject.Repositories
{
    public interface IUserLevelMenuRepository : IRepositoryBase<UserLevelMenu>
    {
        Task<UserLevelMenu?> GetUserLevelMenu(int userLevelID, string endpoint);
    }
}