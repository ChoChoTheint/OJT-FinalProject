using System.Diagnostics;
namespace finalproject.Repositories
{
    public interface IEventLogRepository : IRepositoryBase<EventLog>
    {
        Task InsertEventLog(string controllerName, string functionname, string logMessage);
        Task UpdateEventLog(string controllerName, string functionname, string logMessage);
        Task ErrorEventLog(string controllerName, string functionname, string logMessage, string errorMessage);
        Task DeleteEventLog(string controllerName, string functionname, string logMessage);
    }
}