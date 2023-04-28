using UnityEngine;
public class PlayerStateSwimMove : PlayerStateMove
{
    public PlayerStateSwimMove(PlayerController playerController) : base(playerController) { }
    public override void Enter() 
    {
        _playerController.PlayerAnimator.SetBool("IsSwim", true);
        Debug.Log("Swim Enter");
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit() {
        _playerController.PlayerAnimator.SetBool("IsSwim", false);
        _playerController.IsSwim = false;
        Debug.Log("Swim Exit");
    }
}
