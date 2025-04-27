using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class GameDataUnlocker : MonoBehaviour
{

    Keyboard _keyboard = default;
#if UNITY_EDITOR
    [SerializeField, ReadOnly]
#endif
    int _currentCodeToPressIndex = default;
    bool _readTyping = true;

    Key[] _passCode = new Key[]
    {
       Key.T,
       Key.H,
       Key.E,
       Key.R,
       Key.E,
       Key.I,
       Key.S,
       Key.N,
       Key.O,
       Key.S,
       Key.P,
       Key.O,
       Key.O,
       Key.N,
    };
    Key currentKeyToPress => _passCode[_currentCodeToPressIndex];

    private void Start()
    {
        _readTyping = true;
        _keyboard = InputSystem.GetDevice<Keyboard>();
        ResetKeyCodeOrder();
    }
    // Update is called once per frame
    void Update()
    {

        if (_keyboard.enterKey.wasPressedThisFrame)
        {
            ResetKeyCodeOrder();
        }

        //God mode activated so no need to check anymore
        if (!_readTyping)
        {
            return;
        }

        if (_keyboard[currentKeyToPress].wasPressedThisFrame)
        {
            _currentCodeToPressIndex++;
            CheckIfPassCodeReached();
        }
    }

    void ResetKeyCodeOrder()
    {
        _readTyping = true;
        _currentCodeToPressIndex = 0;
    }

    void CheckIfPassCodeReached()
    {
        //God mode activated so no need to check anymore
        if (_currentCodeToPressIndex == _passCode.Length)
        {
            GameData data = SaveSystem.LoadGame();
            data.TotalStarCount = 999;
            SaveSystem.SaveGame(data);
            MenuCanvas.RaiseOnResetGameProgress();
            _readTyping = false;
            return;
        }
    }

}
