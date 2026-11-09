using UnityEngine;
using Unity.InferenceEngine;
using System;

public class HandLiveTest : MonoBehaviour
{
    public ModelAsset landmarkModel;
    public float detectionThreshold = 1f;

    public int framesNeeded = 5;
    public int lostFramesAllowed = 10;
    public float smoothing = 0.5f;

    string candidate = "-";
    int candidateFrames = 0;
    int lostFrames = 0;

    WebCamTexture cam;
    RenderTexture crop;
    Model model;
    Worker worker;

    Vector3[] lm = new Vector3[21];
    float threshold;
    string gesture = "-";
    bool haveHand;
    bool loggedShapes;

    public float indexThreshold = 1.1f;
    public float middleThreshold = 1.1f;
    public float ringThreshold = 1.1f;
    public float pinkyThreshold = 1.1f;
    public float pinchThreshold = 0.25f;

    Tensor<float> input;

    public string realGesture { get; private set; } = "-";
    public event Action<string> GestureChanged;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = new WebCamTexture(640, 480, 30);
        cam.Play();

        crop  = new RenderTexture(256, 256, 0);

        model = ModelLoader.Load(landmarkModel);
        worker = new Worker(model, BackendType.GPUCompute);

        input = new Tensor<float>(new TensorShape(1, 3, 256, 256));
    }

    // Update is called once per frame
    void Update()
    {

        HandGestures.extendedThreshold[0] = indexThreshold;
        HandGestures.extendedThreshold[1] = middleThreshold;
        HandGestures.extendedThreshold[2] = ringThreshold;
        HandGestures.extendedThreshold[3] = pinkyThreshold;
        HandGestures.pinchThreshold = pinchThreshold;
        if(!cam.didUpdateThisFrame) return;

        float side = Mathf.Min(cam.width, cam.height);
        var scale = new Vector2(side / cam.width, side / cam.height);
        var offset = new Vector2((1 - scale.x) / 2, (1 - scale.y) / 2);

        Graphics.Blit(cam, crop, scale, offset);

        TextureConverter.ToTensor(crop, input, new TextureTransform());
        worker.Schedule(input);


        bool gotLandmarks = false;
        threshold = 0;

        for (int i = 0; i < model.outputs.Count; i++)
        {
            var output = worker.PeekOutput(i) as Tensor<float>;
            if(output == null) continue;

            if(!loggedShapes)
            {
                Debug.Log($"output {i} shape {output.shape}");
                
            }

            var a = output.DownloadToNativeArray();

            if(a.Length == 1 && threshold == 0)
            {
                threshold = a[0];
            }
            else if (a.Length == 63 && !gotLandmarks)
            {
                for(int j = 0; j < 21; j++)
                {
                    Vector3 p = new Vector3(a[j*3], a[j*3+1], a[j*3+2]);
                    lm[j] =haveHand ? Vector3.Lerp(lm[j], p, smoothing) : p;
                   // lm[j] = new Vector3(a[j*3], a[j*3+1], a[j*3+2]);
                }
                gotLandmarks = true;
            }
        }
        loggedShapes = true;

        haveHand = gotLandmarks && threshold > detectionThreshold;
        // gesture = haveHand ? HandGestures.Classify(lm) : "-";

        // if(gesture != realGesture)
        // {
        //     realGesture = gesture;
        //     GestureChanged?.Invoke(realGesture);
        // }

        string raw;
        if(haveHand)
        {
            lostFrames = 0;
            raw = HandGestures.Classify(lm);
        }
        else
        {
            lostFrames++;
           raw = lostFrames > lostFramesAllowed ? "-" : candidate;
        }

        if(raw == candidate) candidateFrames++;
        else
        {
            candidateFrames = 1;
            candidate = raw;
        }

        if(candidateFrames >= framesNeeded && candidate != realGesture)
        {
            realGesture = candidate;
            gesture = candidate;
            GestureChanged?.Invoke(realGesture);
        }

    }

    void OnGUI()
    {
        const float size = 512f;
        GUI.DrawTexture(new Rect(10, 10, size, size), crop);

        if(haveHand)
        {
            for(int k = 0; k < 21; k++)
            {
                float x = 10 + lm[k].x / 256f * size;
                float y = 10 + lm[k].y / 256f * size;
                GUI.DrawTexture(new Rect(x-3, y-3, 6, 6), Texture2D.whiteTexture);
            }
       

        GUI.Label(new Rect(10, 10 + size + 20, 500, 25), $"Gesture: {gesture}");
        GUI.Label(new Rect(10, 10 + size + 40, 500, 25), $"threshold: {threshold}");
        GUI.Label(new Rect(10, 10 + size + 60, 500, 25), $"pinch: {(haveHand ? HandGestures.PinchAmount(lm) : 0f):F2}");
        string[] names = { "idx", "mid", "ring", "pinky" };
        string line = "";
        for (int f = 0; f < 4; f++)
        line += $"{names[f]} {HandGestures.FingerRatio(lm, f):F2}{(HandGestures.FingerExtended(lm, f) ? "*" : "")}   ";
        GUI.Label(new Rect(10, 10 + size + 80, 700, 25), line);
          }
    }

    void OnDestroy()
    {
        input?.Dispose();
        worker?.Dispose();
        crop?.Release();
        if (cam != null) cam.Stop();
    }



 



}
