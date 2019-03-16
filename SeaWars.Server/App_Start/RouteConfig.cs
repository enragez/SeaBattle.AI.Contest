﻿using System.Web.Mvc;
using System.Web.Routing;

namespace SeaWars.Server
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("game",
                            "game/get/{id}",
                            new {controller = "Game", action = "Get"});

            routes.MapRoute("gameTurn",
                            "game/get/{id}/turn/{turn}",
                            new
                            {
                                controller = "Game",
                                action = "GetTurn"
                            });
            routes.MapRoute(
                            "Default",
                            "{controller}/{action}/{id}",
                            new {controller = "Home", action = "Index", id = UrlParameter.Optional}
                           );
        }
    }
}