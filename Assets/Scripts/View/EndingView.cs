using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingView : MonoBehaviour
{

    [SerializeField] float timeout;
    private float _timer;
    private bool runTimer = false;

    [SerializeField] GameObject goodEnding;
    [SerializeField] GameObject badEnding;

    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DataStorage.Init(HudView.dbFilePath);
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
    }

    void OnEnable()
    {
        runTimer = true;
        _timer = timeout;
    }

    // Update is called once per frame
    void Update()
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
