namespace WebApplication1.Models.Pagination
{
    public class PaginationMetadata
    {
        private int _currentPage;
        private int _totalCount;
        private int _totalPages;

        public PaginationMetadata(int totalCount, int currentPage, int itemsPerPage)
        {
            SetCurrentPage(currentPage);
            SetTotalCount(totalCount);
            SetTotalPages(totalCount, itemsPerPage);
        }

        public int CurrentPage => _currentPage;
        public int TotalCount => _totalCount;
        public int TotalPages => _totalPages;
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        private void SetCurrentPage(int currentPage)
        {
            _currentPage = currentPage;
        }

        private void SetTotalCount(int totalCount)
        {
            _totalCount = totalCount;
        }

        private void SetTotalPages(int totalCount, int itemsPerPage)
        {
            _totalPages = (int)Math.Ceiling(totalCount / (double)itemsPerPage);
        }
    }
}
