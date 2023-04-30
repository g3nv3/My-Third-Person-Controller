using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour
{
    public UnityEvent OnSpaceEnter = new UnityEvent();
    public UnityEvent<Vector3> OnMoveInput = new UnityEvent<Vector3>();
    public UnityEvent NoneInput = new UnityEvent();
    public UnityEvent OnCntrl = new UnityEvent();
    public UnityEvent OnCntrlUp = new UnityEvent();
    public UnityEvent OnShift = new UnityEvent();
    public UnityEvent OnShiftUp = new UnityEvent();

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
        ReadCntrl();
        ReadCntrlUp();
        ReadShift();
        ReadShiftUp();
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

    private void ReadCntrl()
    {
        if(Input.GetKey(KeyCode.LeftControl))
            OnCntrl.Invoke();
    }

    private void ReadCntrlUp()
    {
        if (Input.GetKeyUp(KeyCode.LeftControl))
            OnCntrlUp.Invoke();           
    }

    private void ReadShift()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            OnShift.Invoke();
    }

    private void ReadShiftUp()
    {
        if (Input.GetKeyUp(KeyCode.LeftShift))
            OnShiftUp.Invoke();
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
