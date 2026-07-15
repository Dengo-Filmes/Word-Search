using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingView : MonoBehaviour
{

    [SerializeField] float timeout;
    private float _timer;
    private bool runTimer = false;

    [SerializeField] GameObject goodEnding;
    [SerializeField] GameObject badEnding;
    [SerializeField] GameObject _transitionObj;

    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DataStorage.Init(HudView.dbFilePath);

    }

    void SetScreen()
    {
        print("setting ending screen");
        var config = DataStorage.GetItem<GeneralConfig>("config", "general");
        print($"config {config.ending}");
        goodEnding.SetActive(false);
        badEnding.SetActive(false);
        if (config.ending == "GoodEnding")
        {
            goodEnding.SetActive(true);
            badEnding.SetActive(false);
            goodEnding.GetComponent<HudScreenView>().PreLoad();
            goodEnding.GetComponent<HudScreenView>().Show();
        }
        else if (config.ending == "BadEnding")
        {
            badEnding.SetActive(true);
            goodEnding.SetActive(false);
            badEnding.GetComponent<HudScreenView>().PreLoad();
            badEnding.GetComponent<HudScreenView>().Show();
        }
        runTimer = true;
        _timer = timeout;
    }

    void OnEnable()
    {
        _transitionObj.GetComponent<TransitionView>().OnAnimationOpenStart.AddListener(SetScreen);
        _transitionObj.GetComponent<TransitionView>().RunOpenTransition();
        // LeanTween.value(0, 1, 0.01f).setOnComplete(
        //     () =>
        //     {
        //     }
        // );
    }

    // Update is called once per frame
    void Update()
    {
        if (runTimer)
        {
            if (_timer > 0 && runTimer)
            {
                _timer -= Time.deltaTime;
            }
            if (_timer <= 0)
            {
                SceneManager.LoadScene(0);
            }
        }
    }

}
