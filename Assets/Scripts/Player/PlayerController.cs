using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour, IControllable
{
    [Header("Main Components")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _playerMass = 5;
    [SerializeField] private Transform _checkWaterTransform;

    private Dictionary<string, IPlayerState> _playerStates;
    private IPlayerState _currentPlayerState;
    private Transform _transform;
    public Animator PlayerAnimator => _animator;
    public CharacterController PlayerCharacterController => _characterController;
    public Transform CameraTransform => _cameraTransform;
    public float PlayerMass => _playerMass;
    public Transform CheckWaterTransform => _checkWaterTransform;
    public Transform PlayerTransform => _transform;

    [Header("Move")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _canMove;
    [SerializeField] private bool _isSwim;
    [SerializeField] private float _speed = 15f;
    [SerializeField] private float _rotationSpeed;
    private Vector3 movementDirection;
    public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }
    public bool CanMove => _canMove;
    public bool IsSwim { get { return _isSwim; } set { _isSwim = value; } }
    public float Speed => _speed;
    public float RotationSpeed => _rotationSpeed;
    public Vector3 MoveDirection => movementDirection;

 
    [Header("Jump")]
    [SerializeField] private float _jumpForce;
    public float JumpForce => _jumpForce;   

    [Header("Gravity")]
    [SerializeField] private float _heightToMidAirAnimation = 1f;
    private float gravityForce = -9.8f;
    private float tempFallingSpeed = -1f;
    public float HeightToMidAirAnimation => _heightToMidAirAnimation;    
    public float GravityForce => gravityForce;
    public float TempFallingSpeed { get { return tempFallingSpeed; } set { tempFallingSpeed = value; } }
    

    private void Start()
    {
        _transform = GetComponent<Transform>();
        _cameraTransform = Camera.main.GetComponent<Transform>();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        InitStates();
        SwitchState(typeof(PlayerStateIdle).Name);
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
    }

    public void SwitchState(string stateName)
    {
        if(_currentPlayerState != _playerStates[stateName])
        {
            if (_currentPlayerState != null)
                _currentPlayerState.Exit();

            _currentPlayerState = _playerStates[stateName];
            _currentPlayerState.Enter();
        }       
    }

    public void Move(Vector3 direction)
    {
        movementDirection = direction;
        if (_canMove && _isGrounded)
        {
            SwitchState(typeof(PlayerStateMove).Name);
        }   
    }

    public void Jump()
    {
        if(!_isSwim && _isGrounded && _canMove)
            SwitchState(typeof(PlayerStateJump).Name);
    }

    public void Idle()
    {
        if (_canMove && _isGrounded)
        {
            movementDirection = Vector3.zero;
            SwitchState(typeof(PlayerStateIdle).Name);
        }
            
    }

    public float CheckDistanceToGround()
    {
        RaycastHit hit;
        if(Physics.Raycast(_transform.position, Vector3.down, out hit))
            return hit.distance;
        return 0f;
    }


}
