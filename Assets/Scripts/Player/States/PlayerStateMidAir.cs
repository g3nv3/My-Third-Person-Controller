

public class PlayerStateMidAir : PlayerStateMove
{
    public PlayerStateMidAir(PlayerController playerController) : base(playerController) { }
    public override void Enter()
    {
        _playerController.PlayerAnimator.SetTrigger("MidAir");
    }

    public override void Update()
    {
        base.Update();
        if (_playerController.IsGrounded)
            _playerController.SwitchState(typeof(PlayerStateMove).Name);

    }
    public override void Exit()
    {
        if(!_playerController.IsSwim)
            _playerController.PlayerAnimator.SetTrigger("Land");
    }
}