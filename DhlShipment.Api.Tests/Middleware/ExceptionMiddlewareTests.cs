using DhlShipment.Api.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DhlShipment.Api.Tests.Middleware;

public class ExceptionMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WithKeyNotFoundException_ReturnsNotFound()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var mockLogger = new Mock<ILogger<ExceptionMiddleware>>();
        RequestDelegate nextDelegate = context => throw new KeyNotFoundException("Shipment not found");

        var middleware = new ExceptionMiddleware(nextDelegate, mockLogger.Object);

        await middleware.InvokeAsync(httpContext);

        httpContext.Response.StatusCode.Should().Be(404);
        httpContext.Response.ContentType.Should().Be("application/json");
        
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithUnauthorizedAccessException_ReturnsUnauthorized()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var mockLogger = new Mock<ILogger<ExceptionMiddleware>>();
        RequestDelegate nextDelegate = context => throw new UnauthorizedAccessException("Access denied");

        var middleware = new ExceptionMiddleware(nextDelegate, mockLogger.Object);

        await middleware.InvokeAsync(httpContext);

        httpContext.Response.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task InvokeAsync_WithArgumentException_ReturnsBadRequest()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var mockLogger = new Mock<ILogger<ExceptionMiddleware>>();
        RequestDelegate nextDelegate = context => throw new ArgumentException("Invalid argument");

        var middleware = new ExceptionMiddleware(nextDelegate, mockLogger.Object);

        await middleware.InvokeAsync(httpContext);

        httpContext.Response.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task InvokeAsync_WithGenericException_ReturnsInternalServerError()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var mockLogger = new Mock<ILogger<ExceptionMiddleware>>();
        RequestDelegate nextDelegate = context => throw new Exception("Unexpected error");

        var middleware = new ExceptionMiddleware(nextDelegate, mockLogger.Object);

        await middleware.InvokeAsync(httpContext);

        httpContext.Response.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task InvokeAsync_WithNoException_CallsNextDelegate()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var mockLogger = new Mock<ILogger<ExceptionMiddleware>>();
        var nextDelegateCalled = false;
        
        RequestDelegate nextDelegate = context =>
        {
            nextDelegateCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new ExceptionMiddleware(nextDelegate, mockLogger.Object);

        await middleware.InvokeAsync(httpContext);

        nextDelegateCalled.Should().BeTrue();
        httpContext.Response.StatusCode.Should().NotBe(500);
    }

    [Fact]
    public async Task InvokeAsync_LogsExceptionMessage()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var mockLogger = new Mock<ILogger<ExceptionMiddleware>>();
        var exceptionMessage = "Test exception message";
        RequestDelegate nextDelegate = context => throw new Exception(exceptionMessage);

        var middleware = new ExceptionMiddleware(nextDelegate, mockLogger.Object);

        await middleware.InvokeAsync(httpContext);

        mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(exceptionMessage)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_ResponseContentTypeIsJson()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var mockLogger = new Mock<ILogger<ExceptionMiddleware>>();
        RequestDelegate nextDelegate = context => throw new KeyNotFoundException();

        var middleware = new ExceptionMiddleware(nextDelegate, mockLogger.Object);

        await middleware.InvokeAsync(httpContext);

        httpContext.Response.ContentType.Should().Be("application/json");
    }
}
