using UnityEngine;
using Unity.InferenceEngine; // older versions: Unity.Sentis

public class HandModelTest : MonoBehaviour
{
    public ModelAsset detectorModel;

    WebCamTexture cam;
    Worker worker;

    void Start()
    {
        cam = new WebCamTexture(640, 480, 30);
        cam.Play();

        var model = ModelLoader.Load(detectorModel);
        worker = new Worker(model, BackendType.GPUCompute);
    }

    void Update()
    {
        if (!cam.didUpdateThisFrame) return;

        // Resize the frame to 256x256, 3 channels, values 0-1
        using var input = TextureConverter.ToTensor(cam, 256, 256, 3);
        worker.Schedule(input);

       // Vector3[] landmarks = GetLandmarks();
        var fake = new Vector3[21];
        for(int i = 0; i < 21; i++) fake[i] = Vector3.zero;
        Debug.Log(HandGestures.Classify(fake));


        // if(landmarks != null)
        // {
        //     string gesture = HandGestures.Classify(landmarks);
        //     Debug.Log($"Detected gesture: {gesture}");
        // }

//         var boxes = worker.PeekOutput(0) as Tensor<float>;
//         var scores = worker.PeekOutput(1) as Tensor<float>;

//         var s = scores.DownloadToNativeArray(); // method name may differ by version
//         float max = float.MinValue;
//         for (int i = 0; i < s.Length; i++) if (s[i] > max) max = s[i];
// Debug.Log($"max raw score {max}");

//         Debug.Log($"boxes {boxes.shape}  scores {scores.shape}");
    }

    void OnDestroy()
    {
        worker?.Dispose();
        if (cam != null) cam.Stop();
    }
}