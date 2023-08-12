using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurChat
{
    internal class OnlineUsers
    {
        LinkedList<User>? Users { get; set; }
    }


    internal class User
    {
        public string? Name { get; set; }
    }
}
