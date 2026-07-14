using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToutchButton : MonoBehaviour
{

    [SerializeField] Button _btn;
    [SerializeField] UnityEvent onClick;
    [SerializeField] float _debouce = 0.3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {

    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }

    void OnEnable()
    {
        _btn.onClick.RemoveAllListeners();
        _btn.onClick.AddListener(() =>
        {
            StartCoroutine(Click());
        });
    }
    IEnumerator Click()
    {
        print("calling click");
        _btn.interactable = false;
        onClick.Invoke();
        yield return new WaitForSeconds(_debouce);
        _btn.interactable = true;
    }
}
