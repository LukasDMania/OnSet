using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using OnSet.Application.Exceptions;

namespace OnSet.Infrastructure.Filters;

/// <summary>
/// Maps domain exceptions to user-facing pages and correct HTTP status codes instead of a generic 500.
/// </summary>
public sealed class DomainExceptionFilter : IExceptionFilter
{
    public const string DomainRuleMessageKey = "DomainRuleMessage";

    private readonly ILogger<DomainExceptionFilter> _logger;
    private readonly IWebHostEnvironment _env;

    public DomainExceptionFilter(ILogger<DomainExceptionFilter> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case NotFoundException ex:
                _logger.LogInformation(ex, "Resource not found.");
                context.Result = new RedirectToPageResult("/NotFound");
                context.ExceptionHandled = true;
                break;

            case ForbiddenAccessException ex:
                _logger.LogInformation(ex, "Forbidden.");
                context.Result = new RedirectToPageResult("/Forbidden");
                context.ExceptionHandled = true;
                break;

            case DomainRuleException ex:
                _logger.LogWarning(ex, "Domain rule violated.");
                if (_env.IsDevelopment())
                {
                    var tempData = context.HttpContext.RequestServices
                        .GetRequiredService<ITempDataDictionaryFactory>()
                        .GetTempData(context.HttpContext);
                    tempData[DomainRuleMessageKey] = ex.Message;
                }

                context.Result = new RedirectToPageResult("/BadRequest");
                context.ExceptionHandled = true;
                break;
        }
    }
}
