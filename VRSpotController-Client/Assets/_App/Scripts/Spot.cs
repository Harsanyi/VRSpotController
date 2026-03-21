using System;
using System.Collections;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Spot : MonoBehaviour
{
    private const int MOVE_UPDATE_FREQUENCY = 10;
    private const int IMAGE_UPDATE_FREQUENCY = 20;
    private const int PITCH_UPDATE_FREQUENCY = 10;
    private const int IMAGE_ANGLE_RESOLUTION = 10;
    private const int GRIP_UPDATE_FREQUENCY = 20;
    private const float CAM_DISTANCE = 500;
    private const int STATE_UPDATE_FREQUENCY = 10;
    const float FOVH = 72f;
    
    public static Spot instance { get; private set; } = null;

    [SerializeField] private Material camMaterial;
    
    private SpotServer server;
    private Vector2 move;
    private float speed = 0.5f;
    private Vector2 rotate;
    public SpotState state { get; private set; }
    
    private float grip;
    private Vector3 armPos;
    private Quaternion armRot;
    private bool haveArmControll = false;
    private Coroutine armControllRoutine;
    private Coroutine armImageUpdateRoutine;
    private Coroutine gripUpdateRoutine;
    
    private void Awake()
    {
        if(instance)Debug.LogError("Multiple Spots Detected!");
        instance = this;
    }

    private void Start()
    {
        server = new SpotServer();
        StartCoroutine(MoveRoutine());
        StartCoroutine(PitchRoutine());
        
        StartCoroutine(ImageUpdateRoutine(ImageSource.BACK, FOVH, 180, Sphere.UVDir.UP, 0.99f));
        StartCoroutine(ImageUpdateRoutine(ImageSource.LEFT,  FOVH,-180+FOVH, Sphere.UVDir.UP));
        StartCoroutine(ImageUpdateRoutine(ImageSource.RIGHT, FOVH,180f-FOVH, Sphere.UVDir.DOWN));
        StartCoroutine(ImageUpdateRoutine(ImageSource.FRONT_LEFT, FOVH,30, Sphere.UVDir.LEFT, 1.01f));
        StartCoroutine(ImageUpdateRoutine(ImageSource.FRONT_RIGHT, FOVH,-30, Sphere.UVDir.LEFT));
        StartCoroutine(StateUpdateRoutine());
    }

    private void Update()
    {
        if(state == null)return;
        
        this.transform.rotation = Quaternion.Euler(-state.rot.eulerAngles.x, 0f, 0f);
        if (haveArmControll)
        {
            UpdateArmImagePosition();
        }
    }

    void UpdateArmImagePosition()
    {
        var imgTrans = transform.Find(ImageSource.HAND);
        if(!imgTrans)return;

        if (imgTrans.localScale.x != 1f) imgTrans.localScale = Vector3.one;
        imgTrans.position = armPos;
        imgTrans.rotation = armRot;
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
        move = dir * speed;
    }
    
    public void Rotate(Vector2 rot)
    {
        rotate = rot;
    }

    public void SetArmControll(bool val)
    {
        if(haveArmControll == val)return;
        haveArmControll = val;
        
        if(armImageUpdateRoutine != null)StopCoroutine(armImageUpdateRoutine);
        if(gripUpdateRoutine != null)StopCoroutine(gripUpdateRoutine);
        
        if (haveArmControll)
        {
            armImageUpdateRoutine = StartCoroutine(ImageUpdateRoutine(ImageSource.HAND, FOVH, 0,  Sphere.UVDir.UP, 0.1f));
            gripUpdateRoutine = StartCoroutine(GripRoutine());
        }

        if (!haveArmControll)
        {
            DestroyImageIfExist(ImageSource.HAND);
        }

        void DestroyImageIfExist(string source)
        {
            var sTrans = transform.Find(source);
            if(!sTrans)return;

            var renderer = sTrans.GetComponentInChildren<Renderer>();
            var mat = renderer.material;
            var tex = renderer.material.GetTexture("_BaseMap");
            Destroy(tex);
            Destroy(mat);
            Destroy(sTrans.gameObject);
        }
    }

    public void Grip(float val)
    {
        grip = val;
    }

    public void ArmTransform(Vector3 pos, Quaternion rot)
    {
        armPos = pos;
        armRot = rot;
    }

    IEnumerator MoveRoutine()
    {
        Vector2 lastMove = default;
        float lastYaw = 0;
        Stopwatch watch = Stopwatch.StartNew();
        const int SEND_MS = 1000 / MOVE_UPDATE_FREQUENCY;
        while (Application.isPlaying)
        {
            watch.Restart();
            if (move != lastMove || rotate.x != lastYaw)
            {
                server.Move(move, rotate.x);
                lastMove = move;
                lastYaw = rotate.x;
            }

            long remaningMs = SEND_MS - watch.ElapsedMilliseconds;
            if(remaningMs > 0)yield return new WaitForSeconds(remaningMs / 1000f);
        }
    }

    IEnumerator ArmPosRoutine()
    {
        Vector3 lastPos = default;
        Stopwatch watch = Stopwatch.StartNew();
        const int SEND_MS = 1000 / MOVE_UPDATE_FREQUENCY;
        while (Application.isPlaying)
        {
            watch.Restart();
            if (armPos != lastPos)
            {
                server.ArmPos(armPos);
                lastPos = armPos;
            }

            long remaningMs = SEND_MS - watch.ElapsedMilliseconds;
            if(remaningMs > 0)yield return new WaitForSeconds(remaningMs / 1000f);
        }
    }

    IEnumerator PitchRoutine()
    {
        float lastPitch = 0;
        Stopwatch watch = Stopwatch.StartNew();
        const int SEND_MS = 1000 / PITCH_UPDATE_FREQUENCY;
        while (Application.isPlaying)
        {
            watch.Restart();
            if (rotate.y != lastPitch)
            {
                server.Pitch(rotate.y);
                lastPitch = rotate.y;
            }

            long remaningMs = SEND_MS - watch.ElapsedMilliseconds;
            if(remaningMs > 0)yield return new WaitForSeconds(remaningMs / 1000f);
        }
    }

    IEnumerator GripRoutine()
    {
        float lastGrip = 0;
        Stopwatch watch = Stopwatch.StartNew();
        const int SEND_MS = 1000 / GRIP_UPDATE_FREQUENCY;
        while (Application.isPlaying)
        {
            watch.Restart();
            if (grip != lastGrip)
            {
                server.Grip(grip);
                lastGrip = grip;
            }
            
            long remaningMs = SEND_MS - watch.ElapsedMilliseconds;
            if(remaningMs > 0)yield return new WaitForSeconds(remaningMs / 1000f);
        }
    }

    IEnumerator ImageUpdateRoutine(string source, float fovH,  float yawAngle, Sphere.UVDir uvDir, float scale=1f)
    {
        Material mat = null;
        
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
            imageTexture.name = source;
            UpdateMaterial(imageTexture);
            
            long remainingMs = SEND_MS - watch.ElapsedMilliseconds;
            if(remainingMs > 0)yield return new WaitForSeconds(remainingMs / 1000f);
        }

        void UpdateMaterial(Texture2D texture)
        {
            if (mat == null)
            {
                GameObject go = new GameObject(source);
                go.transform.SetParent(this.transform);
                go.transform.localScale = Vector3.one * CAM_DISTANCE * scale;
                go.transform.rotation = Quaternion.Euler(0, yawAngle, 0);
                go.name = texture.name;
        
                var filter = go.AddComponent<MeshFilter>();
                float fovV = texture.height / (float)texture.width * fovH;
                filter.mesh = Sphere.GetSlice(fovH, fovV, IMAGE_ANGLE_RESOLUTION, uvDir);
                var renderer = go.AddComponent<MeshRenderer>();
                mat = Instantiate(camMaterial);
                renderer.material = mat;
            }
            
            mat.SetTexture("_BaseMap", imageTexture);
        }
    }
    
    IEnumerator StateUpdateRoutine()
    {
        int updateTime = 1000 / STATE_UPDATE_FREQUENCY;
        Stopwatch watch = new Stopwatch();
        while (Application.isPlaying)
        {
            watch.Restart();

            var task = server.GetState();
            while (!task.IsCompleted)
            {
                yield return null;
            }
            
            state = task.Result;

            long remains = updateTime - watch.ElapsedMilliseconds;
            if (remains > 0) yield return new WaitForSeconds(remains / 1000f);
        }
    }
}
