namespace APIs.Models
{
    public class GeneralGoods               // 总体周边信息
    {
        public long id { get; set; }        // 周边ID
        public long sellerId { get; set; }  // 商家ID
        public string name { get; set; }    // 周边名称
        public string image { get; set; }   // 周边图片

    }
    public class ParticularGoods                 // 详细周边信息
    {

        public string sellerName { get; set; }   // 商家名称
        public double price { get; set; }        // 价格
        public long available { get; set; }      // 剩余存货

    }
}
