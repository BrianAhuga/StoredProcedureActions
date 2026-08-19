using System.Data;
using System.Data.SqlClient;
using StoredProcedureActions.Models;

namespace StoredProcedureActions.Services
{
    public class StoredProcedureService : IStoredProcedureService
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public StoredProcedureService(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection") ?? "Data Source=(local);Initial Catalog=NORTHWND;Persist Security Info=true; User ID=sa;Password=pass123; Pooling=True";
        }

        public async Task<List<string>> ListStoredProceduresAsync()
        {
            var list = new List<string>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT SPECIFIC_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE'", conn);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(reader.GetString(0));
            }
            return list;
        }

        public Task<List<StoredProcedureParameter>> GetParametersAsync(string storedProcedureName)
        {
            var parameters = new List<StoredProcedureParameter>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(storedProcedureName, connection);
            command.CommandType = CommandType.StoredProcedure;

            connection.Open();
            SqlCommandBuilder.DeriveParameters(command);

            foreach (SqlParameter param in command.Parameters)
            {
                parameters.Add(new StoredProcedureParameter
                {
                    Name = param.ParameterName,
                    DataType = param.SqlDbType.ToString()
                });
            }

            return Task.FromResult(parameters);
        }

        public Task<ExecutionResult> ExecuteStoredProcedureAsync(string storedProcedureName, Dictionary<string, string> parameterValues)
        {
            var resultTable = new DataTable();
            var outputParams = new Dictionary<string, object>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(storedProcedureName, connection);
            command.CommandType = CommandType.StoredProcedure;

            connection.Open();
            SqlCommandBuilder.DeriveParameters(command);

            foreach (SqlParameter param in command.Parameters)
            {
                if (param.Direction == ParameterDirection.Input || param.Direction == ParameterDirection.InputOutput)
                {
                    string value = null;
                    if (parameterValues != null)
                    {
                        parameterValues.TryGetValue(param.ParameterName, out value);
                        if (value == null)
                        {
                            parameterValues.TryGetValue(param.ParameterName.TrimStart('@'), out value);
                        }
                    }

                    param.Value = string.IsNullOrWhiteSpace(value) ? DBNull.Value : (object)value;
                }

                if ((param.Direction == ParameterDirection.Output || param.Direction == ParameterDirection.InputOutput) && param.Size == 0)
                {
                    param.Size = 4000;
                }
            }

            using (var adapter = new SqlDataAdapter(command))
            {
                adapter.Fill(resultTable);
            }

            foreach (SqlParameter param in command.Parameters)
            {
                if (param.Direction == ParameterDirection.Output || param.Direction == ParameterDirection.InputOutput)
                {
                    outputParams[param.ParameterName] = param.Value ?? DBNull.Value;
                }
            }

            connection.Close();

            var model = new ExecutionResult
            {
                Table = resultTable,
                OutputParameters = outputParams
            };

            return Task.FromResult(model);
        }
    }
}
