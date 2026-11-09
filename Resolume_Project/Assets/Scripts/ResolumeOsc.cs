using UnityEngine;
using OscJack;

public class ResolumeOsc : MonoBehaviour
{
    OscClient client;

    void Awake() => client = new OscClient("127.0.0.1", 7000);

    public void SetLayerOpacity(int layer, float value) =>
        client.Send($"/composition/layers/{layer}/video/opacity", value);

    public void TriggerClip(int layer, int clip) =>
        client.Send($"/composition/layers/{layer}/clips/{clip}/connect", 1);

    void OnDestroy() => client?.Dispose();
}