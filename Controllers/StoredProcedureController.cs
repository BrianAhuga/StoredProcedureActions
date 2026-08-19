using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StoredProcedureActions.Models;
using StoredProcedureActions.Services;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace StoredProcedureActions.Controllers
{
    public class StoredProcedureController : Controller
    {
        private readonly IStoredProcedureService _spService;

        public StoredProcedureController(IStoredProcedureService spService)
        {
            _spService = spService;
        }

        public async Task<IActionResult> Index()
        {
            var storedProcedures = await _spService.ListStoredProceduresAsync();
            ViewBag.StoredProcedures = new SelectList(storedProcedures);
            ViewBag.StoredProcedureCount = storedProcedures.Count;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FetchParameters(string storedProcedureName)
        {
            var parameters = await _spService.GetParametersAsync(storedProcedureName);
            return PartialView("_StoredProcedureParameters", parameters);
        }

        [HttpPost]
        public async Task<IActionResult> Execute(string storedProcedureName)
        {
            var form = Request.Form;
            var paramValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var key in form.Keys)
            {
                var k = key.ToString();
                if (k.StartsWith("paramValues[") && k.EndsWith("]"))
                {
                    var paramName = k.Substring("paramValues[".Length, k.Length - "paramValues[]".Length);
                    paramValues[paramName] = form[k];
                }
            }

            var result = await _spService.ExecuteStoredProcedureAsync(storedProcedureName, paramValues);
            return PartialView("_ExecutionResult", result);
        }
    }
}
