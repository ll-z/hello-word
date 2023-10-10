using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Management;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace DemonForWorldZhu.Common
{
   public class DataOperation
    {
        /// <summary>
        /// 将对象序列化为byte数组
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] SerializeObject(object obj)
        {
            if (obj == null)
                return null;
            //内存实例
            MemoryStream ms = new MemoryStream();
            //创建序列化的实例
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);//序列化对象，写入ms流中  
            byte[] bytes = ms.GetBuffer();
            return bytes;
        }

        /// <summary>
        /// 数组反序列化为对象
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static object DeserializeObject(byte[] bytes)
        {
            object obj = null;
            if (bytes == null)
                return obj;
            //利用传来的byte[]创建一个内存流
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;
            BinaryFormatter formatter = new BinaryFormatter();
            obj = formatter.Deserialize(ms);//把内存流反序列成对象  
            ms.Close();
            return obj;
        }

        /// <summary>
        /// 压缩数组
        /// </summary>
        /// <param name="inBytes"></param>
        /// <returns></returns>
        public static MemoryStream Compress(byte[] inBytes)
        {
            MemoryStream outStream = new MemoryStream();
            using (MemoryStream intStream = new MemoryStream(inBytes))
            {
                using (GZipStream Compress = new GZipStream(outStream, CompressionMode.Compress))
                {
                    intStream.CopyTo(Compress);
                }

            }

            return outStream;

        }

        /// <summary>
        /// 解压缩数组
        /// </summary>
        /// <param name="inStream"></param>
        /// <returns></returns>
        public static byte[] Decompress(MemoryStream inStream)
        {
            byte[] result = null;
            MemoryStream compressedStream = new MemoryStream(inStream.ToArray());

            using (MemoryStream outStream = new MemoryStream())
            {
                using (GZipStream Decompress = new GZipStream(compressedStream,

                 CompressionMode.Decompress))
                {
                    Decompress.CopyTo(outStream);
                    result = outStream.ToArray();
                }
            }
            return result;

        }

#if(false)

            MemoryStream memoryStream = new MemoryStream();//o
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, tarStringTmp);
            byte[] bytes = memoryStream.ToArray();
            string tarStringTmpSerialize = CommonModules.ToHexString(bytes);
            byte[] bytes1= CommonModules.GetBytesFromHexString(tarStringTmpSerialize);
            MemoryStream ms = new MemoryStream(bytes1);
            string obj = (string)formatter.Deserialize(ms);//把内存流反序列成对象  

#endif


        /// <summary>
        /// byte数组转十六进制字符
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesToHexString(byte[] bytes)
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {

                    strB.Append(bytes[i].ToString("X2"));
                    hexString = strB.ToString();
                }
            }
            return hexString;
        }

        /// <summary>
        /// 十六进制字符转byte数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] GetBytesFromHexString(string hexString)
        {

            hexString = hexString.Replace(" ", "");
            if (hexString.Length % 2 != 0)
            {
                hexString += "";
            }
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);

            }
            return returnBytes;

        }

        /// <summary>
        /// 字符转byte数组 再转十六进制字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StringToHexString(string str)
        {
            if (str != null)
            {
                MemoryStream memoryStream = new MemoryStream();//o
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, str);
                byte[] bytes = memoryStream.ToArray();
                return BytesToHexString(bytes);
            }
            return null;

        }

        /// <summary>
        /// 十六进制字符转byte数组再转字符
        /// </summary>
        /// <param name="HexStr"></param>
        /// <returns></returns>
        public static string HexStringToString(string HexStr)
        {
            if (HexStr == null) return null;
            byte[] bytes = GetBytesFromHexString(HexStr);
            if (bytes != null && bytes.Length > 0)
            {
                MemoryStream ms = new MemoryStream(bytes);
                BinaryFormatter formatter = new BinaryFormatter();
                return (string)formatter.Deserialize(ms);//把内存流反序列成对象  

            }
            return null;

        }

        //加载配置文件
        public List<string> LoadTextFile(string filePath, out string message)
        {
            //string[] result = new string[];
            List<string> result = new List<string>();
            message = "";
            //判断配置文件是否存在
            if (File.Exists(filePath))
            {
                StreamReader sr = new StreamReader(filePath, Encoding.Default);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    result.Add(line);
                    // _templateStr += line.ToString()+"\n";
                }
            }
            else
            {
                message = filePath + "不存在，请选择模板文件!";
                System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
                dialog.Filter = "TMP File (*.tmp)|*.tmp|All Files (*.*)|*.*";
                dialog.InitialDirectory = "C:";
                dialog.Title = "Select a tmp File";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    result.Add(dialog.FileName);
                }
            }
            return result;
        }


        /// <summary>
        ///  通过WMI读取系统信息里的网卡MAC
        /// </summary>
        /// <returns></returns>
        /// 
        public static List<string> GetMacByWMI()
        {
            List<string> macs = new List<string>();
            try
            {
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"])
                    {
                        mac = mo["MacAddress"].ToString();
                        macs.Add(mac);
                    }
                }
                moc = null;
                mc = null;
            }
            catch
            {
            }

            return macs;
        }

        /// <summary>
        ///  获取网卡硬件地址
        /// </summary>
        /// <returns></returns>
        public static string GetComputerUserName()
        {
            try
            {
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    st = mo["UserName"].ToString();
                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {

            }

        }

        /// <summary>
        /// 获得客户端外网IP地址
        /// </summary>
        /// <returns>IP地址</returns>
        public static string getClientInternetIPAddress()
        {

            string internetAddress = "";
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    internetAddress = webClient.DownloadString("http://www.coridc.com/ip");//从外部网页获得IP地址
                                                                                           //判断IP是否合法
                    if (!System.Text.RegularExpressions.Regex.IsMatch(internetAddress, "[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}"))
                    {
                        internetAddress = webClient.DownloadString("http://fw.qq.com/ipaddress");//从腾讯提供的API中获得IP地址
                    }
                }
                return "外网IP地址：" + internetAddress;
            }
            catch
            {
                return "外网IP地址：unknown";
            }

            finally

            {



            }

        }


        /// <summary>
        /// //获取硬盘ID
        /// </summary>
        /// <returns></returns>
        public static string GetDiskID()
        {
            try
            {
                String HDid = "";
                ManagementClass mc = new ManagementClass("Win32_DiskDrive");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    //HDid =(string)mo.Properties["Model"].ToString();
                    HDid = (String)mo.Properties["Model"].Value.ToString();
                }
                moc = null;
                mc = null;
                return HDid;
            }
            catch
            {
                return "unknow";
            }
            finally
            {


            }

        }


        /// <summary>
        /// 获取CPUid
        /// </summary>
        /// <returns></returns>
        public static string GetCpuID()

        {

            try

            {

                //获取CPU序列号代码

                string cpuInfo = "";//cpu序列号
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }

                moc = null;
                mc = null;
                return cpuInfo;

            }
            catch
            {
                return "unknow";
            }

            finally

            {



            }

        }


        /// <summary>
        /// 系统名称
        /// </summary>
        /// <returns></returns>
        public static string GetSystemType()
        {

            try

            {
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    st = mo["SystemType"].ToString();
                }

                moc = null;
                mc = null;
                return st;

            }
            catch
            {
                return "unknow";
            }

            finally
            {

            }

        }


    }
}
