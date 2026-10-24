using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

public class SpotServer
{
    private const int TIMEOUT = 4000;
    
    public bool isStanding { get; private set; } = false;
    public bool isPoweredOn { get; private set; } = false;
    
    public async void IsAvailable()
    {
        try
        {
            Debug.Log("Check Server Status on "+Config.loaded.serverAddress);
            using var client = new HttpClient();
            var resp = await client.GetAsync(Config.loaded.serverAddress);
            Debug.Log("Http status:"+resp.StatusCode);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async void SayHello()
    {
        try
        {
            Debug.Log("Say Hello");

            var req = new HelloRequest("Unity Client");
            var resp = await SendRequest<Response>(End.HELLO, RequestMethod.POST, req);
            
            Debug.Log(resp.message);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async void Move(Vector2 move, float yaw)
    {
        try
        {
            var req = new MoveRequest(new Vector2(move.y, -move.x), -yaw);
            var resp = await SendRequest<Response>(End.MOVE, RequestMethod.POST, req);
            
            if(!resp.success)throw new Exception(resp.message);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    
    public async void ArmTransform(Vector3 pos, Quaternion rot)
    {
        try
        {
            var req = new ArmRequest(pos.z, -pos.x, pos.y, -rot.z, rot.x, -rot.y, rot.w);
            var resp = await SendRequest<Response>(End.ARM, RequestMethod.POST, req);
            
            if(!resp.success)throw new Exception(resp.message);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    
    public async void Pitch(float pitch)
    {
        try
        {
            var req = new PitchRequest(pitch);
            var resp = await SendRequest<Response>(End.PITCH, RequestMethod.POST, req);
            
            if(!resp.success)throw new Exception(resp.message);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    
    public async void Grip(float grip)
    {
        try
        {
            var req = new GripRequest(grip);
            var resp = await SendRequest<Response>(End.GRIP, RequestMethod.POST, req);
            
            if(!resp.success)throw new Exception(resp.message);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    
    public async void SetStanding(bool standing)
    {
        Debug.Log("Set Standing:"+standing.ToString());
        var req = new StandRequest(standing);
        var resp = await SendRequest<Response>(End.STAND, RequestMethod.POST, req);

        if (resp.success)
        {
            isStanding = standing;
            Debug.Log(resp.message);
        }
        else
        {
            Debug.LogError(resp.message);
        }
    }

    public async void SetPowerOn(bool powerOn)
    {
        Debug.Log("Set Power On:"+powerOn.ToString());
        var req = new PowerOnRequest(powerOn);
        var resp = await SendRequest<Response>(End.POWER, RequestMethod.POST, req);
        
        if (resp.success)
        {
            Debug.Log(resp.message);
            isPoweredOn = powerOn;
        }
        else
        {
            Debug.LogError(resp.message);
        }
    }

    public async Task<Response> SendRequest<T>(string end, string method, Request request) where T : Response
    {
        try
        {
            string address = Config.loaded.serverAddress + end;
            var req = new UnityWebRequest(address);
            req.method = method;
            req.timeout = TIMEOUT;
            req.downloadHandler = new DownloadHandlerBuffer();

            string json = request.ToString();
            byte[] data = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(data);
            req.uploadHandler.contentType = "application/json";

            await req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                throw new Exception(req.error);
            }
            
            json = req.downloadHandler.text;
            T resp = JsonUtility.FromJson<T>(json);

            if (!resp.success)
            {
                throw new Exception(resp.message);
            }
            
            return resp;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return new Response()
            {
                success = false,
                message = e.Message
            };
        }
    }
    
    public async Task<Texture2D> GetImage(string source)
    {
        var req = new ImageRequest(source);
        return await SendImageRequest(End.IMAGE, RequestMethod.POST, req);
    }

    public async Task<Texture2D> SendImageRequest(string end, string method, ImageRequest request)
    {
        try
        {
            string address = Config.loaded.serverAddress + end;
            var req = new UnityWebRequest(address);
            req.method = method;
            req.timeout = TIMEOUT;
            req.downloadHandler = new DownloadHandlerBuffer();

            string json = request.ToString();
            byte[] data = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(data);
            req.uploadHandler.contentType = "application/json";

            await req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                throw new Exception(req.error);
            }

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(req.downloadHandler.data);
            tex.name = request.source + ".jpg";
            tex.Apply();
            
            return tex;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return null;
        }
    }

    public async Task<SpotState> GetState()
    {
        var req = new Request();
        var resp = await SendRequest<StateResponse>(End.STATE, RequestMethod.POST, req) as StateResponse;
        return resp.state;
    }
}

public class RequestMethod
{
    public const string POST = "POST";
}

public class ImageSource
{
    public const string BACK = "back_fisheye_image";
    public const string FRONT_LEFT = "frontleft_fisheye_image";
    public const string FRONT_RIGHT = "frontright_fisheye_image";
    public const string LEFT = "left_fisheye_image";
    public const string RIGHT = "right_fisheye_image";
    public const string HAND = "hand_color_image";
}
