using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoosts : MonoBehaviour
{
    [SerializeField]
    private GameObject wings;

    public static Action<BoosterType> OnBoosterTaken;
    public static Action OnWingsClosed;

    private void OnEnable()
    {
        GameManager.OnEndOfChapter += CloseWings;
        GameManager.OnChapterWon += CloseWings;
        GameManager.OnChapterLost += CloseWings;
        OnBoosterTaken += BoosterTaken;
    }

    private void OnDisable()
    {
        GameManager.OnEndOfChapter -= CloseWings;
        GameManager.OnChapterWon -= CloseWings;
        GameManager.OnChapterLost -= CloseWings;
        OnBoosterTaken -= BoosterTaken;
    }

    public void BoosterTaken(BoosterType boosterType)
    {
        switch (boosterType)
        {
            case BoosterType.WINGS:
                wings.SetActive(true);
                break;
            case BoosterType.SIZEUP:
                break;
            default:
                break;
        }
    }

    public void CloseWings(int chapter)
    {
        wings.SetActive(false);
        OnWingsClosed?.Invoke();
    }

    public void CloseWings()
    {
        wings.SetActive(false);
        OnWingsClosed?.Invoke();
    }
}
