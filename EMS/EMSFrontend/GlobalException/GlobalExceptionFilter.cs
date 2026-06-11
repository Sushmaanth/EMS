using EMSFrontend.Api.ApiException;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace EMSFrontend.GlobalException
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var action = context.RouteData.Values["action"]?.ToString();

            if (context.Exception is ApiRequestException apiRequestException)
            {
                if (action == "MicrosoftResponse")
                {
                    var tempDataFactory = context.HttpContext.RequestServices.GetService<ITempDataDictionaryFactory>();

                    var tempData = tempDataFactory?.GetTempData(context.HttpContext);

                    tempData["MicrosoftLoginError"] = "Access denied. Your Microsoft account is not registered in the EMS system.\r\nPlease contact the administrator for access.";

                    context.Result = new RedirectToActionResult("Login", "Auth", null);

                    context.ExceptionHandled = true;
                    return;
                }

                if (apiRequestException.StatusCode == 401)
                {
                    context.ModelState.AddModelError("Password", "Invalid Email or Password");

                    context.Result = new ViewResult
                    {
                        ViewName = context.RouteData.Values["action"]?.ToString(),

                        ViewData = new ViewDataDictionary(
                                   new EmptyModelMetadataProvider(),
                                   context.ModelState)
                    };

                    context.ExceptionHandled = true;
                    return;
                }

                else if (apiRequestException.StatusCode == 400)
                {
                    if (apiRequestException.Message.Contains(
                        "Employee email not found"))
                    {
                        context.ModelState.AddModelError("EmailId", "Email not found");
                    }

                    else if (apiRequestException.Message.Contains("Account already activated"))
                    {
                        context.ModelState.AddModelError("EmailId", "Account already activated");
                    }
                    else
                    {
                        context.ModelState.AddModelError("", apiRequestException.Message);
                    }


                    context.Result =
                             new ViewResult
                             {
                                 ViewName = context.RouteData.Values["action"]?.ToString(),

                                 ViewData =
                                 new ViewDataDictionary(
                                     new EmptyModelMetadataProvider(),
                                     context.ModelState)
                             };

                    context.ExceptionHandled = true;
                    return;
                }
            }

            if (context.Exception is UnauthorizedException)
            {
                context.HttpContext.Session.Clear();

                var tempDataFactory =
                    context.HttpContext.RequestServices
                    .GetService<ITempDataDictionaryFactory>();

                var tempData =
                    tempDataFactory?.GetTempData(context.HttpContext);

                tempData["SessionExpired"] =
                    "Your session has expired. Please login again.";

                context.Result =
                    new RedirectToActionResult(
                        "Login",
                        "Auth",
                        null);

                context.ExceptionHandled = true;
                return;
            }


            context.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/ServerError.cshtml"
            };

            context.ExceptionHandled = true;
        }
    }
}

