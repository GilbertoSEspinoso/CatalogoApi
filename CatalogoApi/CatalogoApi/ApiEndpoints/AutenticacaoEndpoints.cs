﻿using CatalogoApi.Models;
using CatalogoApi.Services;
using Microsoft.AspNetCore.Authorization;

namespace CatalogoApi.ApiEndpoints;

public static class AutenticacaoEndpoints
{
    public static void MapAutenticacaoEndpoints(this WebApplication app)
    {
        //endpoint para login
        app.MapPost("/login", [AllowAnonymous] (UserModel userModel, ITokenServices tokenServices) =>
        {
            if (userModel == null)
            {
                return Results.BadRequest("Login Inválido");
            }

            if (userModel.UserName == "macoratti" && userModel.Password == "numsey#123")
            {
                var tokenString = tokenServices.GerarToken(app.Configuration["Jwt:Key"],
                        app.Configuration["Jwt:Issuer"],
                        app.Configuration["Jwt:Audience"],
                        userModel);
                return Results.Ok(new { token = tokenString });
            }
            else
            {
                return Results.BadRequest("Login Inválido");
            }

        }).Produces(StatusCodes.Status400BadRequest)
                            .Produces(StatusCodes.Status200OK)
                            .WithName("Login")
                            .WithTags("Autenticacao");
    }
}
