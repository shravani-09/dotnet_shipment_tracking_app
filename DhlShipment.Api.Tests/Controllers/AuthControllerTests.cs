using DhlShipment.Api.Controllers;
using DhlShipment.Api.Models.Auth;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using Xunit;

namespace DhlShipment.Api.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        
        _mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("THIS_IS_A_SUPER_SECRET_KEY_12345");
        _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("DhlShipment.Api");
        _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("DhlShipment.Client");

        _controller = new AuthController(_mockConfiguration.Object);
    }

    #region Login Tests

    [Fact]
    public void Login_WithValidAdminCredentials_ReturnsOkWithToken()
    {
        var loginRequest = new LoginRequest
        {
            Email = "admin@dhl.com",
            Password = "admin123"
        };

        var result = _controller.Login(loginRequest);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        
        var response = okResult.Value.Should().BeOfType<LoginResponse>().Subject;
        response.Token.Should().NotBeNullOrEmpty();
        response.Role.Should().Be("Admin");
    }

    [Fact]
    public void Login_WithValidUserCredentials_ReturnsOkWithToken()
    {
        var loginRequest = new LoginRequest
        {
            Email = "user@dhl.com",
            Password = "user123"
        };

        var result = _controller.Login(loginRequest);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        
        var response = okResult.Value.Should().BeOfType<LoginResponse>().Subject;
        response.Token.Should().NotBeNullOrEmpty();
        response.Role.Should().Be("User");
    }

    [Fact]
    public void Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        var loginRequest = new LoginRequest
        {
            Email = "wrong@dhl.com",
            Password = "wrongpassword"
        };

        var result = _controller.Login(loginRequest);

        var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
        unauthorizedResult.StatusCode.Should().Be(401);
        unauthorizedResult.Value.Should().Be("Invalid credentials");
    }

    [Fact]
    public void Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var loginRequest = new LoginRequest
        {
            Email = "admin@dhl.com",
            Password = "wrongpassword"
        };

        var result = _controller.Login(loginRequest);

        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public void Login_WithWrongEmail_ReturnsUnauthorized()
    {
        var loginRequest = new LoginRequest
        {
            Email = "wrong@dhl.com",
            Password = "admin123"
        };

        var result = _controller.Login(loginRequest);

        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public void Login_GeneratesValidJwtToken_AdminRole()
    {
        var loginRequest = new LoginRequest
        {
            Email = "admin@dhl.com",
            Password = "admin123"
        };

        var result = _controller.Login(loginRequest);
        var okResult = (OkObjectResult)result;
        var response = (LoginResponse)okResult.Value!;

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadToken(response.Token) as JwtSecurityToken;
        
        token.Should().NotBeNull();
        token!.Claims.Should().Contain(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "Admin");
    }

    [Fact]
    public void Login_GeneratesValidJwtToken_UserRole()
    {
        var loginRequest = new LoginRequest
        {
            Email = "user@dhl.com",
            Password = "user123"
        };

        var result = _controller.Login(loginRequest);
        var okResult = (OkObjectResult)result;
        var response = (LoginResponse)okResult.Value!;

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadToken(response.Token) as JwtSecurityToken;
        
        token.Should().NotBeNull();
        token!.Claims.Should().Contain(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "User");
    }

    [Fact]
    public void Login_TokenContainsIssuer()
    {
        var loginRequest = new LoginRequest
        {
            Email = "admin@dhl.com",
            Password = "admin123"
        };

        var result = _controller.Login(loginRequest);
        var okResult = (OkObjectResult)result;
        var response = (LoginResponse)okResult.Value!;

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadToken(response.Token) as JwtSecurityToken;
        
        token!.Issuer.Should().Be("DhlShipment.Api");
    }

    [Fact]
    public void Login_TokenContainsAudience()
    {
        var loginRequest = new LoginRequest
        {
            Email = "user@dhl.com",
            Password = "user123"
        };

        var result = _controller.Login(loginRequest);
        var okResult = (OkObjectResult)result;
        var response = (LoginResponse)okResult.Value!;

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadToken(response.Token) as JwtSecurityToken;
        
        token!.Audiences.Should().Contain("DhlShipment.Client");
    }

    [Fact]
    public void Login_TokenHasExpirationTime()
    {
        var loginRequest = new LoginRequest
        {
            Email = "admin@dhl.com",
            Password = "admin123"
        };

        var result = _controller.Login(loginRequest);
        var okResult = (OkObjectResult)result;
        var response = (LoginResponse)okResult.Value!;

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadToken(response.Token) as JwtSecurityToken;
        
        token!.ValidTo.Should().BeAfter(DateTime.UtcNow);
        var expiryInMinutes = (token.ValidTo - DateTime.UtcNow).TotalMinutes;
        expiryInMinutes.Should().BeGreaterThan(59).And.BeLessThan(61);
    }

    [Fact]
    public void Login_DifferentUsersShouldGetDifferentTokens()
    {
        var adminLogin = new LoginRequest
        {
            Email = "admin@dhl.com",
            Password = "admin123"
        };
        var userLogin = new LoginRequest
        {
            Email = "user@dhl.com",
            Password = "user123"
        };

        var adminResult = _controller.Login(adminLogin);
        var userResult = _controller.Login(userLogin);

        var adminToken = ((LoginResponse)((OkObjectResult)adminResult).Value!).Token;
        var userToken = ((LoginResponse)((OkObjectResult)userResult).Value!).Token;

        adminToken.Should().NotBe(userToken);
    }

    #endregion
}
