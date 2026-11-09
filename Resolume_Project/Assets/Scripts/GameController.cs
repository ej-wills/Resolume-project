using UnityEngine;

public class GameController : MonoBehaviour
{
    public enum State { Idle, Playing, Solved,Failed}


    public HandLiveTest hand;
    public ResolumeOsc resolume;

    [Header("Puzzle")]
    public string[] code = {"Fist", "Open", "Peace"};
    public float roundTime = 60f;
    public float startHoldTime = 1f; //seconds of hand in view to start
    public float resultTime = 6f;

    [Header("ResolumeClips on Layer 1")]
    public int idleClip = 1;
    public int[] stepClips = {2, 3, 4}; //shown after each correct gesture
    public int solvedClip = 5; //shown after all gestures are done
    public int failedClip = 6; //shown after time runs out

    public State state { get; private set; } 

    int step;
    float stateStart;
    float handSeenSince = -1f;

    void OnEnable()
    {
        hand.GestureChanged += OnGesture;
    }

    void OnDisable()
    {
        hand.GestureChanged -= OnGesture;
    }

    void Start()
    {
        SetState(State.Idle);
    }

    void SetState(State newState)
    {
        state = newState;
        stateStart = Time.time;
        step = 0;
        handSeenSince = -1f;
        Debug.Log($"State changed to {state}");

        switch (state)
        {
            case State.Idle:
                resolume.TriggerClip(1, idleClip);
                break;
            case State.Playing:
                step = 0;
                resolume.TriggerClip(1, stepClips[step]);
                break;
            case State.Solved:
                resolume.TriggerClip(1, solvedClip);
                break;
            case State.Failed:
                resolume.TriggerClip(1, failedClip);
                break;
        }
    }


void Update()
    {
        switch (state)
        {
            case State.Idle:
                if (hand.realGesture != "-")
                {
                    if (handSeenSince < 0f) handSeenSince = Time.time;
                    else if (Time.time - handSeenSince > startHoldTime)
                    {
                        SetState(State.Playing);
                    }
                }
                else
                {
                    handSeenSince = -1f;
                }
                break;
            case State.Playing:
                if (Time.time - stateStart > roundTime)
                {
                    SetState(State.Failed);
                }
                break;
            case State.Solved:
            case State.Failed:
                if (Time.time - stateStart > resultTime)
                {
                    SetState(State.Idle);
                }
                break;
        }
    }

    void OnGesture(string g)
    {
        if(state != State.Playing) return;
        if(g == "-" || g == "Unknown") return;

        if(g == code[step])
        {
            step++;
            Debug.Log($"Correct gesture {g}, step {step}/{code.Length}");

            if(step >= code.Length) {SetState(State.Solved); return;}

            int clip = step - 1 < stepClips.Length ? stepClips[step - 1] : idleClip;
            resolume.TriggerClip(1, clip);
        }
        else
        {
            Debug.Log("[Room] wrong, progress reset");
            step = 0;
            resolume.TriggerClip(1, idleClip);
        }
    }

    void OnGUI()
    {
        float left  = Mathf.Max(0, roundTime - (Time.time - stateStart));
        string Timer = state == State.Playing ? $"Time left: {left:F1}s" : "";
        GUI.Label(new Rect(540, 10, 500, 25), $"State: {state} step {step}/{code.Length} {Timer}"); 
    }

    // void OnEnable()
    // {
    //     hand.GestureChanged += OnGestureChanged;
    // }

    // void OnDisable()
    // {
    //     hand.GestureChanged -= OnGestureChanged;
    // }

    // void OnGestureChanged(string gesture)
    // {
    //     switch (gesture)
    //     {
    //         case "Fist":
    //             resolume.TriggerClip(1, 1);
    //             break;
    //         case "Open":
    //             resolume.TriggerClip(1, 2);
    //             break;
    //         case "Pinch":
    //             resolume.TriggerClip(1, 3);
    //             break;
    //     }
    // }
}
