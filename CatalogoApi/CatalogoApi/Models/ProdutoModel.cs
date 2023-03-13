﻿using System.Text.Json.Serialization;

namespace CatalogoApi.Models;

public class ProdutoModel
{
    public int ProdutoId { get; set; }
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public string? Imagem { get; set; }
    public DateTime DataCompra { get; set; }
    public int Estoque { get; set; }


    //relacionamento um p/ muitos
    public int CategoriaId { get; set; }
    [JsonIgnore]
    public CategoriaModel? Categoria { get; set; }
}
