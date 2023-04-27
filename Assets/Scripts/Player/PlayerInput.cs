using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour
{
    public UnityEvent OnSpaceEnter = new UnityEvent();
    public UnityEvent<Vector3> OnMoveInput = new UnityEvent<Vector3>();
    public UnityEvent NoneInput = new UnityEvent();

    private IControllable _controllable;
    private bool cursorVisible = false;


    private void Start()
    {
        _controllable = GetComponent<IControllable>();
        Cursor.visible = cursorVisible;
    }

    private void Update()
    {
        BasePlayerUpdate();
        ReadJump();
        ReadMove();        
        SetCursorVisible();
    }

    private void ReadMove()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical);

        if (direction != Vector3.zero)
            OnMoveInput.Invoke(direction);
        else
            NoneInput.Invoke();
    }

    private void ReadJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            OnSpaceEnter.Invoke();
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
