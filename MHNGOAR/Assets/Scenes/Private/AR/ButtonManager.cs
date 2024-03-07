using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Vuforia;
public class ButtonManager : MonoBehaviour
{
    [SerializeField] private VirtualButtonBehaviour virtualButton;
    public UnityEvent OnButtonPressed;
    public UnityEvent OnButtonReleased;
    // Start is called before the first frame update
    private void OnEnable()
    {
        virtualButton.RegisterOnButtonPressed(ButtonPressed);
        virtualButton.RegisterOnButtonPressed(ButtonReleased);
    }

    private void OnDestroy()
    {
        virtualButton.UnregisterOnButtonPressed(ButtonPressed);
        virtualButton.UnregisterOnButtonPressed(ButtonReleased);
    }

    private void ButtonPressed(VirtualButtonBehaviour button)
    {
        OnButtonPressed?.Invoke();
        Debug.Log("Button pressed");
    }

    private void ButtonReleased(VirtualButtonBehaviour button)
    {
        OnButtonReleased?.Invoke();
        Debug.Log("Button released");
    }
}
