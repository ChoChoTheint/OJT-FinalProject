namespace finalproject.Repositories
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<IEnumerable<RT>> GetAll<RT>(string query, object parameters); // dapper 
        Task<int> EditData(string query, object parameters); // dapper
        Task<T?> FindByID(int id);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
       // Task Save();
      //  Task<int> SaveAsync();
        
    }
}