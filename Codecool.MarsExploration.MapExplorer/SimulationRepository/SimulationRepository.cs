using Codecool.MarsExploration.MapExplorer.Exploration;
using Microsoft.Data.Sqlite;

namespace Codecool.MarsExploration.MapExplorer.SimulationRepository;

public class SimulationRepository : ISimulationRepository
{
    
    private readonly string _dbFilePath;

    public SimulationRepository(string dbFilePath)
    {
        _dbFilePath = dbFilePath;
    }
    
    private SqliteConnection GetPhysicalDbConnection()
    {
        var dbConnection = new SqliteConnection($"Data Source ={_dbFilePath};Mode=ReadWrite");
        dbConnection.Open();
        return dbConnection;
    }
    
    private void ExecuteNonQuery(string query)
    {
        using var connection = GetPhysicalDbConnection();
        using var command = GetCommand(query, connection);
        command.ExecuteNonQuery();
    }
    
    private static SqliteCommand GetCommand(string query, SqliteConnection connection)
    {
        return new SqliteCommand
        {
            CommandText = query,
            Connection = connection,
        };
    }


    public void Add(int numberOfSteps, int amountOfResources, ExplorationOutcome outcome)
    {
        var currentTime = DateTime.Now.ToString();
        var query = $"INSERT INTO simulations (time_of_the_simulation, number_of_steps, amount_of_resources_found, outcome) VALUES ('{currentTime}','{numberOfSteps}','{amountOfResources}','{outcome.ToString()}')";
        ExecuteNonQuery(query);
    }

    public void DeleteAll()
    {
        var query = "DELETE FROM simulations";
        ExecuteNonQuery(query);
        ResetId();
    }

    private void ResetId()
    {
        var resetIdQuery = "UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE NAME='simulations'";
        ExecuteNonQuery(resetIdQuery); 
    }
}