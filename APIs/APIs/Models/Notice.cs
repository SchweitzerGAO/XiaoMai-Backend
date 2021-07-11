namespace APIs.Models
{
    // 我在NOTICE表里面加了TYPE属性（NUMBER），我们约定一下，TYPE为0是顾客商家都能看到，
    // 为1是只有顾客能看到,为2是只有商家能看到
    // 我加了一个TITLE属性存储通知标题，管理员添加的时候注意一下
    public class GeneralNotice
    {
        
        public ulong id { get; set; }        // 通知ID
        public string time { get; set; }    // 通知时间
        public string title { get; set; }   // 通知标题
    }
    public class NoticeContent      //收到的通知包括的信息
    {
        public string time { get; set; }    // 通知时间
        public string title { get; set; }   // 通知标题
        public string content { get; set; }     //通知内容
    }
}
