using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RHOnline.Library.Filters
{
    public class LoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if(context.HttpContext.Session.GetString("CPF") == null)
            {
                Controller controlador = context.Controller as Controller;
                controlador.TempData["MensagemErro"] = "Efetue o login para acessar a página";
            }

            context.Result = new RedirectToActionResult("Index", "Home", null);
        }

    }
}
