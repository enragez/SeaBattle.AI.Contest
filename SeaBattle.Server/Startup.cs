namespace SeaBattle.Server
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Models;
    using Models.Commands;
    using Services;
    using Services.Stats;
    using Services.UpdateNameService;
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
            
            services.AddSingleton<IUpdateHandler, UpdateHandler>();
            services.AddSingleton<IBotService, BotService>();
            services.AddSingleton<IStatisticsService, StatisticsService>();
            services.AddSingleton<IGameRunner, GameRunner>();
            
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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