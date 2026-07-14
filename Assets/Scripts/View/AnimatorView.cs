using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;


public class AnimatorView : MonoBehaviour
{
    [Header("Target")]
    [SerializeField]
    GameObject _target;

    [Header("")]
    [SerializeField]
    public float animationTime;
    [SerializeField]
    public float animationDelay;

    [Header("")]
    [SerializeField]
    public AnimationRunner.AnimationType animationType;

    [SerializeField]
    public AnimationRunner.AnimationDirection animationDirection;


    private AnimationRunner _runner;

    private bool notFirstRun = false;


    void Start()
    {
        _runner = new AnimationRunner(
            _target, animationType, animationDirection, animationTime, animationDelay
        );
        _runner.Run();
        notFirstRun = true;
    }

    void OnEnable()
    {
        if (notFirstRun)
        {
            _runner.Run();
        }
    }


}
[Serializable]
public class AnimationRunner
{

    public enum AnimationType
    {
        SlideIn,
        FadeIn,
        ScaleIn,
        ScaleInBounce,
        ScaleOut,
        SquashIn,
        SquashOut
    }

    public enum AnimationDirection
    {
        up,
        down,
        left,
        right,
        clockWise,
        couterClockWise
    }

    [Header("")]
    [SerializeField]
    public float animationTime;
    [SerializeField]
    public float animationDelay;

    [Header("")]
    [SerializeField]
    public AnimationType animationType;

    [SerializeField]
    public AnimationDirection animationDirection;

    private GameObject target;


    public AnimationRunner(GameObject target, AnimationType animationType, AnimationDirection animationDirection, float animationTime, float animationDelay)
    {
        this.target = target;
        this.animationType = animationType;
        this.animationDirection = animationDirection;
        this.animationTime = animationTime;
        this.animationDelay = animationDelay;
    }

    public void Run()
    {
        ProcessAnimation(target, animationType);
    }


    void ProcessAnimation(GameObject target, AnimationType animationType)
    {
        switch (animationType)
        {
            case AnimationType.SlideIn:
                SlideInAnimation(target, animationDirection, animationTime, animationDelay);
                break;
            case AnimationType.FadeIn:
                FadeInAnimation(target, animationTime, animationDelay);
                break;
            case AnimationType.ScaleIn:
                ScaleInAnimation(target, animationTime, animationDelay);
                break;
            case AnimationType.ScaleInBounce:
                ScaleInBounceAnimation(target, animationTime, animationDelay);
                break;
            case AnimationType.ScaleOut:
                ScaleOutAnimation(target, animationTime, animationDelay);
                break;

        }
    }


    public static void ScaleInAnimation(GameObject target, float animationTime, float animationDelay)
    {
        if (target.LeanIsTweening())
        {
            return;
        }
        var originalScale = target.transform.localScale;
        // target.SetActive(false);
        target.transform.localScale = Vector3.zero;
        // var targetScale = new Vector3(originalScale.x * 0.5f, originalScale.y * 0.5f, originalScale.z);
        target.LeanScale(originalScale, animationTime)
        .setEaseOutQuad()
        // .setOnComplete(() =>
        // {
        //     target.LeanScale(originalScale, animationTime)
        //     .setEaseOutQuad();
        // })
        .delay = animationDelay;
    }
    public static void ScaleInBounceAnimation(GameObject target, float animationTime, float animationDelay)
    {
        if (target.LeanIsTweening())
        {
            return;
        }
        var originalScale = target.transform.localScale;
        // target.SetActive(false);
        target.transform.localScale = Vector3.zero;
        // var targetScale = new Vector3(originalScale.x * 0.5f, originalScale.y * 0.5f, originalScale.z);
        target.LeanScale(originalScale, animationTime)
        .setEaseInElastic()
        // .setOnComplete(() =>
        // {
        //     target.LeanScale(originalScale, animationTime)
        //     .setEaseOutQuad();
        // })
        .delay = animationDelay;
    }

    public static void ScaleOutAnimation(GameObject target, float animationTime, float animationDelay)
    {
        if (target.LeanIsTweening())
        {
            return;
        }
        var originalScale = target.transform.localScale;
        var targetScale = new Vector3(originalScale.x * 1.5f, originalScale.y * 1.5f, originalScale.z);
        target.LeanScale(targetScale, animationTime)
        .setEaseOutQuad()
        .setOnComplete(() =>
        {
            target.LeanScale(originalScale, animationTime)
            .setEaseOutQuad();
        })
        .delay = animationDelay;
    }

    public static void FadeInAnimation(GameObject target, float animationTime, float animationDelay)
    {
        if (target.LeanIsTweening())
        {
            return;
        }
        // var originalCollor = target.GetComponent<Image>().color;
        // target.GetComponent<Image>().color = new Color(r: originalCollor.r, g: originalCollor.g, b: originalCollor.b, a: 0f);
        target.GetComponent<CanvasGroup>().alpha = 0;
        target.GetComponent<CanvasGroup>().LeanAlpha(1, animationTime)
        .delay = animationDelay;
    }
    public static void SlideInAnimation(GameObject target, AnimationDirection animationDirection, float animationTime, float animationDelay)
    {
        if (target.LeanIsTweening())
        {
            return;
        }
        target.LeanCancel();
        var originalPos = MoveTarget(target, animationDirection);
        target.LeanMoveLocal(originalPos, animationTime)
        .setEaseOutQuad()
        .delay = animationDelay;
    }

    private static Vector2 MoveTarget(GameObject target, AnimationDirection animationDirection)
    {
        var resp = target.transform.localPosition;
        switch (animationDirection)
        {
            case AnimationDirection.up:
                target.transform.localPosition = new Vector2(resp.x, 2 * Screen.height * -1);
                break;
            case AnimationDirection.down:
                target.transform.localPosition = new Vector2(resp.x, 2 * Screen.height);
                break;
            case AnimationDirection.left:
                target.transform.localPosition = new Vector2(2 * Screen.width, resp.y);
                break;
            case AnimationDirection.right:
                target.transform.localPosition = new Vector2(0 - (2 * Screen.width), resp.y);
                break;
        }
        return resp;
    }

    public static void SelectedAnimation(GameObject target, float scaleMultiplier, float animationTime, float animationDelay)
    {
        target.transform.localScale *= scaleMultiplier;
        target.transform.LeanScale(Vector3.one, animationTime)
        .setEaseOutElastic()
        .setDelay(animationDelay);
    }


}
