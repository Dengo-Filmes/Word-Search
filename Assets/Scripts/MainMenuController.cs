using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] CanvasGroup signinCanvas;
    [SerializeField] RectTransform inputPanel;

    [Header("Sign-in")]
    [SerializeField] TMP_InputField nameField;
    [SerializeField] TMP_InputField IDField;
    [SerializeField] TMP_Dropdown shiftDropdown;

    [Header("Leaderboard")]
    [SerializeField] GameObject infoPrefab;
    [SerializeField] RectTransform leaderboard;
    [SerializeField] int leaderboardMax = 5;

    [Space(15)]
    [SerializeField] LeanTweenType easeType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DataController.Instance.LoadData();
    }

    // Update is called once per frame
    void Update()
    {
        
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
            LeanTween.alphaCanvas(signinCanvas, 1, 0.5f).setOnComplete(() =>
            {
                LeanTween.size(inputPanel, new Vector2(640, 768), 0.5f).setOnComplete(() =>
                {
                    LeanTween.alphaCanvas(inputPanel.GetChild(0).GetComponent<CanvasGroup>(), 1, 0.5f).setEase(easeType);
                    signinCanvas.blocksRaycasts = true;
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
        SceneManager.LoadScene(1);
    }
}