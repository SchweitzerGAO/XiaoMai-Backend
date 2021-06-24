using System.Collections.Generic;

namespace APIs.Models
{
    public class Area                                // 分区信息
    {
        public string name { get; set; }             // 分区名
        public double price { get; set; }            // 分区价格
        public long available { get; set; }          // 分区可用座位
        public List<int> seatNumbers { get; set; }   // 分区可用座位编号集合
    }
}
