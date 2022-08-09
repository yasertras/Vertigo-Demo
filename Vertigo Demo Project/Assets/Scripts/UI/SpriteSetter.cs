using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SpriteSetter : MonoBehaviour
{
    [SerializeField]
    private string spriteName;

    private void OnEnable()
    {
        GetComponent<Image>().sprite = AddressablesManager.instance.GetSprite(spriteName);
    }
}
