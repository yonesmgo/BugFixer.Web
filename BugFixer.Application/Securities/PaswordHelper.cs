using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Application.Securities
{
    public static class PaswordHelper
    {
        public static string EncodePasswordMD5(string pass)
        {
            Byte[] OrginalByte;
            Byte[] EncodeByte;
            MD5 md5;
            md5 = new MD5CryptoServiceProvider();
            OrginalByte = ASCIIEncoding.ASCII.GetBytes(pass);
            EncodeByte = md5.ComputeHash(OrginalByte);
            return BitConverter.ToString(EncodeByte);
        }
    }
}
