using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateMove : IPlayerState
{
    public PlayerController _playerController;
    private Vector3 movementDirection = Vector3.zero;

    public virtual void Enter() { }

    public PlayerStateMove(PlayerController playerController)
    {
        _playerController = playerController;
    }

    public virtual void Update()
    {
        movementDirection.x = 0f;
        movementDirection.z = 0f;
        MovePlayer(_playerController.MoveDirection);
        if(_playerController.MoveDirection != Vector3.zero) RotatePlayer();
        CheckWater();

        movementDirection.y = _playerController.TempFallingSpeed;
        
        CalculateFallingSpeed();
        _playerController.PlayerCharacterController.Move(movementDirection * Time.deltaTime);
            
    }
    public void CheckWater()
    {
        Collider[] colliders = Physics.OverlapSphere(_playerController.CheckWaterTransform.position, 0.2f);
        foreach (Collider coll in colliders)
        {
            if (coll.gameObject.CompareTag("Water") && !_playerController.IsSwim)
            {
                _playerController.IsSwim = true;
                _playerController.SwitchState(typeof(PlayerStateSwimMove).Name);               
                break;
            }
        }
        
    }
    private void MovePlayer(Vector3 direction)
    {
        movementDirection.x = direction.x * _playerController.Speed;
        movementDirection.z = direction.z * _playerController.Speed;
        
        movementDirection = Vector3.ClampMagnitude(movementDirection, _playerController.Speed);

        AnimationMove(direction);
    }

    private void AnimationMove(Vector3 direction)
    {
        if (_playerController.IsGrounded || _playerController.IsSwim)
        {
            _playerController.PlayerAnimator.SetFloat("Horizontal", direction.x);
            _playerController.PlayerAnimator.SetFloat("Vertical", direction.z);
        }
    }

    private void RotatePlayer()
    {
        Quaternion tempCameraRotation =  _playerController.CameraTransform.rotation;
         _playerController.CameraTransform.rotation = Quaternion.Euler(0f,  _playerController.CameraTransform.eulerAngles.y, 0f);
         movementDirection = _playerController.CameraTransform.TransformDirection(movementDirection);
         _playerController.CameraTransform.rotation = tempCameraRotation;
         _playerController.PlayerTransform.rotation = Quaternion.Lerp( _playerController.PlayerTransform.rotation, Quaternion.LookRotation(new Vector3(movementDirection.x, 0, movementDirection.z)), _playerController.RotationSpeed * Time.deltaTime);
    }

    public void CalculateFallingSpeed()
    {
        if (!_playerController.IsGrounded && !_playerController.IsSwim)
        {
            _playerController.TempFallingSpeed += _playerController.GravityForce * _playerController.PlayerMass * Time.deltaTime;
            if (_playerController.CheckDistanceToGround() >= _playerController.HeightToMidAirAnimation)
            {
                _playerController.SwitchState(typeof(PlayerStateMidAir).Name);
            }
        }
        else if (_playerController.IsGrounded && !_playerController.IsSwim) _playerController.TempFallingSpeed = _playerController.GravityForce;
        else if (_playerController.IsSwim && _playerController.PlayerTransform.position.y >= _playerController.WaterHeight) _playerController.TempFallingSpeed = 0f;
        else if (_playerController.IsSwim && _playerController.PlayerTransform.position.y < _playerController.WaterHeight) _playerController.TempFallingSpeed = _playerController.PopupSpeed;
    }
    public virtual void Exit() { }

}