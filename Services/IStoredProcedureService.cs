using StoredProcedureActions.Models;
namespace StoredProcedureActions.Services
{
    public interface IStoredProcedureService
    {
        Task<List<string>> ListStoredProceduresAsync();
        Task<List<StoredProcedureParameter>> GetParametersAsync(string storedProcedureName);
        Task<ExecutionResult> ExecuteStoredProcedureAsync(string storedProcedureName, Dictionary<string, string> parameterValues);
    }
}
