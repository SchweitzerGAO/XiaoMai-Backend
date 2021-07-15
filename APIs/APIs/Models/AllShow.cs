using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIs.Models
{
    public class AllShow
    {
        public string name { get; set; }    //演出名
        public string introduction { get; set; }    //演出介绍
        public string photo { get; set; }           //演出照片
        public List<string> label { get; set; }      //标签
    }
}
