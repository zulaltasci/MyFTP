using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyFTP
{
    public partial class MyFTP : Component
    {
        public MyFTP()
        {
            InitializeComponent();
        }

        public MyFTP(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

       
        public bool IsSetted = false;
        /// <summary>
        /// Kullanıcıdan alacağımız bilgileri tanımladığım public void türünde metotlar.
        /// </summary>
        public string host=" ";
        public string UserId = " ";
        public string Password=" ";

        /// <summary>
        /// Tanımlama işlemlerinin yapıldığı public metot.
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="UserId1"></param>
        /// <param name="PassWord"></param>
        public void SetInformation(string Host ,string UserId1, string PassWord) 
        {
                 IsSetted = true;
                 host = PassWord;
                 UserId = UserId1;
                 Password = PassWord;        
        }
        /// <summary>
        /// FTP'de klasör oluşturmayı sağlayan public bool metot.
        /// </summary>
        /// <returns></returns>
        public bool CreateFolder()
        {
            string path = "/Index";

            if (IsSetted)
            {
                try
                {
                    WebRequest request = WebRequest.Create(host + path);
                    request.Method = WebRequestMethods.Ftp.MakeDirectory;
                    request.Credentials = new NetworkCredential(UserId, Password);
                    using (var resp = (FtpWebResponse)request.GetResponse())
                    {
                        Console.WriteLine(resp.StatusCode);
                    }
                    return true;
                }

                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
            
        }

        /// <summary>
        /// Bir Ftp dosyası olup olmadığını kontrol ettiğim public bool türünde bir metot.
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public bool FtpDirectoryControl(string dirPath)
        {
            bool IsExist = false;
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(dirPath);
                request.Credentials = new NetworkCredential(UserId, Password);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    IsExist = true;
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    FtpWebResponse response = (FtpWebResponse)ex.Response;
                    if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        return false;
                    }
                }
            }
            return IsExist;
        }

        /// <summary>
        /// FtpDirectoryControl metodunu çağıran metot.
        /// </summary>
        private void DoesExist(string path)
        {
            FtpDirectoryControl(path);
        }
        /// <summary>
        /// Dosyamızı FTP' ye yükleyen public void türünde metot.
        /// </summary>
        public void UploadFile()
        {
            string From = @"C:\DosyaYolu\Dosya.xlsx";
            string To = "ftp://192.168.1.1/dosyayoluy/Dosya.xlsx";

            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(UserId, Password);
                client.UploadFile(To, WebRequestMethods.Ftp.UploadFile, From);
            }
        }

        /// <summary>
        /// Dosyaların ve klasörlerin listesini aldığım private List türünde metot.
        /// </summary>
        /// <param name="Folderpath"></param>
        /// <returns></returns>
        private List<string> GetAllFtpFiles(string Folderpath)
        {
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(Folderpath);
                ftpRequest.Credentials = new NetworkCredential(UserId, Password);
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream());

                List<string> directories = new List<string>();

                string line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    string[] lineArr = line.Split('/');
                    line = lineArr[lineArr.Count() - 1];
                    directories.Add(line);
                    line = streamReader.ReadLine();
                }

                streamReader.Close();

                return directories;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        ///  GetAllFiles metodunu çağıran metot.
        /// </summary>
        private void GetAllFiles(string path)
        { 

           GetAllFtpFiles(path);
        }

        /// <summary>
        /// Dosya ve ya klasörü silmeye yarayan public void türünde metot.
        /// </summary>
        /// <param name="Folderpath"></param>
        public  void DeleteFTPFolder(string Folderpath)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Folderpath);
            request.Method = WebRequestMethods.Ftp.RemoveDirectory;
            request.Credentials = new System.Net.NetworkCredential(UserId, Password);
            request.GetResponse().Close();
        }

        /// <summary>
        /// DeleteFTPFolder metodunu çağıran metot.
        /// </summary>
        private void DeleteFolder(string path)
        {
            DeleteFTPFolder(path);
        }

        
    }
}
