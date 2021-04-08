namespace sigreh.Wrappers
{
    public class PageResponse<T>
    {
        public Page First { get; set; }
        public Page Last { get; set; }

        public int Total { get; set; }
        public int Records { get; set; }
        public Page Next { get; set; }
        public Page Previous { get; set; }
        public T Data { get; set; }

    }
}
