namespace SeaBattle.Server
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Services;

    public class MvcApplication : HttpApplication
    {
        public IWindsorContainer Container { get; set; }
        
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            InitIoc();

            Container.Resolve<IBotService>().SetWebhook();
        }

        private void InitIoc()
        {
            Container = new WindsorContainer();

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(Container));
            
            Container.Register(Component.For<IWindsorContainer>().Instance(Container));
            
            Container.Register(Component.For<IBotService>().ImplementedBy<BotService>().LifestyleSingleton(),
                               Component.For<IUpdateService>().ImplementedBy<UpdateService>().LifestyleTransient());

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var controllerTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Controller)));

                foreach (var controllerType in controllerTypes)
                {
                    if (Container.Kernel.HasComponent(controllerType.Name))
                    {
                        continue;
                    }
                    
                    Container.Register(Component.For(typeof(Controller)).ImplementedBy(controllerType)
                                             .Named(controllerType.Name).LifestyleTransient());
                }
            }
        }
    }
}