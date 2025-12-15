using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    public class PaginatedResult<T> // made it generic so that we can use it to any type we wanted to
    {
        //Generics are really the programming constructs that enable us to make classes or methods that work with various data types until the code is used.
        public PaginationMetadata Metadata { get; set; } = default!;
        public List<T> Items { get; set; } = [];
    }

    public class PaginationMetadata
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }

    public class PaginationHelper
    {
        public static async Task<PaginatedResult<T>> CreateAsync<T>(IQueryable<T> query, int pageNumber, int pageSize)
        {
            var count = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedResult<T>
            {
                Metadata = new PaginationMetadata
                {
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                    PageSize = pageSize,
                    TotalCount = count
                },
                Items = items
            };
        }
    }
}
