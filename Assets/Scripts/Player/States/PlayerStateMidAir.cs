﻿

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
            _playerController.SwitchState("Move");

    }
    public override void Exit()
    {
        _playerController.PlayerAnimator.SetTrigger("Land");
    }
}