using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] ScrollRect textScroll;

    [SerializeField] float scrollSpeed = 1;
    [SerializeField] bool forward = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        AutoScroll();
    }

    void AutoScroll()
    {
        if(forward && textScroll.normalizedPosition.x >= 0.995f) forward = false;
        else if (!forward && textScroll.normalizedPosition.x <= 0.005f) forward = true;

        //textScroll.normalizedPosition = Vector2.MoveTowards(textScroll.normalizedPosition, new Vector2(forward ? 1 : 0, 0), (forward ? scrollSpeed : scrollSpeed * 2) * Time.deltaTime);
        textScroll.normalizedPosition = new Vector2(textScroll.normalizedPosition.x + (forward ? scrollSpeed : -scrollSpeed) * Time.deltaTime, 0);
    }

    public void SetInformation(string name, int score)
    {
        nameText.text = name;
        scoreText.text = "/" + score.ToString();
    }
}
