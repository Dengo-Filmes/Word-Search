using System;
using TMPro;
using UnityEngine;

public class WordView : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public bool found { get; private set; } = false;
    public string text { get; private set; } = "";

    [SerializeField] Color foundCollor;
    [SerializeField] float scaleMultiplier = 1.5f;
    [SerializeField] float animationTime = 0.7f;
    [SerializeField] float animationDelay = 0.3f;

    void OnEnable()
    {
        found = false;
    }

    public void SetFound()
    {
        found = true;
        gameObject.GetComponent<TMP_Text>().text = $"<s>{text}</s>";
        gameObject.GetComponent<TMP_Text>().color = foundCollor;
        AnimationRunner.SelectedAnimation(gameObject, scaleMultiplier, animationTime, animationDelay);
    }

    public void SetText(string text)
    {
        this.text = text;
        gameObject.GetComponent<TMP_Text>().text = text;
    }
}
