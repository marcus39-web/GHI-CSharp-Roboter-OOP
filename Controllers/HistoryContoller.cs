using Microsoft.AspNetCore.Mvc;
using GHI_CSharp_Roboter_OOP.Models;

namespace GHI_CSharp_Roboter_OOP.Controllers
{
    [ApiController]
    [Route("api/history")]
    public class HistoryController : ControllerBase
    {
        private readonly CategorizationDatabase _db;
    public HistoryController(CategorizationDatabase db) => _db = db;

    [HttpGet]
    public IActionResult GetHistory([FromQuery] string? period)
    {
        var data = _db.GetHistory(50);
        return Ok(data);
    }
}
   }