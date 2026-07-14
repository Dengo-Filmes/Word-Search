using System;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class HudScreenView : MonoBehaviour
{


    private bool _executionLock = false;

    [SerializeField]
    public bool isActive;

    [Header("Background")]
    [SerializeField] GameObject _backgroundObj;

    [Header("Background Image")]

    [SerializeField]
    public bool ShowImage = false;

    [SerializeField]
    public Sprite BackgroundImage;

    [SerializeField]
    public GameObject BackgroundImageObject;

    [Header("Background Video")]

    [SerializeField]
    public bool ShowVideo = false;

    [SerializeField]
    public VideoClip VideoClip;
    [SerializeField]
    public string VideoPath;

    [SerializeField]
    public GameObject BackgroundVideoObject;

    [SerializeField, Range(0.0001f, 1f)] float _videoVolume = 0;

    [SerializeField] GameObject _muteBtn;
    [SerializeField] Sprite _soudSprite;
    [SerializeField] Sprite _muteSprite;


    [Header("Content")]
    [SerializeField] GameObject _contentObject;
    private string isMutePerfVar = "volume";

    // [Header("Navigation Hud")]

    // [SerializeField]
    // public bool showNavigationHud;

    // [SerializeField]
    // public bool showHomeButton;

    // [SerializeField]
    // public bool showBackButton;

    // [SerializeField]
    // public bool showStandbyButton;

    // [SerializeField]
    // public bool showVideoButton;

    [Header("standby")]
    [SerializeField]
    bool enableStandby = true;
    [SerializeField]
    float _standbyTimer = 60;

    private bool runTimer = false;
    float _timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isMutePerfVar = $"{isMutePerfVar}-{gameObject.name}";
        if (PlayerPrefs.HasKey(isMutePerfVar) == false)
        {
            PlayerPrefs.SetInt(isMutePerfVar, 0);
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetInt(isMutePerfVar) == 0)
        {
            UnmuteVideo();
        }
        else
        {
            MuteVideo();
        }
    }

    // TODO add reset
    void Update()
    {
        if (enableStandby && runTimer)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                if (!_executionLock)
                {
                    Debug.Log("in standy resetting");
                    // ScreensController.instance.RestartScreen(); \\<-----
                    HudView.instance.RestartScreen();
                    _timer = 0;
                    _executionLock = true;
                }
            }
        }
    }

    private void OnEnable()
    {
        isMutePerfVar = $"{isMutePerfVar}-{gameObject.name}";
        _timer = _standbyTimer;
        runTimer = false;
        if (PlayerPrefs.GetInt(isMutePerfVar) == 0)
        {
            UnmuteVideo();
        }
        else
        {
            MuteVideo();
        }

    }

    void OnDisable()
    {
        BackgroundVideoObject.GetComponent<VideoPlayer>().Stop();
        BackgroundVideoObject.GetComponent<VideoPlayer>().frame = 0;
    }

    public void PreLoad()
    {
        print("Preload");
        _contentObject.SetActive(false);
        _backgroundObj.GetComponent<CanvasGroup>().alpha = 0;
        _backgroundObj.SetActive(true);
        //gameObject.SetActive(isActive);
        if (ShowImage)
        {
            BackgroundVideoObject.SetActive(false);
            BackgroundVideoObject.GetComponent<VideoPlayer>().Stop();
            BackgroundImageObject.GetComponent<Image>().sprite = BackgroundImage;
        }
        else if (ShowVideo)
        {
            BackgroundImageObject.SetActive(false);
            BackgroundVideoObject.SetActive(true);
            if (VideoPath != "")
            {
                print("playing video from path");
                var filePath = Path.Combine(Application.streamingAssetsPath, VideoPath);
                // Debug.LogError(filePath);
                // Debug.LogError(File.Exists(filePath));
                print(filePath);
                BackgroundVideoObject.GetComponent<VideoPlayer>().url = filePath;
            }
            else
            {
                // Debug.LogError("playing from clip");
                // Debug.LogError(VideoClip);
                print("playing from clip");
                BackgroundVideoObject.GetComponent<VideoPlayer>().source = VideoSource.VideoClip;
                BackgroundVideoObject.GetComponent<VideoPlayer>().clip = VideoClip;
            }
            BackgroundVideoObject.GetComponent<VideoPlayer>().Stop();
            BackgroundVideoObject.GetComponent<VideoPlayer>().SetDirectAudioVolume(0, _videoVolume);
            BackgroundVideoObject.GetComponent<VideoPlayer>().Prepare();

            // BackgroundVideoObject.GetComponent<VideoPlayer>().prepareCompleted += (player) =>
            // {
            //     // BackgroundVideoObject.GetComponent<RawImage>().texture = BackgroundVideoObject.GetComponent<VideoPlayer>().texture;
            //     player.Play();
            // };
        }
    }

    public void Show()
    {
        print("Show");
        isActive = true;

        // _backgroundObj.GetComponent<CanvasGroup>().alpha = 1;
        // BackgroundImageObject.SetActive(ShowImage);
        // BackgroundVideoObject.SetActive(ShowVideo);

        // if (showNavigationHud)
        // {
        //     NavigationHudView.instance.SetNavigationHud(showNavigationHud);
        //     NavigationHudView.instance.SetHomeBtn(showHomeButton);
        //     NavigationHudView.instance.SetBackBtn(showBackButton);
        //     NavigationHudView.instance.SetStandbyBtn(showStandbyButton);
        //     NavigationHudView.instance.SetVideoBtn(showVideoButton);

        // }

        // print(string.Format("back: {0} home: {1}", showBackButton, showHomeButton));
        // if (showNavigationHud)
        // {
        //     NavigationHudView.instance.SetBackBtn(showBackButton);
        //     NavigationHudView.instance.SetHomeBtn(showHomeButton);
        //     NavigationHudView.instance.SetStandbyBtn(showStandbyButton);
        //     NavigationHudView.instance.SetVideoBtn(showVideoButton);
        // }
        // else
        // {
        //     NavigationHudView.instance.SetBackBtn(false);
        //     NavigationHudView.instance.SetHomeBtn(false);
        //     NavigationHudView.instance.SetStandbyBtn(false);
        //     NavigationHudView.instance.SetVideoBtn(false);
        // }

        runTimer = true;
        _timer = _standbyTimer;
        _backgroundObj.GetComponent<CanvasGroup>().alpha = 1;

        _contentObject.SetActive(true);
        if (BackgroundVideoObject.activeSelf)
        {
            BackgroundVideoObject.SetActive(ShowVideo);
        }
        if (ShowVideo)
        {
            BackgroundVideoObject.GetComponent<VideoPlayer>().Play();
        }

    }

    public void Hide()
    {
        isActive = false;
        _backgroundObj.GetComponent<CanvasGroup>().alpha = 0;
        _backgroundObj.SetActive(false);
        _contentObject.SetActive(false);
    }

    public void HideContent()
    {
        _contentObject.SetActive(false);
    }

    public void ResetStandby()
    {
        _timer = _standbyTimer;
    }

    public void SetBackButton()
    {

    }

    public void SetBackground(Sprite sprite)
    {
        BackgroundImage = sprite;
    }

    public void SetBackgroundClip(string path)
    {
        this.VideoPath = path;
        print("playing video from path");
        var filePath = Path.Combine(Application.streamingAssetsPath, VideoPath);
        print(filePath);
        BackgroundVideoObject.GetComponent<VideoPlayer>().url = filePath;
        BackgroundVideoObject.GetComponent<VideoPlayer>().Stop();
        BackgroundVideoObject.GetComponent<VideoPlayer>().Prepare();
        BackgroundVideoObject.GetComponent<VideoPlayer>().Play();


    }
    public void SetBackgroundClip(VideoClip clip)
    {
        // this.VideoPath = path;
        // print("playing video from path");
        // var filePath = Path.Combine(Application.streamingAssetsPath, VideoPath);
        // print(filePath);
        BackgroundVideoObject.GetComponent<VideoPlayer>().clip = clip;
        BackgroundVideoObject.GetComponent<VideoPlayer>().Stop();
        BackgroundVideoObject.GetComponent<VideoPlayer>().Prepare();
        BackgroundVideoObject.GetComponent<VideoPlayer>().Play();
    }

    public void MuteVideo()
    {
        if (_muteBtn != null)
        {
            print("muting video");
            BackgroundVideoObject.GetComponent<VideoPlayer>().SetDirectAudioVolume(0, 0);
            _muteBtn.GetComponent<Image>().sprite = _muteSprite;
        }

    }
    public void UnmuteVideo()
    {
        if (_muteBtn != null)
        {
            print("unmutting video");
            BackgroundVideoObject.GetComponent<VideoPlayer>().SetDirectAudioVolume(0, _videoVolume);
            _muteBtn.GetComponent<Image>().sprite = _soudSprite;
        }

    }

    public void HanleMute()
    {
        print(PlayerPrefs.GetInt(isMutePerfVar));
        if (PlayerPrefs.GetInt(isMutePerfVar) == 1)
        {
            UnmuteVideo();
            PlayerPrefs.SetInt(isMutePerfVar, 0);
            PlayerPrefs.Save();
        }
        else
        {
            MuteVideo();
            PlayerPrefs.SetInt(isMutePerfVar, 1);
            PlayerPrefs.Save();
        }
    }
    public void StopStandbyTimer()
    {
        runTimer = false;
    }
    public void StartStandbyTimer()
    {
        runTimer = true;
    }

    public bool IsTimerRunning()
    {
        return runTimer;
    }
}
