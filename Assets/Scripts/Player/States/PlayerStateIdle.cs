
public class PlayerStateIdle : PlayerStateMove
{
    public PlayerStateIdle(PlayerController playerController) : base (playerController) { }
    public override void Enter() { }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit() { }
}
