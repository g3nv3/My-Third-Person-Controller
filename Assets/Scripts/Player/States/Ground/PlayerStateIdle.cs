
public class PlayerStateIdle : PlayerStateMove
{
    public PlayerStateIdle(PlayerController playerController) : base (playerController) { }
    public override void Enter() {
        _playerController.CurrentPlayerState = PlayerController.PlayerStates.Idle;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit() { }
}
