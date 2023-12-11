using Core.Entities;

namespace Application.IServices;

public interface ITokenService
{
    string CreateToken(Employee user);
}