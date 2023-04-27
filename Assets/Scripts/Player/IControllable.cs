
using UnityEngine;

public interface IControllable
{
    public void Move(Vector3 direction);
    public void Jump();
    public void BaseUpdate();
    public void Idle();
}
