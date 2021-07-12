using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIs.Models
{
    public class SellerInformation
    {
        public long settleDay { get; set; }           //建立日期
        public long slotAmount { get; set; }          //场次数量
        public long orderGoodsAmount { get; set; }    //商品订单数量
        public long orderShowAmount { get; set; }     //演出订单数量
        public long earnings { get; set; }            //收入

    }
}
