namespace SeaBattle.Server
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.SessionState;

    using Castle.Windsor;

    public class WindsorControllerFactory : IControllerFactory
    {
        private readonly IWindsorContainer _container;

        private readonly IControllerFactory _defaultFactory;

        public WindsorControllerFactory(IWindsorContainer container) : this(new DefaultControllerFactory(), container)
        {
        }

        public WindsorControllerFactory(IControllerFactory defaultFactory, IWindsorContainer container)
        {
            _defaultFactory = defaultFactory;
            _container = container;
        }

        /// <summary>Создать контейнер</summary>
        /// <param name="requestContext">Контекст запроса</param>
        /// <param name="controllerName">Наименование контроллера</param>
        /// <returns>Экземпляр контроллера</returns>
        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            var controllerComponentName = $"{controllerName}Controller";

            var controller = _container.Kernel.HasComponent(controllerComponentName)
                                 ? _container.Resolve<IController>(controllerComponentName)
                                 : _defaultFactory.CreateController(requestContext, controllerName);
            return controller;
        }

        public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return _defaultFactory.GetControllerSessionBehavior(requestContext, controllerName);
        }

        public void ReleaseController(IController controller)
        {
            _container.Release(controller);
            _defaultFactory.ReleaseController(controller);
        }
    }
}