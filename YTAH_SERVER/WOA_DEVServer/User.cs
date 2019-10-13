using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WOA_DEVServer
{
    public class User
    {
        public  string username;
        public  string poste;
        public  TcpClient client;
        public string admin;
        public string ip;
        public User(string usernames, string postes, TcpClient c, string isadmin, string ipadress)
        {
            username = usernames;
            poste = postes;
            client = c;
            admin = isadmin;
            ip = ipadress;
        }
    }
}
