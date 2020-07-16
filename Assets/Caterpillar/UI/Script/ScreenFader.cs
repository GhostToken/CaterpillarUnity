using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    private static ScreenFader Instance;

    public Image Image;
    public float FadeDuration = 1.0f;

    private Coroutine CurrentProcess;

    public static void Launch_FadeIn(Action OnFadeEnd)
    {
        Instance.StartFadeIn(OnFadeEnd);
    }

    public static void Launch_FadeOut(Action OnFadeEnd)
    {
        Instance.StartFadeOut(OnFadeEnd);
    }

    void StartFadeIn(Action OnFadeEnd)
    {
        if(CurrentProcess != null)
        {
            StopCoroutine(CurrentProcess);
            CurrentProcess = null;
        }

        CurrentProcess = StartCoroutine(FadeIn(OnFadeEnd));
    }

    void StartFadeOut(Action OnFadeEnd)
    {
        if (CurrentProcess != null)
        {
            StopCoroutine(CurrentProcess);
            CurrentProcess = null;
        }

        CurrentProcess = StartCoroutine(FadeOut(OnFadeEnd));
    }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        StartFadeOut(null);
    }

    IEnumerator FadeIn(Action OnFadeEnd)
    {
        Image.color = Color.clear;

        float time = 0.0f;
        while (time < FadeDuration)
        {
            time += Time.deltaTime;
            Image.color = Color.Lerp(Color.clear, Color.black, time / FadeDuration);
            yield return null;
        }

        Image.color = Color.black;
        CurrentProcess = null;

        if(OnFadeEnd != null)
        {
            OnFadeEnd();
        }
    }

    IEnumerator FadeOut(Action OnFadeEnd)
    {
        Image.color = Color.black;

        float time = 0.0f;
        while (time < FadeDuration)
        {
            time += Time.deltaTime;
            Image.color = Color.Lerp(Color.black, Color.clear, time / FadeDuration);
            yield return null;
        }

        Image.color = Color.clear;
        CurrentProcess = null;

        if (OnFadeEnd != null)
        {
            OnFadeEnd();
        }
    }
}
