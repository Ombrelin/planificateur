using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Planificateur.Core.Exceptions;

namespace Planificateur.Web.Middlewares;

public class ExceptionMiddleware : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = new ContentResult
        {
            StatusCode = GetStatusCode(context.Exception),
            Content = context.Exception.Message
        };
    }

    private int GetStatusCode(Exception contextException)
    {
        return contextException switch
        {
            NotFoundException => 404,
            ArgumentException => 400,
            IllegalAccessException => 403,
            _ => 500
        };
    }
}