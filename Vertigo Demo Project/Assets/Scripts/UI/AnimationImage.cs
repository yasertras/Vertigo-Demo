using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AnimationImage : MonoBehaviour
{
    [SerializeField]
    private Image image;


    public void Animate(Vector3 startPosition, Vector3 endPosition, Sprite sprite = null,UnityAction action=null)
    {
        if (sprite)
            image.sprite = sprite;
        transform.DOMove(startPosition, 0.01f);
        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.5f);
        transform.DOMove(new Vector3(startPosition.x + Random.Range(-50f, 75f), startPosition.y + Random.Range(50f, 100f), startPosition.z), 1f).OnComplete(() => 
        transform.DOMove(endPosition, 1.2f).OnComplete(() => 
        transform.DOScale(0f, 0.2f).OnComplete(() =>
        action?.Invoke()
        ).OnComplete(() => 
        DOTween.Kill(transform, true))));
    }
}
