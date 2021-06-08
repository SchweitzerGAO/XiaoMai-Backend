namespace APIs.Models
{
    public class Comment
    {
        public float? rate { get; set; }
        public string content { get; set; }
        public long customerId { get; set; }
        public long showId { get; set; }
    }
    public class CommentCustomer
    {
        public long commentId { get; set; }
        public string customerName { get; set; }
        public long customerId { get; set; }
        public float rate { get; set; }
        public string content { get; set; }
        public string time { get; set; }

    }
}
