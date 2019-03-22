using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    
    [Header("Options")]
    [Range(0, 1f)] public float sensibility = 0.5f;
    [Range(0f, 2f)] public float handleLimit = 1f;
    public JoystickMode joystickMode = JoystickMode.AllAxis;

    private Vector2 inputVector = Vector2.zero;

    [Header("Components")]
    public RectTransform background;
    public RectTransform handle;

    public float Horizontal { get { return inputVector.x; } }
    public float Vertical { get { return inputVector.y; } }
    public Vector2 Direction { get { return new Vector2(Horizontal, Vertical); } }

    public delegate void JoysticInput(Vector2 inputVector);
    public event JoysticInput OnJoysticInput;

    private void Update()
    {
        if (OnJoysticInput != null)
            OnJoysticInput(inputVector);
    }

    protected Vector2 InputVector
    {
        get
        {
            return inputVector;
        }

        set
        {
            inputVector = value;
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {

        
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        
    }

    protected void ClampJoystick()
    {
        if (joystickMode == JoystickMode.Horizontal)
            inputVector = new Vector2(inputVector.x, 0f);
        if (joystickMode == JoystickMode.Vertical)
            inputVector = new Vector2(0f, inputVector.y);
    }
}

public enum JoystickMode { AllAxis, Horizontal, Vertical}
