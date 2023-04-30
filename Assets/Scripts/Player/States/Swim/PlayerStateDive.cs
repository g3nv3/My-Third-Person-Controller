using UnityEngine;
public class PlayerStateDive : PlayerStateSwimMove
{
    public PlayerStateDive (PlayerController playerController) : base(playerController) { }

    public override void Enter()
    {
        base.Enter();
        _playerController.PlayerStates = PlayerController.States.Dive;
        _playerController.TempFallingSpeed = _playerController.DiveSpeed;
        _playerController.PlayerAnimator.SetBool("IsDive", true);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void CalculateFallingSpeed()
    {
        _playerController.TempFallingSpeed = _playerController.DiveSpeed;
    }

    public override void Exit() {
        _playerController.PlayerAnimator.SetBool("IsDive", false);
    }
}
