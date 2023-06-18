using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject blurBackground;

    private List<IWindow> windows;

    private void Awake()
    {
        InitWindows();
    }

    public void SetBlurState(bool state)
    {
        blurBackground.SetActive(state);
    }

    private void GetWindows()
    {
        windows = GetComponentsInChildren<IWindow>().ToList();
    }

    private void InitWindows()
    {
        GetWindows();

        foreach (var window in windows)       
        {
            window.Init(this);
        }
    }
}
