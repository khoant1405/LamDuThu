﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JSN.Core.Entity;
using JSN.Core.Model;
using JSN.Core.ViewModel;
using JSN.Service.Interface;
using JSN.Shared.Config;
using JSN.Shared.Enum;
using JSN.Shared.Model;
using Microsoft.IdentityModel.Tokens;

namespace JSN.Service.Implement;

public class AuthService(IUnitOfWork unitOfWork, IUserService userService, IRepository<User> userRepository) : IAuthService
{
    public async Task<User> RegisterAsync(UserView request)
    {
        CreatePasswordHash(request.Password, out var passwordHash, out var passwordSalt);

        User newUser = new()
        {
            UserName = request.UserName,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            IsActive = true,
            Role = request.UserName == "superadmin" ? (int)Role.Admin : (int)Role.User
        };

        await userRepository.AddAsync(newUser);
        await unitOfWork.CommitAsync();

        return newUser;
    }

    public async Task<TokenModel> LoginAsync(UserView request)
    {
        var user = userService.GetUserByUserName(request.UserName);

        var token = CreateToken(user!);
        var refreshToken = GenerateRefreshToken();
        SetRefreshToken(user!, refreshToken);

        userRepository.Update(user!);
        await unitOfWork.CommitAsync();

        return new TokenModel
        {
            AccessToken = token,
            RefreshToken = refreshToken.Token
        };
    }

    public async Task<TokenModel> RefreshTokenAsync(TokenModel? tokenModel)
    {
        var accessToken = tokenModel!.AccessToken;
        var principal = GetPrincipalFromExpiredToken(accessToken);
        var userName = principal!.Identity?.Name;
        var user = userService.GetUserByUserName(userName);

        var newAccessToken = CreateToken(user!);
        var newRefreshToken = GenerateRefreshToken();
        SetRefreshToken(user!, newRefreshToken);

        userRepository.Update(user!);
        await unitOfWork.CommitAsync();

        return new TokenModel
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token
        };
    }

    public string CheckLogin(UserView request)
    {
        var user = userService.GetUserByUserName(request.UserName);
        if (user == null)
        {
            return "User not found.";
        }

        return !VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt) ? "Wrong password." : "";
    }

    public string CheckUserExists(UserView request)
    {
        var user = userService.GetUserByUserName(request.UserName);
        if (user != null)
        {
            return "User Exists.";
        }

        return "";
    }

    public string CheckRefreshToken(TokenModel? tokenModel)
    {
        if (tokenModel is null)
        {
            return "Invalid client request";
        }

        var accessToken = tokenModel.AccessToken;
        var refreshToken = tokenModel.RefreshToken;

        var principal = GetPrincipalFromExpiredToken(accessToken);
        if (principal == null)
        {
            return "Invalid access token";
        }

        var userName = principal.Identity?.Name;
        var user = userService.GetUserByUserName(userName);
        if (user == null)
        {
            return "Invalid access token";
        }

        if (user.RefreshToken != refreshToken)
        {
            return "Invalid refresh token";
        }

        return user.TokenExpired <= DateTime.Now ? "Token expired" : "";
    }

    private static RefreshToken GenerateRefreshToken()
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expired = DateTime.Now.AddDays(7),
            Created = DateTime.Now
        };

        return refreshToken;
    }

    private static void SetRefreshToken(User user, RefreshToken newRefreshToken)
    {
        user.RefreshToken = newRefreshToken.Token;
        user.TokenCreated = newRefreshToken.Created;
        user.TokenExpired = newRefreshToken.Expired;
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, user.Role.ToString() ?? "")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConfig.JwtConfig.Token!));
        var tokenValidityInMinutes = AppConfig.JwtConfig.TokenValidityInMinutes;

        var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddMinutes(tokenValidityInMinutes), signingCredentials: credential);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    private static bool VerifyPasswordHash(string password, IEnumerable<byte> passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
    }

    private static ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConfig.JwtConfig.Token!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512Signature, StringComparison.InvariantCultureIgnoreCase))
        {
            return null;
        }

        return principal;
    }
}