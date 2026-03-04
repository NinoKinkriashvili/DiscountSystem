using System.Net;
using DiscountsSystem.Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DiscountsSystem.Api.Middleware;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // Client cancelled the request (browser closed, etc.)
            _logger.LogInformation("Request cancelled by client. Path: {Path}", context.Request.Path);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogError(ex, "Unhandled exception after response started. Path: {Path}", context.Request.Path);
                throw;
            }

            _logger.LogError(ex, "Unhandled exception. Path: {Path}", context.Request.Path);

            var result = MapException(context, ex);

            context.Response.Clear();
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = result.StatusCode;

            await context.Response.WriteAsJsonAsync(result.Problem, context.RequestAborted);
        }
    }

    private (int StatusCode, ProblemDetails Problem) MapException(HttpContext context, Exception ex)
    {
        // 1) FluentValidation -> 400 + errors
        if (ex is ValidationException vex)
        {
            var errors = vex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());

            var problem = new ValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Failed",
                Detail = _env.IsDevelopment() ? ex.Message : "One or more validation errors occurred.",
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] = context.TraceIdentifier;

            return (problem.Status.Value, problem);
        }

        // 2) Auth / Access
        if (ex is UnauthorizedAccessException)
        {
            var isAuth = context.User?.Identity?.IsAuthenticated == true;
            var status = isAuth ? StatusCodes.Status403Forbidden : StatusCodes.Status401Unauthorized;

            return (status, BuildProblem(context, status, isAuth ? "Forbidden" : "Unauthorized", ex));
        }

        // 3) Business conflict
        if (ex is ConflictException)
            return (StatusCodes.Status409Conflict, BuildProblem(context, StatusCodes.Status409Conflict, "Conflict", ex));

        // 4) EF Concurrency -> 409
        if (ex is DbUpdateConcurrencyException)
            return (StatusCodes.Status409Conflict, BuildProblem(context, StatusCodes.Status409Conflict, "Conflict", ex));

        // 5) EF DbUpdateException -> 409 only for unique/duplicate, else 500
        if (ex is DbUpdateException dbex)
        {
            if (IsUniqueConstraintViolation(dbex))
                return (StatusCodes.Status409Conflict, BuildProblem(context, StatusCodes.Status409Conflict, "Conflict", ex));

            return (StatusCodes.Status500InternalServerError,
                BuildProblem(context, StatusCodes.Status500InternalServerError, "Internal Server Error", ex));
        }

        // 6) Bad request
        if (ex is ArgumentException or InvalidOperationException)
            return (StatusCodes.Status400BadRequest, BuildProblem(context, StatusCodes.Status400BadRequest, "Bad Request", ex));

        // fallback
        return (StatusCodes.Status500InternalServerError,
            BuildProblem(context, StatusCodes.Status500InternalServerError, "Internal Server Error", ex));
    }

    private ProblemDetails BuildProblem(HttpContext context, int status, string title, Exception ex)
    {
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = _env.IsDevelopment() ? ex.Message : "An unexpected error occurred.",
            Instance = context.Request.Path
        };

        problem.Extensions["traceId"] = context.TraceIdentifier;
        return problem;
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        var sqlEx = ex.InnerException as SqlException
                    ?? ex.InnerException?.InnerException as SqlException;

        if (sqlEx is null) return false;

        return sqlEx.Number is 2601 or 2627;
    }
}
