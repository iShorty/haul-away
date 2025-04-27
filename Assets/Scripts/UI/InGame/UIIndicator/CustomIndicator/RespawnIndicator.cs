using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

///<Summary>Respawn indicator's update will be handled by PlayerManager. To determine whether it should be updated, check its activeself. If inactive, dont update</Summary>
public class RespawnIndicator : ImageIndicator
{
#if UNITY_EDITOR
    [ReadOnly, SerializeField]
#endif
    float _timer = default;

    TextMeshProUGUI _timerText = default;

    ///<Summary>Sets the color of the indicator.Respawn indicator's GameAwake() is not managed by unity's Awake(). Please call it manaully</Summary>
    public void GameAwake(int playerIndex)
    {
        base.GameAwake();
        _timerText = GetComponentInChildren<TextMeshProUGUI>();
        _timerText.color = Constants.For_Player.GetColor(playerIndex);
    }

    ///<Summary>Respawn indicator's update will be handled by PlayerManager</Summary>
    protected override void ExitUpdateLoop() { }

    public void StartCountDown(float time)
    {
        _timer = time;
        _timerText.text = _timer.ToString("F0");
    }

    ///<Summary>Returns true when the respawn delay timer is up</Summary>
    public override bool GameUpdate()
    {
        if (_timer <= 0)
        {
            return true;
        }

        _timer -= Time.deltaTime;
        _timerText.text = _timer.ToString("F0");
        return base.GameUpdate();
    }






}
