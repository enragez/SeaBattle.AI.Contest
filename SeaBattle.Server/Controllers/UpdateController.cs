namespace SeaBattle.Server.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Services;
    using Telegram.Bot.Types;

    [Route("api/[controller]")]
    public class UpdateController : Controller
    {
        private readonly IBotUpdateHandler _botUpdateHandler;

        public UpdateController(IBotUpdateHandler botUpdateHandler)
        {
            _botUpdateHandler = botUpdateHandler;
        }

        // POST api/update
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            await _botUpdateHandler.Handle(update);
            return Ok();
        }
    }
}