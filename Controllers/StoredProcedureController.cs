using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StoredProcedureActions.Models;
using System.Data;
using System.Data.SqlClient;

namespace StoredProcedureActions.Controllers
{
    public class StoredProcedureController : Controller
    {
        private string connectionString = "Data Source=(local);Initial Catalog=SwiftFinancialsDB_Live;Persist Security Info=true; User ID=sa;Password=pass123; Pooling=True";


        public ActionResult Index()
        {
            var storedProcedures = GetStoredProcedures();
            ViewBag.StoredProcedures = new SelectList(storedProcedures);
            ViewBag.StoredProcedureCount = storedProcedures.Count;
            return View();
        }


        [HttpPost]
        public ActionResult FetchParameters(string storedProcedureName)
        {
            var parameters = new List<StoredProcedureParameter>();

            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open(); 
                    SqlCommandBuilder.DeriveParameters(command);
                    connection.Close(); 

                    foreach (SqlParameter param in command.Parameters)
                    {
                        if (param.Direction == ParameterDirection.Input || param.Direction == ParameterDirection.InputOutput)
                        {
                            parameters.Add(new StoredProcedureParameter
                            {
                                Name = param.ParameterName,
                                DataType = param.SqlDbType.ToString()
                            });
                        }
                    }
                }
            }

            return PartialView("_StoredProcedureParameters", parameters);
        }


        private List<string> GetStoredProcedures()
        {
            var storedProcedures = new List<string>();

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("SELECT SPECIFIC_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE'", connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        storedProcedures.Add(reader["SPECIFIC_NAME"].ToString());
                    }
                }
            }

            return storedProcedures;
        }

        private List<StoredProcedureParameter> GetStoredProcedureParameters(string storedProcedureName)
        {
            var parameters = new List<StoredProcedureParameter>();

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(storedProcedureName, connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlCommandBuilder.DeriveParameters(command);

                foreach (SqlParameter param in command.Parameters)
                {
                    parameters.Add(new StoredProcedureParameter
                    {
                        Name = param.ParameterName,
                        DataType = param.SqlDbType.ToString()
                    });
                }
            }

            return parameters;
        }
    }
}
