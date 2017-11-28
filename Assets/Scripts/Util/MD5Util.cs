using System;
using System.Security.Cryptography;

public static class MD5Util
{
    public static string MD5(byte[] source)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] output = md5.ComputeHash(source);
        string Text = BitConverter.ToString(output).Replace("-", "");
        return Text;
    }
}