using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIs.Models
{
    public class AllSlot
    {
        public long showId { get; set; }       // 演出ID
        public long sellerId { get; set; }     // 商家ID
        public string place { get; set; }      // 地点
        public string day { get; set; }        // 日期
        public string timeStart { get; set; }  // 开始时间
        public string timeEnd { get; set; }    // 结束时间
        public string map { get; set; }         // 场次地图

        public List<AreaFitP> areas { get; set; } //分区
       
    }
    public class AreaFitP
    {
        
        public double price { get; set; }       //分区价格
        public string name { get; set; }      //分区名字
        public long available { get; set; }        //分区可用座位数
    }
  
}