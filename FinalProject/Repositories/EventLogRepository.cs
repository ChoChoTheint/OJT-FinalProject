using finalproject.Models;
using EventLog = finalproject.Models.EventLog;

namespace finalproject.Repositories
{
    public class EventLogRepository : RepositoryBase<EventLog>, IEventLogRepository
    {
        public EventLogRepository(AppDB context,IConfiguration configuration) : base(context,configuration)
        {
        }

        public async Task InsertEventLog(string controllerName, string functionname, string logMessage)
        {
            EventLog eventLog = new EventLog
            {
                log_type = (int)LogType.Insert, // 3
                log_datetime = DateTime.Now,
                log_message = "Created a new data " + logMessage,
                error_message = "",
                form_name = functionname,
                source = controllerName
            };
            Create(eventLog);
            await Save();
        }
        public async Task UpdateEventLog(string controllerName, string functionname, string logMessage)
        {
            EventLog eventLog = new EventLog
            {
                log_type = (int)LogType.Update, // 4
                log_datetime = DateTime.Now,
                log_message = "Updated data: " + logMessage,
                error_message = "",
                form_name = functionname,
                source = controllerName
            };
            Create(eventLog);
            await Save();
        }
        public async Task ErrorEventLog(string controllerName, string functionname, string logMessage, string errorMessage)
        {
            EventLog eventLog = new EventLog
            {
                log_type = (int)LogType.Error, // 2
                log_datetime = DateTime.Now,
                log_message = logMessage,
                error_message = errorMessage,
                form_name = functionname,
                source = controllerName
            };
            Create(eventLog);
            await Save();
        }
        public async Task DeleteEventLog(string controllerName, string functionname, string logMessage)
        {
            EventLog eventLog = new EventLog
            {
                log_type = (int)LogType.Delete, // 5
                log_datetime = DateTime.Now,
                log_message = "Deleted data: " + logMessage,
                error_message = "",
                form_name = functionname,
                source = controllerName
            };
            Create(eventLog);
            await Save();
        }

        Task<System.Diagnostics.EventLog?> IRepositoryBase<System.Diagnostics.EventLog>.FindByID(int id)
        {
            throw new NotImplementedException();
        }

        public void Create(System.Diagnostics.EventLog entity)
        {
            throw new NotImplementedException();
        }

        public void Update(System.Diagnostics.EventLog entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(System.Diagnostics.EventLog entity)
        {
            throw new NotImplementedException();
        }
    }
}