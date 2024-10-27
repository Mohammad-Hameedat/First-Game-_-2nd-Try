using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class SaveLoadManager
{
    private static string saveFilePath = Application.persistentDataPath + "/DontDeleteMe.dat";
    private static readonly string encryptionKey = "FreePalestine369"; // 16-character encryption key

    public static void SaveData(List<int> unlockedLevels)
    {
        GameSaveDataContainer data = new GameSaveDataContainer
        {
            unlockedLevelsList = unlockedLevels
        };

        // Prepare data for saving
        byte[] dataToSave = PrepareDataForSaving(data);

        // Save data to file
        File.WriteAllBytes(saveFilePath, dataToSave);
    }

    public static GameSaveDataContainer LoadData()
    {
        if (File.Exists(saveFilePath))
        {
            // Read data from file
            byte[] dataWithChecksum = File.ReadAllBytes(saveFilePath);

            // Extract data from file
            GameSaveDataContainer data = ExtractDataFromFile(dataWithChecksum);

            return data;
        }
        else
        {
            Debug.Log("No save file found, creating a new one at " + saveFilePath);
            return null;
        }
    }

    private static byte[] PrepareDataForSaving(GameSaveDataContainer data)
    {
        // Serialize data to a binary format
        byte[] serializedData = SerializeData(data);

        // Encrypt serialized data
        byte[] encryptedData = EncryptData(serializedData);

        // Generate and append checksum
        byte[] checksum = GenerateChecksum(encryptedData);
        byte[] dataWithChecksum = AppendChecksum(encryptedData, checksum);

        return dataWithChecksum;
    }

    private static GameSaveDataContainer ExtractDataFromFile(byte[] dataWithChecksum)
    {
        // Separate encrypted data and checksum
        byte[] encryptedData = new byte[dataWithChecksum.Length - 16];
        byte[] checksum = new byte[16];
        Buffer.BlockCopy(dataWithChecksum, 0, encryptedData, 0, encryptedData.Length);
        Buffer.BlockCopy(dataWithChecksum, encryptedData.Length, checksum, 0, checksum.Length);

        // Verify checksum
        if (!VerifyChecksum(encryptedData, checksum))
        {
            Debug.LogError("Save file data is corrupted or tampered with.");
            return null;
        }

        // Decrypt data
        byte[] decryptedData = DecryptData(encryptedData);

        // Deserialize data
        GameSaveDataContainer data = DeserializeData(decryptedData);

        return data;
    }

    private static byte[] SerializeData(GameSaveDataContainer data)
    {
        BinaryFormatter formatter = new();
        using MemoryStream memoryStream = new();
        formatter.Serialize(memoryStream, data);
        return memoryStream.ToArray();
    }

    private static GameSaveDataContainer DeserializeData(byte[] data)
    {
        using MemoryStream memoryStream = new(data);
        BinaryFormatter formatter = new();
        return (GameSaveDataContainer)formatter.Deserialize(memoryStream);
    }

    private static byte[] EncryptData(byte[] data)
    {
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(encryptionKey);
        aes.IV = new byte[16]; // Initialization vector

        using MemoryStream ms = new();
        using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(data, 0, data.Length);
        cs.Close();

        return ms.ToArray();
    }

    private static byte[] DecryptData(byte[] data)
    {
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(encryptionKey);
        aes.IV = new byte[16];

        using MemoryStream ms = new();
        using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(data, 0, data.Length);
        cs.Close();

        return ms.ToArray();
    }

    private static byte[] GenerateChecksum(byte[] data)
    {
        using MD5 md5 = MD5.Create();
        return md5.ComputeHash(data);
    }

    private static bool VerifyChecksum(byte[] data, byte[] checksum)
    {
        byte[] computedChecksum = GenerateChecksum(data);
        return StructuralComparisons.StructuralEqualityComparer.Equals(computedChecksum, checksum);
    }

    private static byte[] AppendChecksum(byte[] data, byte[] checksum)
    {
        byte[] dataWithChecksum = new byte[data.Length + checksum.Length];
        Buffer.BlockCopy(data, 0, dataWithChecksum, 0, data.Length);
        Buffer.BlockCopy(checksum, 0, dataWithChecksum, data.Length, checksum.Length);
        return dataWithChecksum;
    }
}

#region The original code
/* 
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using UnityEngine;

public static class SaveLoadManager
{
    private static string saveFilePath = Application.persistentDataPath + "/DontDeleteMe.dat";

    public static void SaveData(List<int> unlockedLevels)
    {

        GameSaveDataContainer data = new GameSaveDataContainer
        {
            unlockedLevelsList = unlockedLevels
        };

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = new(saveFilePath, FileMode.Create);
        formatter.Serialize(file, data);
        file.Close();
    }

    public static GameSaveDataContainer LoadData()
    {
        bool isSaveFileExists = File.Exists(saveFilePath);

        if (isSaveFileExists)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = new(saveFilePath, FileMode.Open);
            GameSaveDataContainer data = formatter.Deserialize(file) as GameSaveDataContainer;
            file.Close();

            return data;
        }
        else
        {
            Debug.Log("No save file container found, creating a new one at... " + saveFilePath);
            return null;
        }
    }
}
*/
#endregion