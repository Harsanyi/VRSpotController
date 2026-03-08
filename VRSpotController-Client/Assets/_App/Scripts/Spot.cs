using System;
using System.Collections;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Spot : MonoBehaviour
{
    private const int MOVE_UPDATE_FREQUENCY = 10;
    private const int IMAGE_UPDATE_FREQUENCY = 1;
    private const int IMAGE_ANGLE_RESOLUTION = 10;
    private const float CAM_DISTANCE = 1000;
    
    public static Spot instance { get; private set; } = null;

    [SerializeField] private Material camMaterial;
    
    private SpotServer server;
    private Vector2 move;

    private void Awake()
    {
        if(instance)Debug.LogError("Multiple Spots Detected!");
        instance = this;
    }

    private void Start()
    {
        server = new SpotServer();
        StartCoroutine(MoveRoutine());

        //img is 640x480
        float fovH = 72f;
        float fovV = fovH * 480f / 640f;
        Vector2 fov = new(fovH,fovV);
        StartCoroutine(ImageUpdateRoutine(ImageSource.BACK, Quaternion.Euler(0,180,0), fov));
        StartCoroutine(ImageUpdateRoutine(ImageSource.LEFT, Quaternion.Euler(0,-180+72f,0), fov));
        StartCoroutine(ImageUpdateRoutine(ImageSource.RIGHT, Quaternion.Euler(0,180f-72f,0), fov));
        StartCoroutine(ImageUpdateRoutine(ImageSource.FRONT_LEFT, Quaternion.Euler(0, 72*0.5f, -0), fov));
        StartCoroutine(ImageUpdateRoutine(ImageSource.FRONT_RIGHT, Quaternion.Euler(0, -72*0.5f, -0), fov));
    }
    
    public void Stand()
    {
        server.SetStanding(!server.isStanding);
    }

    public void PowerOn()
    {
        server.SetPowerOn(!server.isPoweredOn);
    }

    public void Move(Vector2 dir)
    {
        move = dir;
    }

    IEnumerator MoveRoutine()
    {
        Vector2 lastSended = default;
        Stopwatch watch = Stopwatch.StartNew();
        const int SEND_MS = 1000 / MOVE_UPDATE_FREQUENCY;
        while (Application.isPlaying)
        {
            watch.Restart();
            if (move != lastSended)
            {
                server.Move(move);
                lastSended = move;
            }

            long remaningMs = SEND_MS - watch.ElapsedMilliseconds;
            if(remaningMs > 0)yield return new WaitForSeconds(remaningMs / 1000f);
        }
    }

    IEnumerator ImageUpdateRoutine(string source, Quaternion rotation, Vector2 angle)
    {
        GameObject go = new GameObject(source);
        go.transform.SetParent(this.transform);
        go.transform.localScale = Vector3.one * CAM_DISTANCE;
        go.transform.rotation = rotation;
        
        var filter = go.AddComponent<MeshFilter>();
        filter.mesh = Sphere.GetSlice(angle.x, angle.y, IMAGE_ANGLE_RESOLUTION);
        var renderer = go.AddComponent<MeshRenderer>();
        var mat = Instantiate(camMaterial);
        renderer.material = mat;
        
        Stopwatch watch = Stopwatch.StartNew();
        const int SEND_MS = 1000 / IMAGE_UPDATE_FREQUENCY;
        Texture2D imageTexture = null;
        while (Application.isPlaying)
        {
            watch.Restart();

            var task = server.GetImage(source);
            while (!task.IsCompleted)
            {
                yield return null;
            }
            
            if(imageTexture != null)Destroy(imageTexture);
            imageTexture = task.Result;
            Debug.Log($"Got Image:{source} w:{imageTexture.width} h:{imageTexture.height}");
            mat.SetTexture("_BaseMap", imageTexture);
            
            long remainingMs = SEND_MS - watch.ElapsedMilliseconds;
            if(remainingMs > 0)yield return new WaitForSeconds(remainingMs / 1000f);
        }
    }
}
