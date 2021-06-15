using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIs.Models
{
    public class SearchResult  // 搜索结果，包括名称相符的演出、周边
    {
        public List<GeneralShow> shows { get; set; } 
        public List<GeneralGoods> goods { get; set; }
    }
}
