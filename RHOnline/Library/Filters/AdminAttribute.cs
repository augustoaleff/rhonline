using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RHOnline.Library.Filters
{
    public class AdminAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {

            int nivel = context.HttpContext.Session.GetInt32("Nivel") ?? 2;
            

            //Somente Nivel 1 = Administrador ou Nivel 3 = Suporte (TI) podem acessar a página
            //Nível 2 = Usuario Comum, não pode
            if(nivel != 1 && nivel != 3)
            {
                if(context.Controller != null)
                {
                    Controller controlador = context.Controller as Controller;
                    controlador.TempData["MensagemErroInicio"] = "Você não tem permissão para acessar esta página";
                }
                
                context.Result = new RedirectToActionResult("Inicio", "Home", null);

            }
        }
    }
}
