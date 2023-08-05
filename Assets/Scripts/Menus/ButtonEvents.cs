using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class ButtonEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Options")]

    [Range(0, 1)] public float hoverEnterAlpha;
    [Range(0, 1)] public float hoverExitAlpha;
    // public Text
    [Header("Events")]
    public UnityEvent onHoverEnter;
    public UnityEvent onHoverExit;

    Button _button;
    Image _image;
    // Start is called before the first frame update
    void Awake()
    {
        _button = GetComponent<Button>();
        _image = GetComponent<Image>();
    }

    // This function will be called when the mouse hovers over the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        onHoverEnter.Invoke();
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, hoverEnterAlpha);
    }

    // This function will be called when the mouse exits the button area
    public void OnPointerExit(PointerEventData eventData)
    {
        onHoverExit.Invoke();
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, hoverExitAlpha);
    }
}
