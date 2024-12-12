using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransrodenProyecto.Models;
using TransrodenProyecto.ViewModels;

namespace TransrodenProyecto.Security
{
    public class AdminOnlyAttribute : AuthorizeAttribute
    {

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["UsuarioId"] == null || httpContext.Session["UsuarioRol"] == null)
            {
                return false;
            }

            var usuarioRol = (Rol)httpContext.Session["UsuarioRol"];


            if (usuarioRol == Rol.Administrador)
            {
                return true;
            }
            return false;
        }


        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new System.Web.Routing.RouteValueDictionary(
                    new { controller = "Cuenta", action = "Login" }));
        }

    }
}