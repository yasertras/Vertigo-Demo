using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardComponents : MonoBehaviour
{
    [SerializeField]
    private TMP_Text cardLevel;
    [SerializeField]
    private Image card;
    private RectTransform rectTransform;
    public bool isVisible;

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetLevel(int level, Sprite sprite)
    {
        cardLevel.text = level.ToString();
        card.sprite = sprite;
    }

    public void SetLocalPosition(Vector3 setTo)
    {
        rectTransform.anchoredPosition = setTo;
    }

    public Vector3 GetLocalPosition()
    {
        return rectTransform.anchoredPosition;
    }

    public Vector3 GetSizeDelta()
    {
        return rectTransform.sizeDelta;
    }

    public bool IsMasked(RectTransform mask)
    {
        if ((rectTransform.position.x + rectTransform.sizeDelta.x * 0.5f) < (mask.position.x - mask.sizeDelta.x * 0.25f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
