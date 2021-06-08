namespace APIs.Models
{
    // 会员类
    public class VIP
    {
        public long customerId { get; set; }  // 顾客ID
        public long point { get; set; }       // 积分
        public int level { get; set; }
        public double discount { get; set; }

    }
}
