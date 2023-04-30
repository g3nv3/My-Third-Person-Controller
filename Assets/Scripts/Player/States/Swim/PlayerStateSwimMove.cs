using Unity.VisualScripting;
using UnityEngine;
public class PlayerStateSwimMove : PlayerStateMove
{
    public PlayerStateSwimMove(PlayerController playerController) : base(playerController) { }
    public override void Enter() 
    {
        _playerController.PlayerStates = PlayerController.States.Swim;
        PlayerController.SwitchPlayerSpeed.Invoke(_playerController.SwimSpeed);
        _playerController.PlayerAnimator.SetBool("IsSwim", true);
        _playerController.IsSwim = true;
        CalculateFallingSpeed();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void CalculateFallingSpeed()
    {
        if (_playerController.CheckWaterHeight(_playerController.WaterHeight))
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

    public override void CheckStamina()
    {        
        if (_playerController.Stamina <= 0f)
        {
            _playerController.PlayerStates = PlayerController.States.Death;
            _playerController.SwitchState(typeof(PlayerStateDead).Name);
        }
            
        base.CheckStamina();
    }

    public override void RotatePlayer(float angle)
    {
        Quaternion tempCameraRotation = _playerController.CameraTransform.rotation;
        _playerController.CameraTransform.rotation = Quaternion.Euler(0f, _playerController.CameraTransform.eulerAngles.y, 0f);
        movementDirection = _playerController.CameraTransform.TransformDirection(movementDirection);
        _playerController.CameraTransform.rotation = tempCameraRotation;

        if (_playerController.PlayerStates == PlayerController.States.Swim)
            RotatePlayerInWater(-_playerController.Rot);
        else if (_playerController.PlayerStates == PlayerController.States.Dive)
            RotatePlayerInWater(_playerController.Rot);
    }

    private void RotatePlayerInWater(float angle)
    {
        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && !_playerController.CheckWaterHeight(_playerController.WaterHeight))
            RotatePlayerInWater(angle, _playerController.Rot);
        else RotatePlayerInWater(0, _playerController.Rot);
    }

    private void RotatePlayerInWater(float angle, float rot)
    {
        _playerController.PlayerTransform.rotation = Quaternion.Slerp(_playerController.PlayerTransform.rotation, Quaternion.LookRotation(new Vector3(movementDirection.x, -angle, movementDirection.z)), rot * Time.deltaTime);
    }

    public override void Exit()
    {
        if (_playerController.PlayerStates != PlayerController.States.Death)
        {
            _playerController.PlayerAnimator.SetBool("IsSwim", false);
            _playerController.IsSwim = false;
        }
        _playerController.PlayerAnimator.SetBool("IsPopup", false);
        
    }
}
