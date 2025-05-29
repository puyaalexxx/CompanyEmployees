namespace Shared.RequestFeatures
{
    public class PageList<T> : List<T>
    {
        public MetaData MetaData { get; set; }

        public PageList(List<T> items, int count, int pageNumber, int pageSize)
        {
            MetaData = new MetaData
            {
                TotalCount = count,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            };

            AddRange(items);
        }

        public static PageList<T> ToPageList(IEnumerable<T> source, int count, int pageNumber, int pageSize)
        {
            var items = source.ToList();

            return new PageList<T>(items, count, pageNumber, pageSize);
        }
    }
}
