namespace APIs.Models
{
    public class ShowOrder                          // 演出订单
    {
        public long customerId { get; set; }        // 顾客ID
        public long sellerId { get; set; }          // 商家ID
        public string name { get; set; }            // 演出名称
        public long slotId { get; set; }            // 场次ID
        public string areaName { get; set; }        // 分区
        public long seatNumber { get; set; }        // 座位
        public double price { get; set; }           // 票价

    }
    public class GoodsOrder                         // 周边订单
    {
        public long customerId { get; set; }        // 顾客ID
        public long sellerId { get; set; }          // 商家ID
        public long goodsId { get; set; }           // 周边ID
        public string goodsName { get; set; }       // 周边名称
        public double price { get; set; }           // 周边价格
    }
   
}
