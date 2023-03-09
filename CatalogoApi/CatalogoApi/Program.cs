using CatalogoApi.Context;
using CatalogoApi.Models;
using CatalogoApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. // ConfigureServices (class - StartUp)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
                 options
                 .UseMySql(connectionString,
                 ServerVersion.AutoDetect(connectionString)));

//registrando o servi�o token
builder.Services.AddSingleton<ITokenServices>(new TokenServices());

//valida��o do token
builder.Services.AddAuthentication
                    (JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer = builder.Configuration["Jwt:Issuer"],
                            ValidAudience = builder.Configuration["Jwt:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey
                            (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]))
                        };
                    });

builder.Services.AddAuthorization();

var app = builder.Build();

//endpoint para login
app.MapPost("/login", [AllowAnonymous] (UserModel userModel, ITokenServices tokenServices) =>
{
    if (userModel == null)
    {
        return Results.BadRequest("Login Inv�lido");
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
        return Results.BadRequest("Login Inv�lido");
    }

}).Produces(StatusCodes.Status400BadRequest)
                    .Produces(StatusCodes.Status200OK)
                    .WithName("Login")
                    .WithTags("Autenticacao");



//definindo os endpoints 
app.MapGet("/", () => "Cat�logo de Produtos - 2023").ExcludeFromDescription();

//Categorias ..........................................................
app.MapPost("/categorias", async (CategoriaModel categoria, AppDbContext db)
    =>
{
    db.Categorias.Add(categoria);
    await db.SaveChangesAsync();

    return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
});

app.MapGet("/categorias", async (AppDbContext db) =>
    await db.Categorias.ToListAsync()).RequireAuthorization();

app.MapGet("/categorias/{id:int}", async (int id, AppDbContext db) =>
{
    return await db.Categorias.FindAsync(id)
                 is CategoriaModel categoria
                 ? Results.Ok(categoria)
                 : Results.NotFound();
});

app.MapPut("/categorias/{id:int}", async (int id, CategoriaModel categoria, AppDbContext db) =>
{
    if (categoria.CategoriaId != id) return Results.BadRequest();

    var categoriaDB = await db.Categorias.FindAsync(id);

    if (categoriaDB is null) return Results.NotFound();

    categoriaDB.Nome = categoria.Nome;
    categoriaDB.Descricao = categoria.Descricao;

    await db.SaveChangesAsync();
    return Results.Ok(categoriaDB);
});

app.MapDelete("/categorias/{id:int}", async (int id, AppDbContext db) =>
{
    var categoria = await db.Categorias.FindAsync(id);

    if (categoria is null) return Results.NotFound();

    db.Categorias.Remove(categoria);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


//Produto ..........................................................
app.MapPost("/produtos", async (ProdutoModel produto, AppDbContext db)
    =>
{
    db.Produtos.Add(produto);
    await db.SaveChangesAsync();

    return Results.Created($"/produtos/{produto.ProdutoId}", produto);
});

app.MapGet("/produtos", async (AppDbContext db) => 
    await db.Produtos.ToListAsync()).RequireAuthorization();

app.MapGet("/produtos/{id:int}", async (int id, AppDbContext db) =>
{
    return await db.Produtos.FindAsync(id)
                 is ProdutoModel produto
                 ? Results.Ok(produto)
                 : Results.NotFound();
});

app.MapPut("/produtos/{id:int}", async (int id, ProdutoModel produto, AppDbContext db) =>
{
    if (produto.ProdutoId != id) return Results.BadRequest();

    var produtoDB = await db.Produtos.FindAsync(id);

    if (produtoDB is null) return Results.NotFound();

    produtoDB.Nome = produto.Nome;
    produtoDB.Descricao = produto.Descricao;
    produtoDB.Preco = produto.Preco;
    produtoDB.Imagem = produto.Imagem;
    produtoDB.DataCompra = produto.DataCompra;
    produtoDB.Estoque = produto.Estoque;
    produtoDB.CategoriaId = produto.CategoriaId;

    await db.SaveChangesAsync();
    return Results.Ok(produtoDB);
});

app.MapDelete("/produtos/{id:int}", async (int id, AppDbContext db) =>
{
    var produto = await db.Produtos.FindAsync(id);

    if (produto is null) return Results.NotFound();

    db.Produtos.Remove(produto);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


// Configure the HTTP request pipeline. //Configure (class - StartUp)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.Run();

