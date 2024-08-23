using Microsoft.EntityFrameworkCore;
using finalproject.models;
using MySqlConnector;
using Dapper;
using finalproject.Models;
using finalproject.DTO;

public class AppDB : DbContext
{
    readonly string connectionString = "";
    public AppDB(DbContextOptions<AppDB> options) : base(options)
    {
        var appsettingbuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        var Configuration = appsettingbuilder.Build();
        connectionString = Configuration.GetConnectionString("DefaultConnection");
    }


    public async Task<T?> GetAsync<T>(string command, object parms)
    {
        T? result;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            result = (await connection.QueryAsync<T>(command, parms).ConfigureAwait(false)).FirstOrDefault();
        }
        return result;
    }


    public async Task<List<T>> GetAll<T>(string command, object parms)
    {
        List<T> result = new List<T>();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            result = (await connection.QueryAsync<T>(command, parms)).ToList();
        }
        return result;
    }


    public async Task<int> EditData(string command, object parms)
    {
        int result;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            result = await connection.ExecuteAsync(command, parms);
        }
        return result;
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
    public DbSet<User> User { get; set; }
    public DbSet<UserLevel> UserLevel { get; set; }
    public DbSet<Exercise> Exercise { get; set; }
    public DbSet<ExerciseAssign> ExerciseAssign { get; set; }
    public DbSet<EventLog> EventLogs { get; set; }   
    public DbSet<UserLevelMenu> UserLevelMenu { get; set; }
     protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
        .HasKey(c => c.user_id);

        modelBuilder.Entity<UserLevel>()
        .HasKey(c => c.userlevel_id);

        modelBuilder.Entity<Exercise>()
        .HasKey(c => c.exercise_id);

        modelBuilder.Entity<ExerciseAssign>()
        .HasKey(c => c.exercise_assign_id);
        
        modelBuilder.Entity<EventLog>()
        .HasKey(c => new { c.log_type, c.log_datetime });

        modelBuilder.Entity<UserLevelMenu>()
       .HasKey(c => new { c.userlevel_id, c.endpoint });

       
    }

}
