using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;

    public Button retryButton;
    public Button playAgainButton;
    public Button collectButton;

    public GameObject failedPanel;
    public GameObject collectPanel;
    public GameObject gameCanvas;
    public GameObject mainMenuCanvas;

    public WheelController wheelController;

    public object SceneManagment { get; private set; }

    private void Awake()
    {
        instance = this;

        retryButton.onClick.AddListener(Retry);
        playAgainButton.onClick.AddListener(Retry);
        collectButton.onClick.AddListener(EnableCollectPanel);
#if !UNITY_EDITOR
        wheelController.SetUp();
#endif
    }

    public void EnableCollectPanel()
    {
        collectPanel.SetActive(true);
        collectButton.gameObject.SetActive(false);
        collectPanel.transform.DOScale(1.2f, 0.8f).OnComplete(() =>
         collectPanel.transform.DOScale(1f, 0.5f));
    }


    public void EnableFailedPanel()
    {
        collectButton.gameObject.SetActive(false);
        failedPanel.SetActive(true);
        failedPanel.transform.DOScale(1.2f, 0.8f).OnComplete(() =>
         failedPanel.transform.DOScale(1f, 0.5f));
    }
    
    public void StartGame()
    {
        mainMenuCanvas.SetActive(false);
        gameCanvas.SetActive(true);
    }

    private void Retry()
    {
        SceneManager.LoadSceneAsync(0,LoadSceneMode.Single);
    }

    public void EnableCollectButton()
    {
        if (collectButton.gameObject.activeInHierarchy)
            return;
        collectButton.gameObject.SetActive(true);
        collectButton.transform.DOScale(1.2f, 0.8f).OnComplete(() =>
         collectButton.transform.DOScale(1f, 0.5f));
    }

    private void OnValidate()
    {
        retryButton.onClick.AddListener(Retry);
        playAgainButton.onClick.AddListener(Retry);
        collectButton.onClick.AddListener(EnableCollectPanel);

    }
}
