using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurChat
{

    public class AllMessage
    {
        public AllMessage(string text, string name, string time)
        {
            Text = text;
            Name = name;
            Time = time;
        }
        private string name;
        private string time;


        public string Text { get; set; }
        public string Name { get { return "发送者：" + name + "于"; } set { name = value; } }

        public string Time { get { return "时间：" + time + "说："; } set { time = value; }}

        public override string? ToString()
        {
            return base.ToString();
        }
    }

}
