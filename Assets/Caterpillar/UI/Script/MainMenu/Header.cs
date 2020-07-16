using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Header : MonoBehaviour
{
    public float OpenOffset = 0.0f;
    public float CloseOffset = -300.0f;
    public int LayoutCloseOffset = 100;
    public float MovingSpeed = 200.0f;
    public float ScaleDuration = 0.35f;
    public float MaxOpenTime = 10.0f;

    private RectTransform ThisRect;
    public HorizontalLayoutGroup LayoutComponent;
    public RectTransform OtherButton;

    EStatus CurrentStatus = EStatus.Open;
    Coroutine CurrentRoutine = null;

    public enum EStatus
    {
        Open,
        Closing_Scaling,
        Closing_Padding,
        Closing_Offset,
        Closed,
        Opening_Scaling,
        Opening_Padding,
        Opening_Offset
    }

    public void Start()
    {
        ThisRect = GetComponent<RectTransform>();
        StartCoroutine(StartOpenTimeOut());
    }

    public void Open()
    {
        switch (CurrentStatus)
        {
            case EStatus.Opening_Scaling:
            case EStatus.Opening_Padding:
            case EStatus.Opening_Offset:
            case EStatus.Open:
                {
                    return;
                }
            default:
                {
                    if (CurrentRoutine != null)
                    {
                        StopCoroutine(CurrentRoutine);
                    }
                    break;
                }
        }
        switch (CurrentStatus)
        {
            case EStatus.Closed:
            case EStatus.Closing_Scaling:
                {
                    CurrentRoutine = StartCoroutine(Opening_Scaling());
                    break;
                }
            case EStatus.Closing_Offset:
                {
                    CurrentRoutine = StartCoroutine(Opening_Offset());
                    break;
                }
            case EStatus.Closing_Padding:
                {
                    CurrentRoutine = StartCoroutine(Opening_Padding());
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    public IEnumerator Opening_Scaling()
    {
        CurrentStatus = EStatus.Opening_Scaling;

        float StartTime = Time.time;
        float EndTime = StartTime + ScaleDuration;
        while (Time.time < EndTime)
        {
            OtherButton.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, (Time.time - StartTime) / ScaleDuration);
            yield return null;
        }
        OtherButton.localScale = Vector3.one;
        OtherButton.gameObject.SetActive(false);

        yield return Opening_Padding();
    }

    public IEnumerator Opening_Padding()
    {
        CurrentStatus = EStatus.Opening_Padding;
        float PaddingRight = LayoutCloseOffset;
        while (LayoutComponent.padding.right != 0)
        {
            PaddingRight -= (int)(MovingSpeed * Time.deltaTime);
            PaddingRight = Mathf.Clamp(PaddingRight, 0, LayoutCloseOffset);
            LayoutComponent.padding = new RectOffset(0, (int)PaddingRight, 0, 0);
            LayoutComponent.CalculateLayoutInputHorizontal();
            yield return null;
        }

        LayoutComponent.padding = new RectOffset();
        LayoutComponent.CalculateLayoutInputHorizontal();

        yield return Opening_Offset();
    }

    public IEnumerator Opening_Offset()
    {
        CurrentStatus = EStatus.Opening_Offset;
        while (ThisRect.position.x != OpenOffset)
        {
            Vector2 newAncnewPosition = ThisRect.anchoredPosition;
            newAncnewPosition.x += Mathf.Sign(OpenOffset - ThisRect.position.x) * MovingSpeed * Time.deltaTime;
            newAncnewPosition.x = Mathf.Clamp(newAncnewPosition.x, CloseOffset, OpenOffset);
            ThisRect.position = newAncnewPosition;
            yield return null;
        }

        CurrentStatus = EStatus.Open;
        StartCoroutine(StartOpenTimeOut());
    }

    public void Close()
    {
        switch (CurrentStatus)
        {
            case EStatus.Closing_Scaling:
            case EStatus.Closing_Padding:
            case EStatus.Closing_Offset:
            case EStatus.Closed:
                {
                    return;
                }
            default:
                {
                    if (CurrentRoutine != null)
                    {
                        StopCoroutine(CurrentRoutine);
                    }
                    break;
                }
        }
        switch (CurrentStatus)
        {
            case EStatus.Open:
            case EStatus.Opening_Offset:
                {
                    CurrentRoutine = StartCoroutine(Closing_Offest());
                    break;
                }
            case EStatus.Opening_Scaling:
                {
                    CurrentRoutine = StartCoroutine(Closing_Scaling());
                    break;
                }
            case EStatus.Opening_Padding:
                {
                    CurrentRoutine = StartCoroutine(Closing_Padding());
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    public IEnumerator Closing_Offest()
    {
        CurrentStatus = EStatus.Closing_Offset;
        while (ThisRect.position.x != CloseOffset)
        {
            Vector2 newAncnewPosition = ThisRect.position;
            newAncnewPosition.x += Mathf.Sign(CloseOffset - ThisRect.position.x) * MovingSpeed * Time.deltaTime;
            newAncnewPosition.x = Mathf.Clamp(newAncnewPosition.x, CloseOffset, OpenOffset);
            ThisRect.position = newAncnewPosition;
            yield return null;
        }

        yield return Closing_Padding();
    }

    public IEnumerator Closing_Padding()
    {
        CurrentStatus = EStatus.Closing_Padding;
        float PaddingRight = LayoutComponent.padding.right;
        while (LayoutComponent.padding.right != LayoutCloseOffset)
        {
            PaddingRight += (int)(MovingSpeed * Time.deltaTime);
            PaddingRight = Mathf.Clamp(PaddingRight, 0, LayoutCloseOffset);
            LayoutComponent.padding = new RectOffset(0, (int)PaddingRight, 0, 0);
            LayoutComponent.CalculateLayoutInputHorizontal();
            yield return null;
        }

        LayoutComponent.padding = new RectOffset();
        LayoutComponent.CalculateLayoutInputHorizontal();

        yield return Closing_Scaling();
    }

    public IEnumerator Closing_Scaling()
    {
        CurrentStatus = EStatus.Closing_Scaling;
        float StartTime = Time.time;
        float EndTime = StartTime + ScaleDuration;

        OtherButton.gameObject.SetActive(true);
        OtherButton.localScale = Vector3.zero;
        while (Time.time < EndTime)
        {
            OtherButton.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, (Time.time - StartTime) / ScaleDuration);
            yield return null;
        }
        OtherButton.localScale = Vector3.one;

        CurrentStatus = EStatus.Closed;
    }

    public void OpenClose()
    {
        switch (CurrentStatus)
        {
            case EStatus.Open:
            case EStatus.Opening_Scaling:
            case EStatus.Opening_Padding:
            case EStatus.Opening_Offset:
                {
                    Close();
                    break;
                }
            case EStatus.Closed:
            case EStatus.Closing_Scaling:
            case EStatus.Closing_Padding:
            case EStatus.Closing_Offset:
                {
                    Open();
                    break;
                }
        }
    }

    public IEnumerator StartOpenTimeOut()
    {
        yield return new WaitForSeconds(MaxOpenTime);

        Close();
    }
}
