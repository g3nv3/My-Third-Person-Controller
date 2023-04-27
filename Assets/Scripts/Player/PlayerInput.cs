using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private IControllable _controllable;
    private bool cursorVisible = false;

    private void Start()
    {
        _controllable = GetComponent<IControllable>();   
    }

    private void Update()
    {
        BasePlayerUpdate();
        ReadJump();
        ReadMove();        
    }

    private void ReadMove()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical);

        if (direction != Vector3.zero) 
            _controllable.Move(direction);
        else
            _controllable.Idle();
    }

    private void ReadJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            _controllable.Jump();
    }

    private void BasePlayerUpdate()
    {
        _controllable.BaseUpdate();
    }

    private void SetCursorVisible()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            cursorVisible = !cursorVisible;
        Cursor.visible = cursorVisible;
    }
}
