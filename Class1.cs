using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    class Class1
    {
        public String SQLCommand_ExecuteNonQuery(String query, SqlConnection con)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();
                return "Success";
            }
            catch (Exception ex)
            {
                WriteToExFile(ex + "");
                return ex.ToString();
            }
        }

        public String SQLCommand_ExecuteScalar(String query, SqlConnection con)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(query, con);
                String temp = cmd.ExecuteScalar() + "";
                return temp;
            }
            catch (Exception ex)
            {
                WriteToExFile(ex + "");
                return ex.ToString();
            }
        }

        public DataTable SQLDataAdapter(String query, SqlConnection con)
        {
            try
            {
                SqlDataAdapter sda = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                WriteToExFile(ex + "");
                DataTable dt = new DataTable();
                return dt;
            }
        }

        public void WriteToExFile(string Message)
        {
            try
            {
                string path = "C:\\Athena Logs\\" + DateTime.Now.ToString("MMMM yyyy");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string filepath = path + "\\Logs_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".txt";
                if (!File.Exists(filepath))
                {
                    using (StreamWriter sw = File.CreateText(filepath))
                    {
                        sw.WriteLine(DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + " : " + Message);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(filepath))
                    {
                        sw.WriteLine(DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + " : " + Message);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToExFile("Export Logfile is in Use : " + ex.ToString());
            }
        }

        public String EncryptPassword(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            AppSettingsReader settingsReader = new AppSettingsReader();
            string key = "WETHEPEOPLEOFINDIAHAVING";

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();

            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public string DecryptPassword(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            string key = "WETHEPEOPLEOFINDIAHAVING";
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
            {
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();

            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public String GetProductLicence(SqlConnection con)
        {
            String licence = "";
            try
            {
                String key = SQLCommand_ExecuteScalar("select Licence_Code from Product_Licence", con);
                if (key != "")
                {
                    licence = DecryptPassword(key, true);
                }
            }
            catch(Exception ex)
            {
                WriteToExFile(ex + "");
            }
            return licence;
        }
    }
}
