using UnityEngine;

public class PlayerStateSwimIdle : PlayerStateMove
{
    public PlayerStateSwimIdle(PlayerController playerController) : base(playerController) { }
    public override void Enter() { Debug.Log("SwimIdle enter"); }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit() { }
}
