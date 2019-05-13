using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonJoystic: Joystick
{
    [Header("Button Joystick Options")]
    public bool isFixed = false;
    public Vector2 fixedScreenPosition;
    [Range(0,100)]
    public float buttonUnbral=20;

    private static ButtonJoystic instance;

    Vector2 joystickCenter = Vector2.zero;
    bool release;

    public static ButtonJoystic Instance
    {
        get
        {
            return instance;
        }

        set
        {
            instance = value;
        }
    }

    public delegate void JoysticButtonPressed();
    public event JoysticButtonPressed OnJoysticButtonRelease;

    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        if (isFixed)
            OnFixed();
        else
            OnFloat();
    }
    public void ChangeFixed(bool joystickFixed)
    {
        if (joystickFixed)
            OnFixed();
        else
            OnFloat();
        isFixed = joystickFixed;
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if(release)
        {
            release = false;
            if (OnJoysticButtonRelease != null)
                OnJoysticButtonRelease();

        }
    }
    public override void OnDrag(PointerEventData eventData)
    {
        
        Vector2 direction = eventData.position - joystickCenter;
        if(direction.magnitude > buttonUnbral)
            InputVector = (direction.magnitude > background.sizeDelta.x / 2f) ? direction.normalized : direction / (background.sizeDelta.x / 2f);
        ClampJoystick();
        handle.anchoredPosition = (InputVector * background.sizeDelta.x / 2f) * handleLimit;
        //base.OnDrag(eventData);

    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!isFixed)
        {
            background.gameObject.SetActive(true);
            background.position = eventData.position;
            handle.anchoredPosition = Vector2.zero;
            joystickCenter = eventData.position;
        }
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!isFixed)
        {
            background.gameObject.SetActive(false);
        }
        release = true;
        InputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }
    void OnFixed()
    {
        joystickCenter = fixedScreenPosition;
        background.gameObject.SetActive(true);
        handle.anchoredPosition = Vector2.zero;
        background.anchoredPosition = fixedScreenPosition;
    }
    void OnFloat()
    {
        handle.anchoredPosition = Vector2.zero;
        background.gameObject.SetActive(false);
    }


}
