namespace APIs.Models
{
    public class Comment                           // 评论信息
    {
        public float? rate { get; set; }           // 评分
        public string content { get; set; }        // 内容
        public long customerId { get; set; }       // 顾客ID
        public long showId { get; set; }           // 演出ID
    }
    // 与上同名之属性不再注释
    public class CommentCustomer                   // 评论+顾客信息
    {
        public long commentId { get; set; }        // 评论ID
        public string customerName { get; set; }   // 顾客用户名
        public long customerId { get; set; }       
        public float rate { get; set; }
        public string content { get; set; }
        public string time { get; set; }           // 评论时间

    }
}
