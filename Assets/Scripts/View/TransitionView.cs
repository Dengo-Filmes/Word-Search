using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public class TransitionView : MonoBehaviour
{

    public static TransitionView instance;

    [SerializeField] GameObject _transitionObj;

    [Header("hooks")]
    [SerializeField] public UnityEvent OnAnimationCloseStart;
    [SerializeField] public UnityEvent OnAnimationCloseEnd;
    [SerializeField] public UnityEvent OnAnimationOpenStart;
    [SerializeField] public UnityEvent OnAnimationOpenEnd;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        gameObject.SetActive(true);
        print("instance");
    }

    // Update is called once per frame
    void Update()
    {

    }

    // public void RunTranstion()
    // {

    // }

    public void RunCloseTransition()
    {
        gameObject.GetComponent<Animator>().Play("TransitionClose");

    }

    public void RunOpenTransition()
    {
        gameObject.GetComponent<Animator>().Play("TransitionOpen");
    }

    public void AnimationCloseStartState()
    {
        print("Animation Close started");
        _transitionObj.GetComponent<CanvasGroup>().blocksRaycasts = true;
        _transitionObj.GetComponent<CanvasGroup>().alpha = 1;
        OnAnimationCloseStart.Invoke();
    }
    public void AnimationCloseEndState()
    {
        print("Animation Close ended");
        OnAnimationCloseEnd.Invoke();
    }
    public void AnimationOpenStartState()
    {
        _transitionObj.GetComponent<CanvasGroup>().alpha = 1;
        _transitionObj.GetComponent<CanvasGroup>().blocksRaycasts = true;
        print("Animation Open started");
        OnAnimationOpenStart.Invoke();
    }
    public void AnimationOpenEndState()
    {
        print("Animation Open ended");
        OnAnimationOpenEnd.Invoke();
        _transitionObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
        _transitionObj.GetComponent<CanvasGroup>().alpha = 0;
    }
}
