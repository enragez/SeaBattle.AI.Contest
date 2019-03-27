namespace SeaBattle.Server.Controllers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Services;

    public class HomeController : Controller
    {
        private readonly ApplicationContext _dbContext;

        public HomeController(IBotService botService, ApplicationContext dbContext)
        {
            _dbContext = dbContext;
            botService.SetWebhook();
        }
        
        public async Task<IActionResult> Index()
        {
            var playerStats = await _dbContext.Participants.Join(
                                                             _dbContext.Statistic,
                                                             p => p.Id,
                                                             s => s.ParticipantId,
                                                             (p, s) => new
                                                                       {
                                                                           Player = p,
                                                                           Stats = s
                                                                       })
                                       .OrderByDescending(dto => dto.Stats.Rating)
                                       .ToListAsync();
            
            var model = new PlayersStatistics
                        {
                            Stats = new List<PlayerStats>()
                        };

            for (var i = 0; i < playerStats.Count; i++)
            {
                var player = playerStats[i].Player;
                var stats = playerStats[i].Stats;
                
                model.Stats.Add(new PlayerStats
                                {
                                    Position = i + 1,
                                    Id = player.Id,
                                    Name = player.Name,
                                    Rating = stats.Rating,
                                    WinsCount = stats.Wins,
                                    GamesCount = stats.GamesPlayed,
                                    LossesCount = stats.Losses
                                });
            }
            
            return View(model);
        }

        public IActionResult About() 
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}