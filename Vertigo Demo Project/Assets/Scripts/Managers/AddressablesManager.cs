using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

public class AddressablesManager : MonoBehaviour
{
    public static AddressablesManager instance;

    public AssetReference cardGameSpritesReference;
    private SpriteAtlas cardGameSprites;


    private void Awake()
    {
        instance = this;

    }

    private void Start()
    {
       // Addressables.InitializeAsync().Completed += AddressablesManager_Completed;
    }

    private void AddressablesManager_Completed(AsyncOperationHandle<IResourceLocator> obj)
    {
        cardGameSpritesReference.LoadAssetAsync<SpriteAtlas>().Completed += LoadSpriteAtlas;
    }

    private void LoadSpriteAtlas(AsyncOperationHandle<SpriteAtlas> obj)
    {
        cardGameSprites = obj.Result;
    }

    public Sprite GetSprite(string spriteName)
    {
        return cardGameSprites.GetSprite(spriteName);
    }
}
