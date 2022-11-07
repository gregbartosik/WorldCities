using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace WorldCitiesApi1.Data
{
    public class ApiResult<T>
    {
        //private constructor called by the CreateAsync method.
        private ApiResult(List<T> data, int count, int pageIndex, int pageSize, string? sortColumn, string? sortOrder)
        {
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            SortColumn = sortColumn;
            SortOrder = sortOrder;
        }
        // Pages an IQueryable source
        // Returns an object containing the paged/sorted result and all the relevant paging info
        // The sorting order ("ASC" or "DESC")
        public static async Task<ApiResult<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize, string? sortColumn = null, string? sortOrder = null)
        {
            var count = source.Count();
            if (!string.IsNullOrEmpty(sortColumn) && IsValidProperty(sortColumn))
            {
                sortOrder = !string.IsNullOrEmpty(sortOrder) && sortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
                source = source.OrderBy(string.Format("{0} {1}", sortColumn, sortOrder));
            }
            source = source
                .Skip(pageIndex * pageSize)
                .Take(pageSize);
            var data = await source.ToListAsync();
            return new ApiResult<T>(data, count, pageIndex, pageSize, sortColumn, sortOrder);
        }
        // Checks if the given property name exists to protect against SQL injection attacks
        public static bool IsValidProperty(string propertyName, bool throwExceptionIfNotFound = true)
        {
            var prop = typeof(T).GetProperty(propertyName,BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop ==null && throwExceptionIfNotFound)
            {
                throw new NotSupportedException(string.Format($"ERROR: Property '{propertyName}' does not exist."));
            }
            return prop != null;
        }
        // The Data result
        public List<T> Data { get; private set; }
        // Zero-based index of current page.
        public int PageIndex { get; private set; }
        // Number of items contained in each page
        public int PageSize { get; private set; }
        // Total items count
        public int TotalCount { get; private set; }
        // Total pages
        public int TotalPages { get; private set; }
        // TRUE if the current page has a previous page,
        // FALSE otherwise
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 0);
            }
        }
        // TRUE if the current page has a next page,
        // FALSE otherwise
        public bool HasNextPage
        {
            get
            {
                return ((PageIndex +1 ) < TotalPages);
            }
        }
        // Sorting Column name
        public string? SortColumn { get; set; }
        // Sorting Column order "ASC" "DESC" or null if not set
        public string? SortOrder { get; set; }
    }
}
