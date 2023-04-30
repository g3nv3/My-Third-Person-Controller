
using UnityEngine;

public class PlayerStateDead : IPlayerState
{
    private PlayerController playerController;
    public PlayerStateDead(PlayerController controller)
    {
        playerController = controller;
    }
    public void Enter() 
    {
        if (playerController.IsSwim)
        {
            playerController.PlayerAnimator.SetTrigger("SwimDeath");
            playerController.Death();
        }        
    }
    public void Update() 
    {
        playerController.TempFallingSpeed = playerController.DiveSpeed / 4;
        playerController.PlayerCharacterController.Move(Vector3.up * playerController.TempFallingSpeed * Time.deltaTime);
    }

    public void Exit() { }
  
}
