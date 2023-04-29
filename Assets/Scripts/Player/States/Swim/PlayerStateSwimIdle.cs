using UnityEngine;

public class PlayerStateSwimIdle : PlayerStateMove
{
    public PlayerStateSwimIdle(PlayerController playerController) : base(playerController) { }
    public override void Enter() {
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit() { }
}
