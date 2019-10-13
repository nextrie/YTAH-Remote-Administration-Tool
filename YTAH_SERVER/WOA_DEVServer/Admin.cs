using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WOA_DEVServer
{
    public class Admin
    {
        public static List<Admin> admins = new List<Admin>();
        public string username;
        public TcpClient client;
        public User selected;
        public Admin(string usernam, TcpClient cl)
        {
            username = usernam;
            client = cl;
        }
    }
}
