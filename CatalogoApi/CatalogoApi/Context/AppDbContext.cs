using CatalogoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoApi.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        //A Instância de DbContextOptions carrega todas as informações e configurações, como por exemplo a string de conexão


        public DbSet<ProdutoModel> Produtos { get; set; }
        public DbSet<CategoriaModel> Categorias { get; set; }

    }
}
