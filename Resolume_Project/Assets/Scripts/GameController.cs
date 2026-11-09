using UnityEngine;

public class GameController : MonoBehaviour
{
    public HandLiveTest hand;
    public ResolumeOsc resolume;

    void OnEnable()
    {
        hand.GestureChanged += OnGestureChanged;
    }

    void OnDisable()
    {
        hand.GestureChanged -= OnGestureChanged;
    }

    void OnGestureChanged(string gesture)
    {
        switch (gesture)
        {
            case "Fist":
                resolume.TriggerClip(1, 1);
                break;
            case "Open":
                resolume.TriggerClip(1, 2);
                break;
            case "Pinch":
                resolume.TriggerClip(1, 3);
                break;
        }
    }
}
