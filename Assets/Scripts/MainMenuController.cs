using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenuController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] CanvasGroup signinCanvas;
    [SerializeField] CanvasGroup menuCanvas;
    [SerializeField] RectTransform inputPanel;
    [SerializeField] Animator transitionCanvas;

    [Header("Sign-in")]
    [SerializeField] TMP_InputField nameField;
    [SerializeField] TMP_InputField IDField;
    [SerializeField] TMP_Dropdown shiftDropdown;

    [Header("Rules")]
    [SerializeField] CanvasGroup ruleCanvas;

    [Header("Leaderboard")]
    [SerializeField] GameObject infoPrefab;
    [SerializeField] RectTransform leaderboard;
    [SerializeField] int leaderboardMax = 5;

    [Header("Standby")]
    [SerializeField] float standbyWaitTime = 30;
    float s_timer;
    bool onStandby = true;

    [Space(15)]
    [SerializeField] LeanTweenType easeType;
    [SerializeField] VideoClip backgroundClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            DataController.Instance.LoadData();
        SetStandby(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(signinCanvas.alpha < 1 && !onStandby)
        {
            if (s_timer < standbyWaitTime) s_timer += Time.deltaTime;
            else
            {
                if(!onStandby) 
                    SetStandby(true);
            }
        }
    }

    public void SetStandby(bool standby)
    {
        if (standby)
            DataController.Instance.SetStandby();
        else
            DataController.Instance.ChangeBackground();

        menuCanvas.alpha = standby ? 0 : 1;
        menuCanvas.blocksRaycasts = !standby;

        if (!standby) s_timer = 0;

        onStandby = standby;
    }

    public void CreateUser()
    {
        if (string.IsNullOrWhiteSpace(nameField.text) || string.IsNullOrWhiteSpace(IDField.text) || shiftDropdown.value == 0)
        {
            print("<color=red>PREENCHER OS CAMPOS CORRETAMENTE");
            return;
        }

        UserData thisUser = new(nameField.text, IDField.text, shiftDropdown.value);
        DataController.Instance.SetUserData(thisUser);

        StartGameButton();
    }

    public void StartGameButton(bool start)
    {
        if (start)
        {
            s_timer = 0;

            LeanTween.alphaCanvas(signinCanvas, 1, 0.5f).setOnComplete(() =>
            {
                LeanTween.size(inputPanel, new Vector2(640, 640), 0.5f).setOnComplete(() =>
                {
                    LeanTween.alphaCanvas(inputPanel.GetChild(0).GetComponent<CanvasGroup>(), 1, 0.5f).setEase(easeType);
                    signinCanvas.blocksRaycasts = true;

                    System.Diagnostics.Process.Start("OSK.exe");
                }).setEase(easeType);
            }).setEase(easeType);
        }
        else
        {
            LeanTween.alphaCanvas(inputPanel.GetChild(0).GetComponent<CanvasGroup>(), 0, 0.5f).setOnComplete(() =>
            {
                LeanTween.size(inputPanel, new Vector2(640, 0), 0.5f).setOnComplete(() =>
                {
                    LeanTween.alphaCanvas(signinCanvas, 0, 0.5f).setEase(easeType);
                    signinCanvas.blocksRaycasts = false;
                }).setEase(easeType);

            }).setEase(easeType);
        }
    }

    public void OpenCloseRules(bool open)
    {
        LeanTween.alphaCanvas(ruleCanvas, open ? 1 : 0, 0.5f).setEaseInOutCirc();
        ruleCanvas.blocksRaycasts = open;
    }

    public void CreateLeaderboard()
    {
        for (int i = 0; i < DataController.Instance.GetUsers().Count; i++)
        {
            if (i >= leaderboardMax) break;
            LeaderboardEntry thisEntry = Instantiate(infoPrefab, leaderboard).GetComponent<LeaderboardEntry>();
            thisEntry.SetInformation(DataController.Instance.GetUsers()[i].username, DataController.Instance.GetUsers()[i].score);
        }
    }

    public void StartGameButton()
    {
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        transitionCanvas.Play("Transition_open");

        yield return new WaitForSeconds(0.7f);

        DataController.Instance.ChangeBackground(backgroundClip);
        SceneManager.LoadScene(1);
    }
}