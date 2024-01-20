using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class FileManager
{
    protected string path;
    private static readonly string key = "VoxelTown";

    protected void CreateFolderTree()
    {
        CreateDirectory("/Save/");
        CreateDirectory("/Save/Data/");
    }

    protected void DeleteFolderTree()
    {
        DeleteDirectory("/Save/", true);
    }

    private void CreateDirectory(string path)
    {
        if (!ExistsDirectory(path))
        {
            Directory.CreateDirectory(Application.persistentDataPath + path);
        }
    }

    private void DeleteDirectory(string path, bool complete = false)
    {
        if (ExistsDirectory(path))
        {
            Directory.Delete(Application.persistentDataPath + path, complete);
        }
    }

    public void Save()
    {
        Encrypt(JsonUtility.ToJson(this), path);

        //Save without encrypt
        //if(File.Exists(path))
        //{
        //    File.Delete(path);
        //}
        //File.WriteAllText(path, JsonUtility.ToJson(this));
    }
    public bool Load()
    {
        if (File.Exists(path))
        {
            JsonUtility.FromJsonOverwrite(Decrypt(path), this);

            //Load unencrypted
            //JsonUtility.FromJsonOverwrite(File.ReadAllText(path), this);

            return true;
        }
        return false;
    }

    private bool ExistsDirectory(string path) => Directory.Exists(Application.persistentDataPath + path);

    private static void Encrypt(string data, string path)
    {
        byte[] dataInBytes = Encoding.UTF8.GetBytes(data);
        byte[] keyInBytes = Encoding.UTF8.GetBytes(key);

        MD5CryptoServiceProvider hashMD5 = new();
        keyInBytes = hashMD5.ComputeHash(Encoding.UTF8.GetBytes(key));
        hashMD5.Clear();

        TripleDESCryptoServiceProvider TDes = new()
        {
            Key = keyInBytes,
            Mode = CipherMode.ECB,
            Padding = PaddingMode.PKCS7
        };

        ICryptoTransform encrypt = TDes.CreateEncryptor();
        byte[] encrypterData = encrypt.TransformFinalBlock(dataInBytes, 0, dataInBytes.Length);

        if (File.Exists(path)) {
            File.Delete(path);
        }
        string base64EncryptedData = Convert.ToBase64String(encrypterData);
        File.WriteAllText(path, base64EncryptedData);
        TDes.Clear();
    }

    private static string Decrypt(string path) 
    {
        byte[] keyInBytes;

        MD5CryptoServiceProvider hashMD5 = new();
        keyInBytes = hashMD5.ComputeHash(Encoding.UTF8.GetBytes(key));
        hashMD5.Clear();

        TripleDESCryptoServiceProvider TDes = new()
        {
            Key = keyInBytes,
            Mode = CipherMode.ECB,
            Padding = PaddingMode.PKCS7
        };

        byte[] encryptedData = Convert.FromBase64String(File.ReadAllText(path));

        ICryptoTransform dedecrypt = TDes.CreateDecryptor();
        byte[] decryptedData = dedecrypt.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

        TDes.Clear();

        return Encoding.UTF8.GetString(decryptedData);
    }
}
