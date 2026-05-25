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
            if (context.Exception is ApiRequestException apiRequestException)
            {
                if (apiRequestException.StatusCode == 401)
                {
                    context.ModelState.AddModelError("Password", "Invalid Email or Password");
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
                }

                if (context.Exception is UnauthorizedException)
                {
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
}

