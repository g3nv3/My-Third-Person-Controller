
public class PlayerStateJump : PlayerStateMove
{
    public PlayerStateJump(PlayerController playerController) : base(playerController) { }
    public override void Enter()
    {
        if(_playerController.IsGrounded && _playerController.CanMove)
        {
            _playerController.TempFallingSpeed = _playerController.JumpForce;
            _playerController.PlayerAnimator.SetTrigger("Jump");
            _playerController.IsGrounded = false;
        }
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit() { }
}