using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIAnimateButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private bool isPressed;
    [SerializeField] private DOTweenAnimator startPressAnimation, cancelPress, confirmPress;

    private void Awake()
    {
        confirmPress.OnFinishAnimation += DoSomething;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        startPressAnimation.Animate();
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (isPressed)
        {
            isPressed = false;

            cancelPress.Animate();
        }
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if (isPressed)
        {
            isPressed = false;

            confirmPress.Animate();
        }
    }

    protected abstract void DoSomething();
}
