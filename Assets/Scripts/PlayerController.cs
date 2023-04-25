using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Main Components")]
    [SerializeField] private States _states;
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private float _playerMass = 5;
    [SerializeField] private Transform _cameraTransform;
    private Transform _transform;
    private enum States
    {
        Idle,
        Run,
        Jump,
        MidAir
    }

    [Header("Move")]
    [SerializeField] private bool _canMove = true;
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _rotationSpeed = 15f;
    private Vector3 movementDirection;

    [Header("Jump")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private float _jumpForce = 15f;
    [SerializeField] private float _heightToLandAnimation = 1f;
    private float gravityForce = -9.8f;
    private float tempFallingSpeed = -1f;
    private bool midAir;

    private void Start()
    {
        _transform = GetComponent<Transform>();
        _cameraTransform = Camera.main.GetComponent<Transform>();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        StateMachine();
        Move();
    }

    private void StateMachine()
    {
        _isGrounded = _characterController.isGrounded;
        if (_isGrounded && _states == States.MidAir)
        {
            _animator.SetTrigger("Land");
            midAir = false;
        }
        if (!_isGrounded && CheckDistanceToGround() >= _heightToLandAnimation)
            _states = States.MidAir;

        if ((Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f) && _canMove && _isGrounded)
            _states = States.Run;
        else if (_isGrounded) _states = States.Idle;
  
        if (_isGrounded && _canMove && Input.GetKeyDown(KeyCode.Space)) _states = States.Jump;
    }
    private void Move()
    {
        movementDirection.x = 0f;
        movementDirection.z = 0f;
        MovePlayer();
        RotatePlayer();
        CalculateFallingSpeed();
        Jump();
        movementDirection.y = tempFallingSpeed;
        _characterController.Move(movementDirection * Time.deltaTime);      
    }

    private void MovePlayer()
    {
        movementDirection.x = Input.GetAxis("Horizontal") * _speed;
        movementDirection.z = Input.GetAxis("Vertical") * _speed;
        movementDirection = Vector3.ClampMagnitude(movementDirection, _speed);
        if (_isGrounded)
        {
            _animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
            _animator.SetFloat("Vertical", Input.GetAxis("Vertical"));
        }
    }

    private void RotatePlayer()
    {
        Quaternion tempCameraRotation = _cameraTransform.rotation;
        _cameraTransform.rotation = Quaternion.Euler(0f, _cameraTransform.eulerAngles.y, 0f);
        movementDirection = _cameraTransform.TransformDirection(movementDirection);
        _cameraTransform.rotation = tempCameraRotation;

        _transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.LookRotation(new Vector3(movementDirection.x, 0, movementDirection.z)), _rotationSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        if (_states == States.MidAir)
        {
            if(!midAir)
                _animator.SetTrigger("MidAir");         
            midAir = true;
        }          
        else if (_states == States.Jump)
        {
            tempFallingSpeed = _jumpForce;
            _animator.SetTrigger("Jump");
            _states = States.MidAir;
        }       
    }

    private float CheckDistanceToGround()
    {
        RaycastHit hit;
        if(Physics.Raycast(_transform.position, Vector3.down, out hit))
            return hit.distance;
        return 0f;
    }

    private void CalculateFallingSpeed()
    {
        if (!_isGrounded)
            tempFallingSpeed += gravityForce * _playerMass * Time.deltaTime;      
        else tempFallingSpeed = -1f;
    }
}
