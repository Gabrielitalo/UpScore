namespace RecomeceAPI.Models.Common
{
    public class PaginationModel
    {
        public int PageCount { get; set; } // Total de páginas
        public int TotalItens { get; set; } // Total de itens
        public int CurrentPage { get; set; } // Página atual
        public int ItensPerPage { get; set; } // Total de itens por página
        public object? Itens { get; set; } // Itens do retorno dinâmico
        public object? Totals { get; set; } // Totalizados do retorno dinâmico
    }
}
