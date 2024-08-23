using finalproject.DTO;
using finalproject.models;


namespace finalproject.Repositories
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        Task<GetUserResponseDTO?> GetUser(int id,string key);
        Task<IEnumerable<User>> GetAllStudent();
        Task<User?> GetName(string name);
        Task Save();
    }
}