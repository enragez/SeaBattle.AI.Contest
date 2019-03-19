namespace SeaBattle.Server.Controllers
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Newtonsoft.Json;
    using Services;
    using Telegram.Bot.Types;
    
    public class UpdateController : Controller
    {
        public IUpdateService UpdateService { get; set; }

        // POST api/update
        [HttpPost]
        public async Task<ActionResult> Post()
        {
            var req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            var json = new StreamReader(req).ReadToEnd();

            try
            {
                var update = JsonConvert.DeserializeObject<Update>(json);
                await UpdateService.EchoAsync(update);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
    }
}