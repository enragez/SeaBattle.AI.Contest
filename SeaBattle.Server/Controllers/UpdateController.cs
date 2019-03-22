namespace SeaBattle.Server.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Services;
    using Telegram.Bot.Types;

    [Route("api/[controller]")]
    public class UpdateController : Controller
    {
        private readonly IUpdateHandler _updateHandler;

        public UpdateController(IUpdateHandler updateHandler)
        {
            _updateHandler = updateHandler;
        }

        // POST api/update
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            await _updateHandler.Handle(update);
            return Ok();
        }
    }
}