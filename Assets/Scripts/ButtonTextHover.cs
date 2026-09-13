using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonTextHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text _text;

    [SerializeField] private Color _normalColor = new Color32(255, 244, 222, 255); // #FFF4DE
    [SerializeField] private Color _hoverColor = new Color32(48, 52, 56, 255);    // #303438

    private void Awake()
    {
        if (_text == null)
            _text = GetComponent<TMP_Text>();

        _text.color = _normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _text.color = _hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _text.color = _normalColor;
    }
}