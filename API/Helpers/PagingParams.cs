namespace API.Helpers
{
    public class PagingParams
    {
        //created this class so that we can take in the pagination parameters as Query string parameters inside our controller. and we will be using this class in the repository.
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        //check what the pagesize is being requested and if it is over 50 then we will set the page size to be 50 we will set it to be what is requested.
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
