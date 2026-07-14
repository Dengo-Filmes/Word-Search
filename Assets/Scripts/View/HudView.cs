using UnityEngine;
using System.IO;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine.SceneManagement;
using System;

public class HudView : MonoBehaviour
{

    public static HudView instance;
    [Header("Screens")]
    [SerializedDictionary("Screen ID", "Screen")]
    public SerializedDictionary<string, GameObject> _screens;
    [SerializeField] string standbyScreen;
    private string _currentScreen;

    private bool _firstStart = false;
    public static string dbFilePath { get; private set; } = Path.Join(Application.streamingAssetsPath, "DB", "db.json");
    private DateTime date1 = DateTime.Parse("2026-07-18 22:00:00");

    [SerializeField] GameObject _transitionObj;

    // private DateTime date1 = DateTime.Parse("2026-07-05 22:00:00");
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
        Application.targetFrameRate = 60;
        _currentScreen = standbyScreen;
        SetupDb();


        // TestDataBase.TestNewStorage();
        // TestDataBase.TestLoadStorage();
        // print(DataStorage.GetItem<Dictionary<string, int>>("chain1", "dict")["item1"]);
    }

    // private void FixedUpdate()
    // {
    //     foreach (var screen in _screens)
    //     {
    //         if (screen.Value != null)
    //         {
    //             if (screen.Value.activeSelf)
    //             {
    //                 return;
    //             }
    //         }
    //     }
    //     print("all screens are dead");
    //     RestartScreen();
    // }

    // Update is called once per frame
    void Update()
    {

    }

    void Start()
    {
        if (!_firstStart)
        {
            foreach (var screen in _screens)
            {
                if (screen.Value == null)
                {
                    print(string.Format("{0} screen does not exist", screen.Key));
                    continue;
                }
                screen.Value.GetComponent<HudScreenView>().Hide();
                screen.Value.SetActive(false);
            }
            _firstStart = true;
        }
        // SetScreen(standbyScreen);
        _transitionObj.GetComponent<TransitionView>().OnAnimationOpenStart.AddListener(() =>
        {
            _currentScreen = standbyScreen;
            print($"starting screen {standbyScreen}");
            _screens[_currentScreen].SetActive(true);
            _screens[_currentScreen].GetComponent<HudScreenView>().PreLoad();
            _screens[_currentScreen].GetComponent<HudScreenView>().Show();
        });
        _transitionObj.GetComponent<TransitionView>().RunOpenTransition();
        // _transitionObj.GetComponent<TransitionView>().
    }

    public void RestartScreen()
    {

        _transitionObj.GetComponent<TransitionView>().RunCloseTransition();
        _transitionObj.GetComponent<TransitionView>().OnAnimationCloseEnd.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
    }

    private void SetupDb()
    {
        DataStorage.Init(dbFilePath);
        if (!DataStorage.Exists("config"))
        {
            DataStorage.AddKeyChain("config");
            DataStorage.AddItem("config", "general", new GeneralConfig());
            DataStorage.SaveKeyChain(dbFilePath);
        }
        // var artes = DataStorage.GetItem<Artes>("SALAO DE ARTES SUSTENTAVEIS", "dados");
        // print(artes.artes);
        // print(DataStorage.GetItem<Dictionary<string>>)
        // print(artes.artes["1"].id);

    }

    public void SetScreen(string screenId)
    {
        _transitionObj.GetComponent<TransitionView>().OnAnimationOpenStart.RemoveAllListeners();
        _transitionObj.GetComponent<TransitionView>().OnAnimationOpenEnd.RemoveAllListeners();
        _transitionObj.GetComponent<TransitionView>().OnAnimationCloseStart.RemoveAllListeners();
        _transitionObj.GetComponent<TransitionView>().OnAnimationCloseEnd.RemoveAllListeners();

        _transitionObj.GetComponent<TransitionView>().OnAnimationCloseEnd.AddListener(() =>
        {
            print($"closing screen {_currentScreen}");
            _screens[_currentScreen].SetActive(false);
            _transitionObj.GetComponent<TransitionView>().RunOpenTransition();
        });
        _transitionObj.GetComponent<TransitionView>().OnAnimationOpenStart.AddListener(() =>
        {
            _currentScreen = screenId;
            print($"starting screen {screenId}");
            _screens[_currentScreen].SetActive(true);
            _screens[_currentScreen].GetComponent<HudScreenView>().PreLoad();
            _screens[_currentScreen].GetComponent<HudScreenView>().Show();
        });
        _transitionObj.GetComponent<TransitionView>().RunCloseTransition();
    }

    public void SendToGame()
    {
        _transitionObj.GetComponent<TransitionView>().OnAnimationCloseEnd.AddListener(() =>
        {
            SceneManager.LoadScene(1);
        });
        _transitionObj.GetComponent<TransitionView>().RunCloseTransition();
    }

    // public void SendToVoting(Obra obra)
    // {
    //     var screenId = "Voting";
    //     _transitionObj.GetComponent<TransitionView>().OnAnimationOpenStart.RemoveAllListeners();
    //     _transitionObj.GetComponent<TransitionView>().OnAnimationOpenEnd.RemoveAllListeners();
    //     _transitionObj.GetComponent<TransitionView>().OnAnimationCloseStart.RemoveAllListeners();
    //     _transitionObj.GetComponent<TransitionView>().OnAnimationCloseEnd.RemoveAllListeners();

    //     _transitionObj.GetComponent<TransitionView>().OnAnimationCloseEnd.AddListener(() =>
    //     {
    //         print($"closing screen {_currentScreen}");
    //         _screens[_currentScreen].SetActive(false);
    //         _transitionObj.GetComponent<TransitionView>().RunOpenTransition();
    //     });
    //     _transitionObj.GetComponent<TransitionView>().OnAnimationOpenStart.AddListener(() =>
    //     {
    //         _currentScreen = screenId;
    //         print($"starting screen {screenId}");
    //         _screens[_currentScreen].SetActive(true);
    //         VotingView.instance.ShowObra(obra);
    //         _screens[_currentScreen].GetComponent<HudScreenView>().PreLoad();
    //         _screens[_currentScreen].GetComponent<HudScreenView>().Show();


    //     });
    //     _transitionObj.GetComponent<TransitionView>().RunCloseTransition();
    // }


    // public void SendToForm()
    // {
    //     print(date1);
    //     var now = DateTime.Now;
    //     if (now >= date1)
    //     {
    //         print("votacao encerrada");
    //         SetScreen("Encerrada");
    //     }
    //     else
    //     {
    //         SetScreen("Cadastro");
    //     }
    // }
}
