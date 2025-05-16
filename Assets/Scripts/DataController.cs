using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Overlays;
using UnityEngine;

public class DataController : MonoBehaviour
{
    public static DataController Instance;

    public UserData currentUser;

    public List<UserData> players = new();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //LoadData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUserData(UserData userData)
    {
        currentUser = userData;
        players.Add(currentUser);
        
        print("<color=cyan>USUÁRIO CRIADO");
    }

    public void SaveUserData(int score)
    {
        currentUser.SetScore(score);
        // Save to file
        SaveData();
    }

    public UserData GetUserData()
    {
        return currentUser;
    }

    void SaveData()
    {
        string data = "";
        players = players.OrderByDescending(x => x.score).ToList();

        for (int i = 0; i < players.Count; i++)
        {
            data += "\n" + players[i].username + "," + players[i].ID + "," + players[i].shift.ToString() + "," + players[i].score.ToString();
        }

        File.WriteAllText(Application.dataPath + "/savedata.txt", data);
    }

    public void LoadData()
    {
        if (!File.Exists(Application.dataPath + "/savedata.txt")) return;
        string loadData = File.ReadAllText(Application.dataPath + "/savedata.txt");

        players.Clear();

        List<string> lines = loadData.Split('\n').ToList<string>();
        for (int i = 1; i < lines.Count; i++)
        {
            List<string> lineData = lines[i].Split(',').ToList<string>();
            UserData user = new UserData(lineData[0], lineData[1], int.Parse(lineData[2]));
            user.SetScore(int.Parse(lineData[3]));
            players.Add(user);

        }

        players = players.OrderByDescending(x => x.score).ToList();
        FindFirstObjectByType<MainMenuController>().CreateLeaderboard();
    }

    public List<UserData> GetUsers()
    {
        return players;
    }
}

[System.Serializable]
public class UserData
{
    public string username;
    public string ID;
    public int shift;
    public int score = 0;

    public UserData(string username, string ID, int shift)
    {
        this.username = username;
        this.ID = ID;
        this.shift = shift;
    }

    public void SetScore(int score)
    {
        this.score = score;
    }
}
