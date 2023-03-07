namespace CatalogoApi.Models
{
    public class CategoriaModel
    {
        public int CategoriaId { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }

        
        //relacionamento um p/ muitos
        public ICollection<ProdutoModel>? Produtos { get; set; }
    }
}
