namespace StoredProcedureActions.Models
{
    public class StoredProcedureViewModel
    {
        public string StoredProcedureName { get; set; }
        public List<StoredProcedureParameter> Parameters { get; set; }
    }
}
