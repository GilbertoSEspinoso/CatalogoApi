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


        public DbSet<ProdutoModel>? Produtos { get; set; }
        public DbSet<CategoriaModel>? Categorias { get; set; }



        // A class DbContext possui um métod chamado OnModelCreating que usa uma instância de ModelBuilder
        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<CategoriaModel>().HasKey(c => c.CategoriaId); //definindo a chave primária

            //configuração das colunas

            //Categoria
            mb.Entity<CategoriaModel>().Property(c => c.Nome).HasMaxLength(100).IsRequired();

            mb.Entity<CategoriaModel>().Property(c => c.Descricao).HasMaxLength(150).IsRequired();

            //Produto
            mb.Entity<ProdutoModel>().HasKey(c => c.ProdutoId);

            mb.Entity<ProdutoModel>().Property(c => c.Nome).HasMaxLength(100).IsRequired();

            mb.Entity<ProdutoModel>().Property(c => c.Descricao).HasMaxLength(150);

            mb.Entity<ProdutoModel>().Property(c => c.Imagem).HasMaxLength(100);

            mb.Entity<ProdutoModel>().Property(c => c.Preco).HasPrecision(14, 2);

            //Relacionamento
            mb.Entity<ProdutoModel>()
                    .HasOne<CategoriaModel>(c => c.Categoria)
                    .WithMany(p => p.Produtos)
                    .HasForeignKey(c => c.CategoriaId);

        }

    }
}
