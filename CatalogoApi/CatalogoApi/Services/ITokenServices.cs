using CatalogoApi.Models;

namespace CatalogoApi.Services;

public interface ITokenServices
{
    string GerarToken (string key, string issuer, string audience, UserModel user);
}
