using UnityEngine;
using System.Collections.Generic;
using System;

 public enum SwipeDir {  left, right }
public class HandSwipe : MonoBehaviour
{

    public event Action<SwipeDir> Swiped;

    public float window = 0.3f;
    public float minDistance = 1.6f;
    public float minSpeed = 5f;
    public float axisDominance = 2f;
    public float cooldown = 0.5f;
    public bool mirrorX = true;

    

    struct Sample { public Vector2 pos; public float t; }
    readonly List<Sample> history = new();
    float lastSwipe = -10f;

    public void Clear() => history.Clear();

    void Start()
    {
        
    }

    // Update is called once per frame
    public void Update(Vector3[] lm, bool haveHand)
    {
        if(!haveHand) { history.Clear();  return; }

        Vector2 p = (V2(lm[0]) + V2(lm[5]) + V2(lm[9]) + V2(lm[13]) + V2(lm[17])) / 5f;

        float size = Mathf.Max(Vector2.Distance(V2(lm[0]), V2(lm[9])), 1f);
        p /= size;

        float now = Time.time;
        history.Add(new Sample { pos = p, t = now });
        while(history.Count > 0 && now - history[0].t > window)  history.RemoveAt(0);

        if (history.Count < 3 || now - lastSwipe < cooldown) return;

        Vector2 d = history[^1].pos - history[0].pos;
        float dt = history[^1].t - history[0].t;
        if (dt <= 0.05f) return;
        float ax = Mathf.Abs(d.x), ay = Mathf.Abs(d.y);
        float dist = Mathf.Max(ax, ay);
        if (dist < minDistance || dist / dt < minSpeed) return;
        if(Mathf.Max(ax, ay) < axisDominance * Mathf.Min(ax, ay)) return;

        SwipeDir dir;
        if(ax > ay) dir = (d.x > 0) != mirrorX ? SwipeDir.right : SwipeDir.left;

        lastSwipe = now;
        history.Clear();
        Swiped?.Invoke(dir);

    }

    static Vector2 V2(Vector3 v) => new Vector2(v.x, v.y);
}
