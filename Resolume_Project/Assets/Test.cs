using UnityEngine;

public class Test : MonoBehaviour
{

      public ResolumeOsc resolume;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resolume.TriggerClip(1, 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
