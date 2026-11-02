using UnityEngine;

public static class HandGestures 
{
    public static float[] extendedThreshold = { 1.1f, 1.1f, 1.1f, 1.1f };
    public static float pinchThreshold = 0.25f;

    static readonly int[] tips = {  8, 12, 16, 20 };
    static readonly int[] pips = {  6, 10, 14, 18 };

    static float Dist2D(Vector3 a, Vector3 b)
    {
        return Vector2.Distance(new Vector2(a.x, a.y), new Vector2(b.x, b.y));
    }

    public static float FingerRatio(Vector3[] lm, int finger)
    {
        return Dist2D(lm[tips[finger]], lm[0]) / Mathf.Max(Dist2D(lm[pips[finger]], lm[0]), 0.0001f);
    }


   public static bool FingerExtended(Vector3[] lm, int finger)
   {
        return FingerRatio(lm, finger) > extendedThreshold[finger];
    //return Vector3.Distance(lm[tip], lm[0]) > Vector3.Distance(lm[pip], lm[0]) * 1.1f;
   }
    
    public static float HandSize(Vector3[] lm) => Dist2D(lm[0], lm[9]);

    public static float PinchAmount(Vector3[] lm)
    {
        return Dist2D(lm[4], lm[8]) / Mathf.Max(HandSize(lm), 0.0001f);
    }
    
    public static string Classify(Vector3[] lm)
    {
        if(PinchAmount(lm) < pinchThreshold) return "Pinch";
        
        // bool index = FingerExtended(lm, 8, 6);
        // bool middle = FingerExtended(lm, 12, 10);
        // bool ring = FingerExtended(lm, 16, 14);
        // bool pinky = FingerExtended(lm, 20, 18);
        bool index = FingerExtended(lm,0);
        bool middle = FingerExtended(lm,1);
        bool ring = FingerExtended(lm,2);
        bool pinky = FingerExtended(lm,3);


        if (index && middle && ring && pinky) return "Open";
        if (!index && !middle && !ring && !pinky) return "Fist";
        if (index && !middle && !ring && !pinky) return "Pointing";
        if (index && middle && !ring && !pinky) return "Peace";
        
        return "Unknown";
    }
}
