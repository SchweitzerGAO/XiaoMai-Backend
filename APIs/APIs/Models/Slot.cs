using System.Collections.Generic;
namespace APIs.Models
{
    public class GeneralSlot                   // 总体场次信息
    {
        public long id { get; set; }           // 场次ID
        public long sellerId { get; set; }     // 商家ID
        public string place { get; set; }      // 地点
        public string day { get; set; }        // 日期
        public string timeStart { get; set; }  // 开始时间
        public string timeEnd { get; set; }    // 结束时间
    }
    public class ParticularSlot                // 详细场次信息
    {
        public long sellerId { get; set; }
        public string showName { get; set; }
        public string map { get;set; }         // 场次地图
        public List<Area> areas { get; set; }  // 分区信息
    }
    public class SlotInfo
    {
        public string name { get; set; }
        public string timeStart { get; set; }
        public string timeEnd { get; set; }
    }
    public class SellerSlot
    {
        public long id { get; set; }           // 场次ID
       public string showName { get; set; }    // 演出名称
        public string place { get; set; }      // 地点
        public string day { get; set; }        // 日期
        public string timeStart { get; set; }  // 开始时间
        public string timeEnd { get; set; }    // 结束时间
        public List<Area> areas { get; set; }  // 分区信息

    }
}
