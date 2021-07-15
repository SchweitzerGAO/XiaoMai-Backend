using System.Collections.Generic;

namespace APIs.Models
{
    public class SearchResult                         // 搜索结果，包括名称相符的演出、周边
    {
        public List<GeneralShow> shows { get; set; }  // 演出
        public List<GeneralGoods> goods { get; set; } // 周边
    }
}
