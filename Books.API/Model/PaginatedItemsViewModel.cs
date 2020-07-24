using System.Collections.Generic;

namespace Books.API.Model
{
    public class PaginatedItemsViewModel<TEntity> where TEntity : class
    {
        /// <summary>
        /// index of return page in get methods with more than one record.
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// size of return page of get methods
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// count of all records
        /// </summary>
        public long Count { get; private set; }

        public IEnumerable<TEntity> Data { get; private set; }

        public PaginatedItemsViewModel(int pageIndex, int pageSize, long count, IEnumerable<TEntity> data)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.Count = count;
            this.Data = data;
        }
    }
}
