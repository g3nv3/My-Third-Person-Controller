using UnityEngine;
public class PlayerStateSwimMove : PlayerStateMove
{
    public PlayerStateSwimMove(PlayerController playerController) : base(playerController) { }
    public override void Enter() 
    {
        _playerController.Speed = _playerController.SwimSpeed;
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
        _playerController.Speed = _playerController.BaseSpeed;
        Debug.Log("Swim Exit");
    }
}
