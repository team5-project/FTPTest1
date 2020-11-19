using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace FTPTest1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void button_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("TT");
            //string file = "F:\txt.txt";
           Upload(@"F:\file\txt.txt");
            //UploadFile("F:\file\txt.txt");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DownLoad();
            MessageBox.Show("TT");
            //string file = "F:\txt.txt";
             //Upload(@"F:\file\txt.txt");
            //UploadFile("F:\file\txt.txt");
        }


        private static void UploadFile(string localFile)
        {
            string FtpAddress = "192.168.1.98";
           // string FtpRemotePath = "File";
            string FtpUid = "TT";
            string FtpPwd = "FTP";

            FileInfo fi = new FileInfo(localFile);
            FileStream fs = fi.OpenRead();
            long length = fs.Length;
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create("ftp://" + FtpAddress+ "/" + fi.Name);
            req.Credentials = new NetworkCredential(FtpUid, FtpPwd);
            req.Method = WebRequestMethods.Ftp.UploadFile;
            req.UseBinary = true;
            req.ContentLength = length;
            req.Timeout = 10 * 1000;
            try
            {
                Stream stream = req.GetRequestStream();
                int BufferLength = 2048;
                byte[] b = new byte[BufferLength];
                int i;
                while ((i = fs.Read(b, 0, BufferLength)) > 0)
                {
                    stream.Write(b, 0, i);
                }
                stream.Close();
                stream.Dispose();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public void Upload(string filename)
        {
            string FtpAddress = "192.168.1.98";
            string FtpRemotePath = "/File/";
            string username = "TT";
            string password = "FTP";
            FileInfo fileInf = new FileInfo(filename);
            string uri = "ftp://" + FtpAddress + "/" + fileInf.Name;
            FtpWebRequest reqFTP;

            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            reqFTP.Credentials = new NetworkCredential(username, password);
            reqFTP.KeepAlive = false;
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            reqFTP.UseBinary = true;
            reqFTP.UsePassive = false;
            reqFTP.ContentLength = fileInf.Length;
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            FileStream fs = fileInf.OpenRead();
            try
            {
                Stream strm = reqFTP.GetRequestStream();
                contentLen = fs.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                strm.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Ftphelper Upload Error --> " + ex.Message);
            }

        }

        public void DownLoad()
        {
            string FtpPath = "ftp://" + "192.168.1.98"+"/txt.txt";
            // string FtpRemotePath = "File";
            string FtpUserName = "TT";
            string FtpPassWord = "FTP";
            Uri uri = new Uri(FtpPath);
            string TempFolderPath = @"F:\file\getfile";
            string FileName = Path.GetFullPath(TempFolderPath) + Path.DirectorySeparatorChar.ToString() + Path.GetFileName(uri.LocalPath);

            //创建一个文件流
            FileStream fs = null;
            Stream responseStream = null;
            try
            {
                //创建一个与FTP服务器联系的FtpWebRequest对象
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
                //设置请求的方法是FTP文件下载
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                //连接登录FTP服务器
                request.Credentials = new NetworkCredential(FtpUserName, FtpPassWord);

                //获取一个请求响应对象
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                //获取请求的响应流
                responseStream = response.GetResponseStream();

                //判断本地文件是否存在，如果存在，则打开和重写本地文件

                //if (File.Exists(FileName))

                //{
                //    if (LocalFileExistsOperation == "write")

                //    {
                //        fs = File.Open(FileName, FileMode.Open, FileAccess.ReadWrite);

                //    }
                //}

                //判断本地文件是否存在，如果不存在，则创建本地文件
                //else

                //{
                fs = File.Create(FileName);
                //}

                if (fs != null)
                {

                    int buffer_count = 65536;
                    byte[] buffer = new byte[buffer_count];
                    int size = 0;
                    while ((size = responseStream.Read(buffer, 0, buffer_count)) > 0)
                    {
                        fs.Write(buffer, 0, size);

                    }
                    fs.Flush();
                    fs.Close();
                    responseStream.Close();
                }
            }
            finally
            {
                if (fs != null)
                    fs.Close();
                if (responseStream != null)
                    responseStream.Close();
            }
        }
    }
}
