using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.helpers;
using Application.IServices;
using Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class TokenService : ITokenService
{
    //microsoft.aspnetcore.auth
    private readonly AppSettings _appSettings;
    public TokenService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }
    public string CreateToken(Employee user)
    {
        var _key = Encoding.UTF8.GetBytes(_appSettings.Secret); 

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Name),
            new Claim("id", user.EmployeeId.ToString()),
            new Claim("role", user.Role), // maybe this need to be updated 
            new Claim("warehouseId", user.WarehouseId.ToString()) // for extra verification
            
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha512Signature);
    
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}