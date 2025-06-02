using UnityEngine;

public class ButtonMouseCheck : MonoBehaviour
{
    public bool mouseIsOn = false;
    void OnEnable()
    {
        mouseIsOn = false; //if it is true when the mouse is off, it will be stuck true until the mouse enters and leaves again (happens when its disabled)
    }
    public void mouseEnters() { mouseIsOn = true; } //called by an event trigger on the gameobject (must be set up in editor)
    public void mouseLeaves() { mouseIsOn = false;  } //called by an event trigger on the gameobject
}
