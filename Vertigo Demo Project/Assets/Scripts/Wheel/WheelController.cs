using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;

[Serializable]
public class AssetReferenceCustom : AssetReferenceT<SlotContainer>
{
    public AssetReferenceCustom(string guid) : base(guid) { }
}

public class WheelController : MonoBehaviour
{
    [Header("LuckWheelAssets")]
    public AssetReferenceCustom[] slotsAsset;
    public AssetReference slotComponents;
    public AssetReference collectedPrizeAsset;
    public AssetReference levelCard;
    public AssetReference blueSpriteAssetReference;
    public AssetReference greenSpriteAssetReference;
    public AssetReference animationImagesReference;
    public AssetReference wheelVisualsReference;

    [SerializeField]
    private RectTransform wheelTransform;
    [SerializeField]
    private Transform levelCardsParent;
    [SerializeField]
    private List<GameObject> slots;
    [SerializeField]
    private Button spinButton;
    [SerializeField]
    private Image wheelImage;
    [SerializeField]
    private Image wheelCursor;

    public Sprite blueSprite;
    public Sprite greenSprite;
    private SlotContainer sc;
    public Image currentCardShower;
    public TMP_Text currentCardLevel;
    public RectTransform cardsMask;
    public RectTransform collectedCardsContainer;
    public Button startGameButton;
    private int switchPositionCardID = 0;
    private int lastSettedCardID;

    private List<WheelComponents> wheelComponents;
    private List<CardComponents> levelCards;
    private List<SlotComponents> collectedPrizes;
    private List<AnimationImage> collectedPrizesAnimation;
    int currentLevel = 0;
    int currentCard = 0;
    private int numObjects = 8;
    private float radius;
    float[] stopAngles = new float[] { 720, 765, 810, 855, 900, 945, 990, 1035, 1080 };

    // Take the start position and distance between the cards from a global settings
    private void WheelController_Completed(AsyncOperationHandle<IResourceLocator> obj)
    {
        blueSpriteAssetReference.LoadAssetAsync<Sprite>().Completed += go =>
        {
            blueSprite = go.Result;
        };

        greenSpriteAssetReference.LoadAssetAsync<Sprite>().Completed += go =>
        {
            greenSprite = go.Result;
        };

        for (int i = 0; i < 5; i++)
        {
            animationImagesReference.InstantiateAsync().Completed += go =>
            {
                go.Result.transform.SetParent(transform.parent, false);
                go.Result.transform.position = Vector3.down * 1000f;
                collectedPrizesAnimation.Add(go.Result.GetComponent<AnimationImage>());
            };
        }

        wheelVisualsReference.LoadAssetAsync<WheelVisuals>().Completed += go =>
        {
            for (int i = 0; i < 3; i++)
            {
                wheelComponents.Add(go.Result.components[i]);
            }
        };

        levelCard.InstantiateAsync().Completed += (go) =>
        {
            float cardWidth = go.Result.GetComponent<RectTransform>().sizeDelta.x;
            float mul = 0.5f;
            for (int i = 0; i < numObjects; i++)
            {
                GameObject obj = Instantiate(go.Result);
                obj.transform.SetParent(levelCardsParent, false);
                obj.GetComponent<RectTransform>().anchoredPosition = Vector3.right * ((cardWidth + 20f * mul) * i + cardWidth * 0.5f);
                levelCards.Add(obj.GetComponent<CardComponents>());
                mul = 1f;
            }
            Sequence seq = DOTween.Sequence();
            seq.SetDelay(0.01f).OnComplete(() =>
            {
                for (int i = 0; i < levelCards.Count; i++)
                {
                    if ((i + 1) % 5 == 0 || i == 0)
                    {
                        levelCards[i].SetLevel(i + 1, greenSprite);
                    }
                    else
                    {
                        levelCards[i].SetLevel(i + 1, blueSprite);
                    }
                }
            });
            lastSettedCardID = numObjects;
            StartCoroutine(Mover());
        };

        slotComponents.InstantiateAsync().Completed += (go) =>
        {
            radius = wheelTransform.sizeDelta.x * 0.25f;
            for (int i = 0; i < numObjects; i++)
            {
                float theta = i * 2 * Mathf.PI / numObjects;
                float x = Mathf.Sin(theta) * radius;
                float y = Mathf.Cos(theta) * radius;

                GameObject obj = Instantiate(go.Result);
                obj.transform.SetParent(wheelTransform, false);
                obj.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
                obj.transform.up = (spinButton.transform.position - obj.transform.position).normalized * -1;
                slots.Add(obj);
            }

            slotsAsset[currentLevel].LoadAssetAsync<SlotContainer>().Completed += (go) =>
            {
                sc = go.Result;
                for (int i = 0; i < slots.Count; i++)
                {
                    slots[i].GetComponent<SlotComponents>().SetComponents(sc.slots[i].mainSprite, sc.slots[i].value, sc.slots[i].weaponType);
                }

            };
        };
    }

    public void Spin()
    {
        ButtonClickAnimation();
        wheelTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        wheelTransform.DOLocalRotate(Vector3.back * stopAngles[UnityEngine.Random.Range(0, 8)], 3f, RotateMode.LocalAxisAdd).OnComplete(() => GetEarnedPrize());
    }

    public void SwitchWheelSprite(WheelLevel level)
    {
        wheelImage.transform.DOScale(1.3f, 0.3f).OnComplete(() =>
         wheelImage.transform.DOScale(1f, 0.3f));
        LoadNextCardComponents();
        wheelImage.DOColor(new Color(wheelImage.color.r, wheelImage.color.r, wheelImage.color.r, 0f), 0.3f).OnComplete(() =>
        {

        }
        ).OnComplete(() =>
        wheelImage.DOColor(new Color(wheelImage.color.r, wheelImage.color.r, wheelImage.color.r, 1f), 0.3f)
        );

        for (int i = 0; i < wheelComponents.Count; i++)
        {
            if (wheelComponents[i].level == level)
            {
                wheelImage.sprite = wheelComponents[i].wheelImage;
                wheelCursor.sprite = wheelComponents[i].wheelCursur;
            }
        }
    }

    public void ChangeCardPosition()
    {
        if (levelCards[switchPositionCardID].IsMasked(cardsMask))
        {
            levelCards[switchPositionCardID].SetLocalPosition(Vector3.right * ((levelCards[switchPositionCardID].GetSizeDelta().x + 20f) * lastSettedCardID + levelCards[switchPositionCardID].GetSizeDelta().x * 0.5f));
            if ((lastSettedCardID + 1) % 5 == 0)
            {
                levelCards[switchPositionCardID].SetLevel(lastSettedCardID + 1, greenSprite);
            }
            else
            {
                levelCards[switchPositionCardID].SetLevel(lastSettedCardID + 1, blueSprite);
            }

            if (lastSettedCardID == levelCards.Count - 1)
            {
                lastSettedCardID = 0;
            }
            else
            {
                lastSettedCardID++;
            }

            switchPositionCardID++;
        }
        else
        {

        }
    }

    private IEnumerator Mover()
    {
        while (levelCards[currentCard].transform.position.x > currentCardShower.transform.position.x)
        {
            levelCardsParent.transform.position += Vector3.left * 100f * Time.deltaTime;
            yield return null;
        }

        if (currentCard == levelCards.Count -1)
        {
            currentCard = 0;
        }
        else
        {
            currentCard++;
        }
        spinButton.transform.DOScale(1f, 0.3f);
        spinButton.interactable = true;
    }

    private void LoadNextCardComponents()
    {
        if(currentLevel==slotsAsset.Length)
        {
            UIManager.instance.EnableCollectPanel();
            return;
        }
        slotsAsset[currentLevel].LoadAssetAsync<SlotContainer>().Completed += (go) =>
        {
            sc = go.Result;
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].GetComponent<SlotComponents>().SetComponents(sc.slots[i].mainSprite, sc.slots[i].value, sc.slots[i].weaponType);
            }
        };
    }

    public void MoveNext()
    {
        if (currentLevel == slotsAsset.Length - 1)
        {
            currentLevel = 0;
        }
        else
        {
            currentLevel++;
        }

        if ((currentLevel + 1) % 5 == 0 || currentLevel + 1 == 1)
        {
            SwitchWheelSprite(WheelLevel.Silver);
            currentCardShower.sprite = greenSprite;
            currentCardLevel.text = (currentLevel + 1).ToString();
        }
        else
        {
            SwitchWheelSprite(WheelLevel.Bronz);
            currentCardShower.sprite = blueSprite;
            currentCardLevel.text = (currentLevel + 1).ToString();
        }

        StartCoroutine(Mover());
        ChangeCardPosition();
    }

    public void GetEarnedPrize()
    {
        float highest = 0;
        int index = 0;
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].transform.position.y > highest)
            {
                highest = slots[i].transform.position.y;
                index = i;
            }
        }
        if (sc.slots[index].weaponType == CollectableObjects.Death)
        {
            UIManager.instance.EnableFailedPanel();

        }
        else
        {
            UIManager.instance.EnableCollectButton();
            AddEarnedPrize(sc.slots[index].mainSprite, sc.slots[index].value, sc.slots[index].weaponType, slots[index].transform.position);
        }
    }

    public void AddEarnedPrize(Sprite sprite, int count, CollectableObjects type, Vector3 earnedPrizePosition)
    {
      
        for (int i = 0; i < collectedPrizes.Count; i++)
        {
            if (collectedPrizes[i].objectType == type)
            {
                for (int z = 0; z < collectedPrizesAnimation.Count; z++)
                {
                    collectedPrizesAnimation[z].Animate(earnedPrizePosition, collectedPrizes[i].GetImagePosition(true), sprite);
                }
                Sequence seq = DOTween.Sequence();
                seq.SetDelay(2.8f).OnComplete(() =>
                {
                    collectedPrizes[i].AddNewAmount(count);
                    MoveNext();
                }
                );

                return;
            }
        }

        collectedPrizeAsset.InstantiateAsync().Completed += (go) =>
        {
            collectedPrizes.Add(go.Result.GetComponent<SlotComponents>());
            go.Result.transform.SetParent(collectedCardsContainer, false);
            go.Result.GetComponent<SlotComponents>().SetComponents(sprite, 0, type);
            Sequence seq = DOTween.Sequence();
            seq.SetDelay(0.1f).OnComplete(() =>
            {
                for (int z = 0; z < collectedPrizesAnimation.Count; z++)
                {
                    collectedPrizesAnimation[z].Animate(earnedPrizePosition, go.Result.GetComponent<SlotComponents>().GetImagePosition(true), sprite);
                }
                seq = DOTween.Sequence();
                seq.SetDelay(2.8f).OnComplete(() =>
                {
                    collectedPrizes[collectedPrizes.Count - 1].AddNewAmount(count);
                    MoveNext();
                }
                );
            }
            );

            return;
        };
    }

    public void ButtonClickAnimation()
    {
        spinButton.interactable = false;
        spinButton.transform.DOScale(1.2f, 0.3f).OnComplete(() =>
        spinButton.transform.DOScale(0f, 0.2f));
    }

    public void SetUp()
    {
        spinButton.onClick.AddListener(Spin);
        startGameButton.onClick.AddListener(Initialize);
    }

    private void Initialize()
    {
        UIManager.instance.StartGame();
        slots = new List<GameObject>();
        levelCards = new List<CardComponents>();
        collectedPrizesAnimation = new List<AnimationImage>();
        wheelComponents = new List<WheelComponents>();
        collectedPrizes = new List<SlotComponents>();
        Addressables.InitializeAsync().Completed += WheelController_Completed;
    }

    private void OnValidate()
    {
        spinButton.onClick.AddListener(Spin);
        startGameButton.onClick.AddListener(Initialize);
    }

}
