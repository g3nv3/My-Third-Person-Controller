
using UnityEngine;

public class PlayerStateDead : IPlayerState
{
    private PlayerController _playerController;
    public PlayerStateDead(PlayerController controller)
    {
        _playerController = controller;
    }
    public void Enter() 
    {
        if (_playerController.IsSwim)
        {
            _playerController.PlayerAnimator.SetTrigger("SwimDeath");
            _playerController.Death();
        }        
    }
    public void Update() 
    {
        _playerController.TempFallingSpeed = _playerController.DiveSpeed / 4;
        _playerController.PlayerCharacterController.Move(Vector3.up * _playerController.TempFallingSpeed * Time.deltaTime);
    }

    public void Exit() { }
  
}
