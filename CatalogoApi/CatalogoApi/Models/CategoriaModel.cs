using System.Text.Json.Serialization;

namespace CatalogoApi.Models;

public class CategoriaModel
{
    public int CategoriaId { get; set; }
    public string? Nome { get; set; }
    public string? Descricao { get; set; }


    //relacionamento um p/ muitos
    [JsonIgnore]
    public ICollection<ProdutoModel>? Produtos { get; set; }
}
