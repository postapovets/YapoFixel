using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;

    public void ButtonGetColorsOnClick()
    {
        _gameManager.GetColors();
    }

    public void ButtonAnalyzeOnClick()
    {
        _gameManager.Analyze();
    }

}
