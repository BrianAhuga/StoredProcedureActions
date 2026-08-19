using System.Data;
namespace StoredProcedureActions.Models
{
    public class ExecutionResult
    {
        public DataTable Table { get; set; }
        public Dictionary<string, object> OutputParameters { get; set; }
    }
}
