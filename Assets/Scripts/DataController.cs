using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class DataController : MonoBehaviour
{
    public static DataController Instance;
    LeaderboardGoogleFile googleFile;

    public UserData currentUser;

    public List<UserData> players = new();
    [SerializeField] RawImage texture;
    [SerializeField] string menuClip;
    [SerializeField] string standbyClip;

    [SerializeField] [TextArea] string loadedData;

    const string leaderboardID = "Online_Players";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }

        googleFile = GetComponent<LeaderboardGoogleFile>();

        //await UnityServices.InitializeAsync();
        //await AuthenticationService.Instance.SignInAnonymouslyAsync();
        //await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardID, 0);

        //LoadDataAsync();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //LoadData();
        ChangeBackground(standbyClip);
    }

    // Update is called once per frame
    void Update()
    {
        if (texture == null)
        {
            texture = GameObject.FindGameObjectWithTag("Finish").GetComponent<RawImage>();
        } else
            texture.texture = GetComponent<VideoPlayer>().texture;
    }

    public void ChangeBackground(string URL)
    {
        VideoPlayer player = GetComponent<VideoPlayer>();

        string url = System.IO.Path.Combine(Application.streamingAssetsPath, "Videos", URL);
        player.url = url;

        player.Play();
    }

    public void ChangeBackground()
    {
        VideoPlayer player = GetComponent<VideoPlayer>();

        string url = System.IO.Path.Combine(Application.streamingAssetsPath, "Videos", menuClip);
        player.url = url;

        player.Play();
    }

    public void SetStandby()
    {
        ChangeBackground(standbyClip);
    }

    public void SetUserData(UserData userData)
    {
        currentUser = userData;
        players.Add(currentUser);
        
        //AuthenticationService.Instance.UpdatePlayerNameAsync(userData.username + "," + userData.ID);
        //print(AuthenticationService.Instance.PlayerName);

        print("<color=cyan>USUÁRIO CRIADO");
    }

    public void SaveUserData(int score)
    {
        currentUser.SetScore(score);
        // Save to file
        //SaveData();
        SaveGoogleData();
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

    void SaveGoogleData()
    {
        string data = "";
        players = players.OrderByDescending(x => x.score).ToList();

        for (int i = 0; i < players.Count; i++)
        {
            data += "\n" + players[i].username + "," + players[i].ID + "," + players[i].shift.ToString() + "," + players[i].score.ToString();
        }

        googleFile.UploadLeaderboard(data);
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

    public void LoadGoogleData()
    {
        googleFile.DownloadLeaderboard();
    }

    public void GenerateGoogleData(string loadData)
    {
        loadedData = loadData;

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

    //public async void LoadDataAsync()
    //{
    //    try
    //    {
    //        var players = await LeaderboardsService.Instance.GetScoresAsync(leaderboardID);
    //        if(players.Results.Count <= 0) return;
    //        this.players.Clear();
    //        for (int i = 0; i < players.Results.Count; i++)
    //        {
    //            List<string> lineData = players.Results[i].PlayerName.Split(",").ToList<string>();
    //            UserData data = new UserData(lineData[0], lineData[1], 0);
    //            data.SetScore((int)players.Results[i].Score);
    //            this.players.Add(data);
    //        }

    //        this.players = this.players.OrderByDescending(x => x.score).ToList();
    //        FindFirstObjectByType<MainMenuController>().CreateLeaderboard();
    //    }
    //    catch (System.Exception e)
    //    {
    //        Debug.Log(e.Message);
    //        throw;
    //    }
    //}

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
