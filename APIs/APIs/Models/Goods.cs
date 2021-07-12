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
    public class GoodsResult                  //商家查看自己的周边时返回的结果
    {
        public string id { get; set; }              // 周边ID（以G开头）
        public string goodsName { get; set; }       // 周边名
        public double price { get; set; }           // 价格
        public long available { get; set; }      // 剩余存货
    }
    public class AddGoods                    // 周边信息（用于商家添加）
    {
        public long sellerId { get; set; }  //商家ID
        public long showId { get; set; }        //演出ID
        public string goodsName { get; set; }       // 周边名
        public string description { get; set; }     //周边简介
        public string goodsPhoto { get; set; }       //周边图片
        public double price { get; set; }        //价格
        public long available { get; set; }      //数量
    }
    public class UpdateGoods                    // 周边信息（用于商家修改）
    {
        public string id { get; set; }              //周边ID
        public double price { get; set; }        //价格
        public long available { get; set; }      //数量
    }
}
