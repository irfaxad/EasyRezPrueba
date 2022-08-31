namespace EasyRez.Models
{
    public class Paginador<T> where T : class
    {
        public int PaginaActual { get; set; }
        public int TipoEntidadTributaria { get; set; }
        public int TotalPaginas { get; set; }
        public IEnumerable<T>? Resultado { get; set; }
    }
}
