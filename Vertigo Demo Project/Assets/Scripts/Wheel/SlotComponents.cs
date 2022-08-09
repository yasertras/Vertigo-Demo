using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class SlotComponents : MonoBehaviour
{
    [SerializeField]
    private Image mainImage;
    [SerializeField]
    private TMP_Text slotText;
    public CollectableObjects objectType;
    private int amount = 0;

    public void Clear()
    {
        amount = 0;
        mainImage.sprite = null;
        slotText.text = "";
    }

    public void SetComponents(Sprite sprite, int value, CollectableObjects type)
    {
        mainImage.sprite = sprite;
        objectType = type;
        if (value != 0)
        {
            amount += value;
            slotText.text = "X" + amount.ToString();
        }
        else
        {
            slotText.text = "";
        }
        mainImage.SetNativeSize();
        mainImage.rectTransform.localScale = Vector3.one * 0.1f;
        if (mainImage.rectTransform.sizeDelta.x < 512 && mainImage.rectTransform.sizeDelta.y < 512)
        {
            if (mainImage.rectTransform.sizeDelta.y > mainImage.rectTransform.sizeDelta.x)
            {
                float val3 = mainImage.rectTransform.sizeDelta.y;
                float tVal = 512 - val3;
                float rVal = tVal / mainImage.rectTransform.sizeDelta.y;
                mainImage.rectTransform.sizeDelta = new Vector2(mainImage.rectTransform.sizeDelta.x + tVal * rVal, mainImage.rectTransform.sizeDelta.y + tVal);
            }
            else
            {
                float val3 = mainImage.rectTransform.sizeDelta.x;
                float tVal = 512 - val3;
                float rVal = tVal / mainImage.rectTransform.sizeDelta.x;
                mainImage.rectTransform.sizeDelta = new Vector2(mainImage.rectTransform.sizeDelta.x + tVal, mainImage.rectTransform.sizeDelta.y + tVal * rVal);
            }
        }
    }

    public void AddNewAmount(int amount)
    {
        this.amount += amount;
        slotText.text = "X" + this.amount.ToString();
    }

    public Vector3 GetImagePosition(bool worldPosition)
    {
        if (worldPosition)
            return mainImage.transform.position;
        else
            return mainImage.transform.localPosition;
    }
}
