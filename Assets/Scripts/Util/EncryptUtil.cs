using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EncryptUtil
{
    /// <summary>
    /// 加密KEY
    /// </summary>
    public static uint EncryptKey = EncryptUtil.StrHash("GEMAGAME");

    const uint FNV32_PRIME = 0x01000193;
    const uint FNV32_OFFSET_BASIS = 0x811c9dc5;

    /// <summary>
    /// 计算文件地址hash
    /// </summary>
    /// <param name="buf"></param>
    /// <returns></returns>
    public static uint FileNameHash(string fileName)
    {
        fileName = fileName.ToLower();
        uint crc32 = FNV32_OFFSET_BASIS;
        for (int i = 0; i < fileName.Length; i++)
        {
            char c = fileName[i];

            if (c == '/')
            {
                c = '\\';
            }

            crc32 *= FNV32_PRIME;
            crc32 ^= c;
        }

        return crc32;
    }

    /// <summary>
    /// 计算文字符串hash
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static uint StrHash(string str)
    {
        uint crc32 = FNV32_OFFSET_BASIS;
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            crc32 *= FNV32_PRIME;
            crc32 ^= c;
        }
        return crc32;
    }

    /// <summary>
    /// 字节加密
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="pwd"></param>
    /// <returns></returns>
    public static bool Encryption(byte[] buffer)
    {
        int len = buffer.Length;
        byte[] key = new byte[]
        {
            (byte)((EncryptKey & 0xff000000) >> 24),
            (byte)((EncryptKey & 0x00ff0000) >> 16),
            (byte)((EncryptKey & 0x0000ff00) >> 8),
            (byte)((EncryptKey & 0x000000ff) >> 0)
        };

        for (int i = 0; i < len; i++)
        {
            buffer[i] ^= key[i % 4];
        }
        return true;
    }

    /// <summary>
    /// 字节解密
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static bool Decryption(byte[] buffer)
    {
        return Encryption(buffer);
    }
}
