using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIs.Models
{
    public class GeneralShow                 // 总体演出信息
    {
        public long showId { get; set; }     // 演出ID
        public string name { get; set; }     // 演出名称
        public string image { get; set; }    // 演出图片
    }
    public class ParticularShow
    {
        public string introduction { get; set; }               // 详细演出信息
        public List<GeneralSlot> slots { get; set; }           // 所有场次
        public List<GeneralGoods> goods { get; set; }          // 所有周边
        public List<CommentCustomer> comments { get; set; }    // 所有评论
        public List<string> labels { get; set; }               // 所有标签
    }
}
