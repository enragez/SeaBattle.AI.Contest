namespace SeaBattle.Server
{
    using Commands;
    using Config;
    using Dal;
    using FluentScheduler;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Scheduling;
    using Services;
    using Services.Compile;
    using Services.Rating;
    using Services.Registration;
    using Services.Stats;
    using Services.Updates;
    using StateMachine;
    using StateMachine.Registration;
    using StateMachine.UpdateName;
    using StateMachine.UpdateStrategy;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
                                                    {
                                                        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                                                        options.CheckConsentNeeded = context => true;
                                                        options.MinimumSameSitePolicy = SameSiteMode.None;
                                                    });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(Configuration.GetConnectionString("ApplicationContext")));
            
            services.AddSingleton<ICommand, RegisterCommand>();
            services.AddSingleton<ICommand, UpdateNameCommand>();
            services.AddSingleton<ICommand, UpdateStrategyCommand>();
            services.AddSingleton<ICommand, MyStatsCommand>();
            services.AddSingleton<ICommand, PlayersCommand>();
            services.AddSingleton<ICommand, PlayerStatsCommand>();
            services.AddSingleton<ICommand, DuelCommand>();
            services.AddSingleton<ICommand, MyLast20GamesCommand>();
            
            services.AddSingleton<IBotUpdateHandler, BotUpdateHandler>();
            services.AddSingleton<IBotService, BotService>();
            services.AddSingleton<IStatisticsService, StatisticsService>();
            services.AddSingleton<IStrategyCompiler, StrategyCompiler>();
            services.AddSingleton<IEloRatingCalculator, EloRatingCalculator>();
            
            services.AddTransient<IGameRunner, GameRunner>();
            
            services.AddSingleton<RegisterService>();
            services.AddSingleton<UpdateStrategyService>();
            services.AddSingleton<UpdateNameService>();
            
            services.AddSingleton<IServiceWithState>(x => x.GetRequiredService<RegisterService>());
            services.AddSingleton<IServiceWithState>(x => x.GetRequiredService<UpdateStrategyService>());
            services.AddSingleton<IServiceWithState>(x => x.GetRequiredService<UpdateNameService>());
            
            services.AddSingleton<IServiceWithState<RegistrationState>>(x => x.GetRequiredService<RegisterService>());
            services.AddSingleton<IServiceWithState<UpdateStrategyState>>(x => x.GetRequiredService<UpdateStrategyService>());
            services.AddSingleton<IServiceWithState<UpdateNameState>>(x => x.GetRequiredService<UpdateNameService>());
            
            services.AddTransient<IStateMachine<RegistrationState>, RegistrationStateMachine>();
            services.AddTransient<IStateMachine<UpdateStrategyState>, UpdateStrategyStateMachine>();
            services.AddTransient<IStateMachine<UpdateNameState>, UpdateNameStateMachine>();

            services.Configure<BotConfiguration>(Configuration.GetSection("BotConfiguration"));
            services.Configure<EloConfiguration>(Configuration.GetSection("EloConfiguration"));

            var gamesConfig = Configuration.GetSection("GamesConfiguration");
            
            var gamesConfigModel = new GamesConfiguration
                                   {
                                       Enabled = bool.TryParse(gamesConfig["Enabled"], out var enabled) && enabled,
                                       StartNow = bool.TryParse(gamesConfig["StartNow"], out var startNow) && startNow,
                                       Interval = int.TryParse(gamesConfig["Interval"], out var interval)
                                            ? interval
                                            : 15
                                   };
            
            JobManager.Initialize(new JobsRegistry(services.BuildServiceProvider(), gamesConfigModel));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Utils.Utils.DebugMode = env.IsDevelopment();
            
            app.Use((context, next) =>
                    {
                        if (Utils.Utils.CurrentApplicationUrl == null)
                        {
                            Utils.Utils.CurrentApplicationUrl = context.Request.GetDisplayUrl();
                        }
                        
                        return next.Invoke();
                    });
            
            app.UseMvc();

            app.UseStaticFiles();

            app.UseMvc(routes =>
                       {
                           routes.MapRoute(
                                           name: "game",
                                           template: "game/get/{id}",
                                           defaults: new {controller = "Game", action = "Get"});
                           routes.MapRoute(
                                           name: "gameTurn",
                                           template: "game/get/{id}/turn/{turn}",
                                           defaults: new {controller = "Game", action = "GetTurn"});
                           
                           routes.MapRoute(
                                           name: "default",
                                           template: "{controller=Home}/{action=Index}/{id?}");
                       });
        }
    }
}