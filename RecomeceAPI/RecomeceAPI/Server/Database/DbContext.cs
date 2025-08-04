using MySql.Data.MySqlClient;
using System.Data;
using Microsoft.Extensions.Logging;
using System.Text;
using RecomeceAPI.Interfaces;

namespace RecomeceAPI.Server.Database
{
  public class DbContext : IDisposable
  {
    private readonly ConnectDB _connectDB;
    private readonly ILogger<DbContext> _logger;
    private readonly IFileService _fileService;
    public Parameters Parametros { get; set; }

    private readonly MySqlConnection _connection;

    public DbContext(ConnectDB connectDB, ILogger<DbContext> logger, IFileService fileService)
    {
      _connectDB = connectDB;
      _logger = logger;
      _fileService = fileService;
      Parametros = new Parameters();
      _connection = _connectDB.GetCon();
    }

    private MySqlCommand CreateCommand(string commandText, int cmdType)
    {
      var cmd = new MySqlCommand(commandText, _connection)
      {
        CommandTimeout = 30,
        CommandType = cmdType == 1 ? CommandType.StoredProcedure : CommandType.Text
      };

      foreach (var param in Parametros.ParametersList)
        cmd.Parameters.AddWithValue(param.Field, param.Value);

      return cmd;
    }

    public async Task ExecAsync(string cmdStr, int cmdType)
    {
      try
      {
        if (_connection.State != ConnectionState.Open)
          await _connection.OpenAsync();

        using var cmd = CreateCommand(cmdStr, cmdType);
        await cmd.ExecuteNonQueryAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error executing command on database");
        HandleCatch(ex, cmdStr);
        throw;
      }
      finally
      {
        await _connection.CloseAsync();
      }
    }

    public async Task<long> SaveAsync(string cmdStr, int cmdType)
    {
      try
      {
        if (_connection.State != ConnectionState.Open)
          await _connection.OpenAsync();

        using var cmd = CreateCommand(cmdStr, cmdType);
        var result = await cmd.ExecuteScalarAsync();

        if (result == null || result == DBNull.Value)
          return 0;

        return Convert.ToInt64(result);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error saving data to database");
        HandleCatch(ex, cmdStr);
        throw;
      }
      finally
      {
        await _connection.CloseAsync();
      }
    }

    public async Task<DataSet> QueryAsync(string cmdStr, int cmdType)
    {
      try
      {
        if (_connection.State != ConnectionState.Open)
          await _connection.OpenAsync();

        using var cmd = CreateCommand(cmdStr, cmdType);
        using var da = new MySqlDataAdapter(cmd);

        var ds = new DataSet();
        await Task.Run(() => da.Fill(ds));

        return ds;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error querying data from database");
        HandleCatch(ex, cmdStr);
        throw;
      }
      finally
      {
        await _connection.CloseAsync();
      }
    }

    public void HandleCatch(Exception ex, string cmdStr)
    {
      var sb = new StringBuilder();

      sb.AppendLine("Servidor: " + _connectDB.Servidor);
      sb.AppendLine("Banco de Dados: " + _connectDB.Banco);
      sb.AppendLine("CMD: " + cmdStr);
      sb.AppendLine("InnerException: " + (ex.InnerException?.ToString() ?? "N/A"));
      sb.AppendLine("Parâmetros:");
      sb.AppendLine(GetStringParameters().Replace(",", ",\r\n"));
      sb.AppendLine("\r\nMensagem: " + ex.Message);
      sb.AppendLine("\r\nStack Trace:\r\n" + ex.StackTrace);

      string logMessage = sb.ToString();

      _fileService.CreateFile($"{Configs.RootFiles()}/Erros/",
          $"Erro_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt", logMessage);
    }

    public string GetStringParameters()
    {
      var sb = new StringBuilder();

      foreach (var param in Parametros.ParametersList)
      {
        sb.AppendLine($"{param.Field} = {param.Value},");
      }

      return sb.ToString();
    }

    public void Dispose()
    {
      _connection?.Dispose();
    }
  }
}
