using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public static class DataStorage
{
    private static Dictionary<string, Dictionary<string, JToken>> _keyChainInstance;
    private static System.Object _lock = new();

    public static void Init(string path)
    {
        LoadKeyChain(path);
    }

    private static void Load(string path)
    {
        var jsonData = File.ReadAllText(path);
        _keyChainInstance = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, JToken>>>(jsonData);
    }

    private static void Save(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }
        var jsonString = JsonConvert.SerializeObject(_keyChainInstance, Newtonsoft.Json.Formatting.Indented);

        File.WriteAllText(path, jsonString);
    }

    public static void LoadKeyChain(string path)
    {
        lock (_lock)
        {
            if (File.Exists(path))
            {
                Load(path);
            }
            else
            {
                _keyChainInstance = new Dictionary<string, Dictionary<string, JToken>>();
                Save(path);
            }
        }
    }
    public static void SaveKeyChain(string path)
    {
        lock (_lock)
        {
            Save(path);
        }
    }

    public static void AddKeyChain(string key)
    {
        lock (_lock)
        {
            if (!_keyChainInstance.ContainsKey(key))
            {
                _keyChainInstance[key] = new Dictionary<string, JToken>();
            }
        }
    }

    public static bool Exists(string key)
    {
        lock (_lock)
        {
            return _keyChainInstance.ContainsKey(key);
        }
    }

    public static void AddItem<T>(string keyChain, string keyId, T value)
    {
        lock (_lock)
        {
            if (!_keyChainInstance.ContainsKey(keyChain))
            {
                _keyChainInstance[keyChain].Add(keyId, JToken.FromObject(value));
            }
            else
            {
                _keyChainInstance[keyChain].Remove(keyId);
                _keyChainInstance[keyChain].Add(keyId, JToken.FromObject(value));
            }
        }
    }
    public static T? GetItem<T>(string keyChain, string keyId)
    {
        lock (_lock)
        {
            if (_keyChainInstance.ContainsKey(keyChain) && _keyChainInstance[keyChain].ContainsKey(keyId))
            {

                return (T)_keyChainInstance[keyChain][keyId].ToObject(typeof(T));
            }
            else { return default(T); }
        }
    }
    //public static T? GetItem<T>(string keyChain, string keyId) where T : class 
    //{
    //    lock (_lock)
    //    {
    //        if (_keyChainInstance.ContainsKey(keyChain) && _keyChainInstance[keyChain].ContainsKey(keyId))
    //        {

    //            return (T)_keyChainInstance[keyChain][keyId].ToObject(typeof(T));
    //        }
    //        else { return default(T); }
    //    }

    //}

}



public class TestDataBase
{
    private static string filePath = Path.Join(Application.dataPath, "DB", "test.json");
    public static void TestLoadStorage()
    {
        DataStorage.Init(filePath);
        Console.WriteLine(DataStorage.GetItem<int>("chain1", "int1"));
        Console.WriteLine(DataStorage.GetItem<bool>("chain1", "boll"));
        Console.WriteLine(DataStorage.GetItem<double>("chain1", "float"));
        Console.WriteLine(DataStorage.GetItem<List<int>>("chain1", "list"));
        Console.WriteLine(DataStorage.GetItem<Dictionary<string, int>>("chain1", "dict"));
        Console.WriteLine(DataStorage.GetItem<GameData>("chain1", "GameData"));
    }
    public static void TestNewStorage()
    {
        DataStorage.Init(filePath);
        DataStorage.AddKeyChain("chain1");
        DataStorage.AddItem("chain1", "int1", 1);
        Console.WriteLine(DataStorage.GetItem<int>("chain1", "int1"));
        DataStorage.AddItem("chain1", "bool", false);
        Console.WriteLine(DataStorage.GetItem<bool>("chain1", "boll"));
        DataStorage.AddItem("chain1", "float", 1f);
        Console.WriteLine(DataStorage.GetItem<double>("chain1", "float"));
        DataStorage.AddItem("chain1", "list", new List<int>() { 1, 2, 3, 4, 5 });
        Console.WriteLine(DataStorage.GetItem<List<int>>("chain1", "list"));
        DataStorage.AddItem("chain1", "dict", new Dictionary<string, int>() { { "item1", 1 }, { "item2", 2 }, { "item3", 3 }, { "item4", 4 } });
        Console.WriteLine(DataStorage.GetItem<Dictionary<string, int>>("chain1", "dict"));
        DataStorage.AddItem("chain1", "GameData", new GameData(10, 9999, new Dictionary<int, string>() { { 0, "peixe" }, { 1, "unobtenioum" } }));
        Console.WriteLine(DataStorage.GetItem<GameData>("chain1", "GameData"));
        DataStorage.SaveKeyChain(filePath);

    }
}
[Serializable]
public class GameData
{
    public int health;
    public int monney;

    public Dictionary<int, string> inventory = new();

    public GameData(int health, int monney, Dictionary<int, string> inventory)
    {
        this.health = health;
        this.monney = monney;
        this.inventory = inventory;
    }

    public override string ToString()
    {
        return String.Format("health: {0} monney: {1}", health, monney);
    }
}
