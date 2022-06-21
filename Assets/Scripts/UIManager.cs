using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI currentLevelText;

    [SerializeField]
    private TextMeshProUGUI nextLevelText;

    [SerializeField]
    private List<Image> chapterIndicators;

    [SerializeField]
    private Color doneChapterColor;

    [SerializeField]
    private GameObject gameOverPanel, startPanel, nextLevelPanel;

    private void OnEnable()
    {
        GameManager.OnGamePlayStarted += CloseStartPanel;
        GameManager.OnChapterWon += ChapterPassed;
        GameManager.OnEndLevel += LevelPassed;
        GameManager.OnEndLevel += OpenNextLevelPanel;
        GameManager.OnGameOver += LevelLost;
    }

    private void OnDisable()
    {
        GameManager.OnGamePlayStarted -= CloseStartPanel;
        GameManager.OnChapterWon -= ChapterPassed;
        GameManager.OnEndLevel -= LevelPassed;
        GameManager.OnEndLevel -= OpenNextLevelPanel;
        GameManager.OnGameOver -= LevelLost;
    }

    void Start()
    {
        startPanel.SetActive(true);
        currentLevelText.text = GameManager.Instance.currentLevel.ToString();
        nextLevelText.text = (GameManager.Instance.currentLevel + 1).ToString();
    }

    public void ChapterPassed(int chapter)
    {
        chapterIndicators[chapter].color = doneChapterColor;
    }

    public void LevelPassed()
    {
        currentLevelText.text = GameManager.Instance.currentLevel.ToString();
        nextLevelText.text = (GameManager.Instance.currentLevel+1).ToString();

        foreach (var indicator in chapterIndicators)
        {
            indicator.color = Color.white;
        }
    }

    public void LevelLost()
    {
        gameOverPanel.SetActive(true);
    }

    public void TryAgain()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void CloseStartPanel()
    {
        startPanel.SetActive(false);
    }

    public void OpenNextLevelPanel()
    {
        nextLevelPanel.SetActive(true);
    }

    public void PlayNextLevel()
    {
        GameManager.OnNextLevel?.Invoke();
        nextLevelPanel.SetActive(false);
    }
}
