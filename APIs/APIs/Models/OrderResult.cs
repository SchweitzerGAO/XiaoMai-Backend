using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIs.Models
{
    public class GoodsOrderResult                   // 查看周边订单时返回的结果
    {
        public string id { get; set; }              // ID(以G开头)
        public string sellerName { get; set; }      // 商家名
        public string goodsName { get; set; }       // 周边名
        public string payTime { get; set; }         // 支付时间
        public double price { get; set; }           // 价格

    }
    public class ShowOrderResult                   // 查看演出订单返回的结果
    {
        public string id { get; set; }             // ID(以S开头)
        public string sellerName { get; set; }     // 商家名
        public string showName { get; set; }       // 演出名
        public string place { get; set; }          // 演出地点
        public string day { get; set; }            // 演出日期
        public string timeStart { get; set; }      // 开始
        public string timeEnd { get; set; }        // 结束
        public string areaName { get; set; }       // 分区
        public long seatNumber { get; set; }       // 座位
        public string payTime { get; set; }        // 支付时间
        public double price { get; set; }          // 价格
    }
    public class AllOrderResult                    // 全部订单
    {
        public List<GoodsOrderResult> goodOrders { get; set; }  // 周边订单
        public List<ShowOrderResult> showsOrders { get; set; }  // 演出订单
    }
}
