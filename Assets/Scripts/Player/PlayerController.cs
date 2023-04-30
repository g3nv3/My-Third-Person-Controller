using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour, IControllable
{
    [Header("Main Components")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _playerMass = 5;
    [SerializeField] private Transform _checkWaterTransform;
    [SerializeField] private float _waterHeight;
    public States PlayerStates;
    public static UnityEvent<float> SwitchPlayerSpeed = new UnityEvent<float>();

    private Dictionary<string, IPlayerState> _playerStates;
    private IPlayerState _currentPlayerState;
    private Transform _transform;
    public Animator PlayerAnimator => _animator;
    public CharacterController PlayerCharacterController => _characterController;
    public Transform CameraTransform => _cameraTransform;
    public float PlayerMass => _playerMass;
    public Transform CheckWaterTransform => _checkWaterTransform;
    public Transform PlayerTransform { get { return _transform; } set { _transform = value; } }
    public float WaterHeight => _waterHeight;

    public enum States
    {
        Move,
        Swim,
        Dive,
        MidAir,
        Death
    }

    [Header("Move")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _canMove;    
    [SerializeField] private float _speed = 15f;
    [SerializeField] private float _rotationSpeed = 15f;
    [SerializeField] private Vector3 movementDirection;
    public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }
    public bool CanMove => _canMove;  
    public float Speed => _speed;
    public float RotationSpeed => _rotationSpeed;
    public Vector3 MoveDirection => movementDirection;

    public float BaseSpeed { get; set; }

    [Header("Water")]
    [SerializeField] private bool _isSwim;
    [SerializeField] private float _swimSpeed = 10f;
    [SerializeField] private float _popupSpeed;
    [SerializeField] private float _diveSpeed = -1f;
    [SerializeField] private float _angleOnDive;
    public float Rot;
    public bool IsSwim { get { return _isSwim; } set { _isSwim = value; } }
    public float SwimSpeed => _swimSpeed;
    public float PopupSpeed => _popupSpeed;
    public float DiveSpeed => _diveSpeed;
    public float AngleOnDive => _angleOnDive;

    [Header("Sprint")]
    [SerializeField] private bool _canSprint = true;
    [SerializeField] private bool _isSprint = false;
    [SerializeField] private float _addPerscentSpeed = 15f;
    [SerializeField] private float _stamina = 100f;
    [SerializeField] private float _coefStamina = 1.5f;
    [SerializeField] private float _minStaminaToStart = 15f;
    private float tempSpeed;
    private float startStamina;
    private bool shiftUp = true;
    public float Stamina { get { return _stamina; } set { _stamina = value; } }
    public bool IsSprint => _isSprint;
    public float StartStamina => startStamina;
    public float CoefStamine => _coefStamina;
    public bool CanSprint { get { return _canSprint; } set { _canSprint = value; } }

    [Header("Jump")]
    [SerializeField] private float _jumpForce;
    public float JumpForce => _jumpForce;   

    [Header("Gravity")]
    [SerializeField] private float _heightToMidAirAnimation = 1f;
    private float gravityForce = -9.8f;
    [SerializeField] private float tempFallingSpeed = -9.8f;
    public float HeightToMidAirAnimation => _heightToMidAirAnimation;    
    public float GravityForce => gravityForce;
    public float TempFallingSpeed { get { return tempFallingSpeed; } set { tempFallingSpeed = value; } }


    private void Start()
    {
        BaseSpeed = _speed;
        startStamina = _stamina;
        _transform = GetComponent<Transform>();
        _cameraTransform = Camera.main.GetComponent<Transform>();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        InitStates();
        SwitchState(typeof(PlayerStateIdle).Name);
        SwitchPlayerSpeed.AddListener(SwitchSpeed);
    }
    public void BaseUpdate()
    {
        _currentPlayerState.Update();        
        _isGrounded = _characterController.isGrounded;
    }
    private void InitStates()
    {
        _playerStates = new Dictionary<string, IPlayerState>();

        _playerStates[typeof(PlayerStateMove).Name] = new PlayerStateMove(this);
        _playerStates[typeof(PlayerStateIdle).Name] = new PlayerStateIdle(this);
        _playerStates[typeof(PlayerStateJump).Name] = new PlayerStateJump(this);
        _playerStates[typeof(PlayerStateMidAir).Name] = new PlayerStateMidAir(this);
        _playerStates[typeof(PlayerStateSwimMove).Name] = new PlayerStateSwimMove(this);
        _playerStates[typeof(PlayerStateSwimIdle).Name] = new PlayerStateSwimIdle(this);
        _playerStates[typeof(PlayerStateDive).Name] = new PlayerStateDive(this);
        _playerStates[typeof(PlayerStateDead).Name] = new PlayerStateDead(this);
    }

    private void SwitchSpeed(float speed)
    {
        _speed = speed;
        tempSpeed = _speed;
    }

    public void SwitchState(string stateName)
    {
        if (_currentPlayerState != _playerStates[stateName])
        {
            StopSprint();
            if (_currentPlayerState != null)
                _currentPlayerState.Exit();

            _currentPlayerState = _playerStates[stateName];
            _currentPlayerState.Enter();
        }
    }

    public void StartSprint()
    {   
        if(_stamina >= _minStaminaToStart && shiftUp && _canSprint)
        {
            if (!_isSprint)
            {
                tempSpeed = _speed;
                _speed += _speed * _addPerscentSpeed / 100f;
            }
            shiftUp = false;
            _isSprint = true;
            _animator.SetBool("IsSprint", true);
        }        
    }

    public void Death()
    {
        _canMove = false;
        _canSprint = false;
        _isSprint = false;
        _isSwim = false;
    }
    public void StopSprint()
    {
        if (_isSprint)
            SwitchPlayerSpeed.Invoke(tempSpeed);
        _isSprint = false;
        _animator.SetBool("IsSprint", false);
    }

    public void ShiftUp()
    {
        shiftUp = true;
    }

    public void Dive()
    {
        if (IsSwim)
            SwitchState(typeof(PlayerStateDive).Name);
    }

    public void Popup()
    {
        if (IsSwim)
            SwitchState(typeof(PlayerStateSwimMove).Name);
            
    }

    public void Move(Vector3 direction)
    {
        movementDirection = direction;
        if (CheckCanMoveOnGround())
        {
            SwitchState(typeof(PlayerStateMove).Name);
        }   
    }

    public void Jump()
    {
        if(CheckCanMoveOnGround())
            SwitchState(typeof(PlayerStateJump).Name);
    }

    public void Idle()
    { 
        if (CheckCanMoveOnGround()) 
        {
            movementDirection = Vector3.zero;
            SwitchState(typeof(PlayerStateIdle).Name);
            if (_isSprint) StopSprint();
        }
            
    }

    public bool CheckCanMoveOnGround()
    {
        return _canMove && _isGrounded && CheckWaterHeight(_waterHeight);
    }

    public bool CheckWaterHeight(float waterHeight)
    {
        if (_transform.position.y >= waterHeight)
            return true;
        return false;
    }

    public float CheckDistanceToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(_transform.position + Vector3.up, Vector3.down, out hit))
            return hit.distance;
        return 0f;
    }
}
