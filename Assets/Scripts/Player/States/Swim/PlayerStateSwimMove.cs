using Unity.VisualScripting;
using UnityEngine;
public class PlayerStateSwimMove : PlayerStateMove
{
    public PlayerStateSwimMove(PlayerController playerController) : base(playerController) { }
    public override void Enter() 
    {
        _playerController.CurrentPlayerState = PlayerController.PlayerStates.Swim;
        PlayerController.SwitchPlayerSpeed.Invoke(_playerController.SwimSpeed);
        _playerController.PlayerAnimator.SetBool("IsSwim", true);
        _playerController.IsSwim = true;
        CalculateFallingSpeed();
    }

    public override void Update()
    {
        base.Update();
    }

    protected override void CalculateFallingSpeed()
    {
        if (_playerController.CheckPlayerOnWater(_playerController.WaterHeight))
        {
            _playerController.PlayerAnimator.SetBool("IsPopup", false);
            _playerController.TempFallingSpeed = 0f;
        }   
        else
        {
            _playerController.PlayerAnimator.SetBool("IsPopup", true);
            _playerController.TempFallingSpeed = _playerController.PopupSpeed;
        }    
    }

    protected override void CheckStamina()
    {        
        if (_playerController.Stamina <= 0f)
        {
            _playerController.CurrentPlayerState = PlayerController.PlayerStates.Death;
            RotatePlayerInWater(0f, _playerController.RotationSpeedInWater);
            _playerController.SwitchState(typeof(PlayerStateDead).Name);
        }
            
        base.CheckStamina();
    }

    protected override void RotatePlayer(float angle, float rotaionSpeed)
    {
        if (_playerController.CurrentPlayerState == PlayerController.PlayerStates.Swim)
            RotatePlayerInWater(-_playerController.AngleOnDive, _playerController.RotationSpeedInWater);
        else if (_playerController.CurrentPlayerState == PlayerController.PlayerStates.Dive)
            RotatePlayerInWater(_playerController.AngleOnDive, _playerController.RotationSpeedInWater);
    }

    private void RotatePlayerInWater(float angle, float rotationSpeed)
    {
        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && !_playerController.CheckPlayerOnWater(_playerController.WaterHeight))
            base.RotatePlayer(angle, rotationSpeed);
        else base.RotatePlayer(0, rotationSpeed);
    }

    public override void Exit()
    {
        if (_playerController.CurrentPlayerState != PlayerController.PlayerStates.Death)
        {
            _playerController.PlayerAnimator.SetBool("IsSwim", false);
            _playerController.IsSwim = false;
        }
        _playerController.PlayerAnimator.SetBool("IsPopup", false);
        
    }
}
