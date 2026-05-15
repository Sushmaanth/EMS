namespace EMSFrontend.Models
{
    public class PaginationViewModel<T>
    {
        public IEnumerable<T> Data { get; set; }

        public int TotalRecords { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalPages =>
        (int)Math.Ceiling((double)TotalRecords / PageSize);

        public string? SearchText { get; set; }
    }
}
