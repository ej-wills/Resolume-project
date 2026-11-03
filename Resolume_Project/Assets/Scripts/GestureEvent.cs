using UnityEngine;

public class GestureEvent : MonoBehaviour
{

    public HandLiveTest hand;

    private void OnEnable() => hand.GestureChanged += OnGesture;
    void OnDisable() => hand.GestureChanged -= OnGesture; 
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnGesture(string g)
    {
        switch(g)
        {
            case "Open": transform.rotation = Quaternion.Euler(300, 0, 100); break;

            case "Pinch": transform.rotation = Quaternion.Euler(100, 0, 100); break;
        }
    }
}
