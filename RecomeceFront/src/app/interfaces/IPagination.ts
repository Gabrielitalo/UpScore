export interface IPagination {
  pageCount: number; // Total de páginas
  totalItens: number; // Total de itens
  currentPage: number; // Página atual
  itensPerPage: number; // Total de itens por página
  itens: any;// Itens do retorno dinâmico
  totals: any;// Totalizados do retorno dinâmico
}
