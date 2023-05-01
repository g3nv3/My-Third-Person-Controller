using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateMove : IPlayerState
{
    protected PlayerController _playerController;
    protected Vector3 movementDirection = Vector3.zero;

    public virtual void Enter() {
        PlayerController.SwitchPlayerSpeed.Invoke(_playerController.BaseSpeed);
        _playerController.CurrentPlayerState = PlayerController.PlayerStates.Move;
    }

    public PlayerStateMove(PlayerController playerController)
    {
        _playerController = playerController;
    }

    protected void CheckWater()
    { 
        Collider[] colliders = Physics.OverlapSphere(_playerController.CheckWaterTransform.position, 0.2f);
        foreach (Collider coll in colliders)
            if (coll.gameObject.CompareTag("Water") && !_playerController.IsSwim && _playerController.CurrentPlayerState != PlayerController.PlayerStates.Death)
            {
                _playerController.IsSwim = true;
                _playerController.SwitchState(typeof(PlayerStateSwimMove).Name);
                break;
            }
    }
    public virtual void Update()
    {
        movementDirection.x = 0f;
        movementDirection.z = 0f;

        CheckStamina();
        CheckWater();

        MovePlayer(_playerController.MoveDirection);
        if(_playerController.MoveDirection != Vector3.zero) RotatePlayer(0f, _playerController.RotationSpeed);

        movementDirection.y = _playerController.TempFallingSpeed;
        CalculateFallingSpeed();

        _playerController.PlayerCharacterController.Move(movementDirection * Time.deltaTime);
    }

    protected virtual void CheckStamina()
    {        
        if (_playerController.Stamina > 0f && _playerController.IsSprint)
            _playerController.Stamina -= Time.deltaTime * _playerController.MultiplierStaminaConsumption;
        else if (_playerController.Stamina > 0f && _playerController.IsSwim)
            _playerController.Stamina -= Time.deltaTime * _playerController.MultiplierStaminaConsumption / 4f;
        else
        {
            if(_playerController.IsSprint) _playerController.StopSprint();   
                _playerController.Stamina += Time.deltaTime * _playerController.MultiplierStaminaConsumption;
        }
        _playerController.Stamina = Mathf.Clamp(_playerController.Stamina, 0f, _playerController.StartStamina);
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

    protected virtual void RotatePlayer(float angle, float rotationSpeed)
    {
        GetLocalMovement();
        Quaternion tempQuaternion = Quaternion.LookRotation(new Vector3(movementDirection.x, 0f, movementDirection.z));
        _playerController.PlayerTransform.rotation = Quaternion.Lerp(_playerController.PlayerTransform.rotation, Quaternion.Euler(angle, tempQuaternion.eulerAngles.y, 0f), rotationSpeed * Time.deltaTime);
       
    }

    protected void GetLocalMovement()
    {
        Quaternion tempCameraRotation = _playerController.CameraTransform.rotation;
        _playerController.CameraTransform.rotation = Quaternion.Euler(0f, _playerController.CameraTransform.eulerAngles.y, 0f);
        movementDirection = _playerController.CameraTransform.TransformDirection(movementDirection);
        _playerController.CameraTransform.rotation = tempCameraRotation;
    }

    protected virtual void CalculateFallingSpeed()
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
    }
    public virtual void Exit() { }

}