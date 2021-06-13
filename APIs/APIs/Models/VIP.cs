namespace APIs.Models
{
    // 会员类
    public class VIP
    {
        public long customerId { get; set; }  // 顾客ID
        public double point { get; set; }     // 积分
        public int level { get; set; }        // 等级
        public double discount { get; set; }  // 折扣

    }
}
