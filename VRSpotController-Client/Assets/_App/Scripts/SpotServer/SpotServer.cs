using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TreeEditor;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;
using Ping = System.Net.NetworkInformation.Ping;
using UnityEngine.Events;

public class SpotServer
{
    private const int TIMEOUT = 4000;
    
    public bool isStanding { get; private set; } = false;
    public bool isPoweredOn { get; private set; } = false;
    
    public async void IsAvaiable()
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
            var resp = await SendRequest<Response>(End.HELLO, Methode.POST, req);
            
            Debug.Log(resp.message);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async void Move(Vector2 move)
    {
        try
        {
            var req = new MoveRequest(new Vector2(move.y, -move.x));
            var resp = await SendRequest<Response>(End.MOVE, Methode.POST, req);
            
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
        var resp = await SendRequest<Response>(End.STAND, Methode.POST, req);

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
        var resp = await SendRequest<Response>(End.POWER, Methode.POST, req);
        
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

    public async Task<Response> SendRequest<T>(string end, string methode, Request request) where T : Response
    {
        try
        {
            string address = Config.loaded.serverAddress + end;
            var req = new UnityWebRequest(address);
            req.method = methode;
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
        return await SendImageRequest(End.IMAGE, Methode.POST, req);
    }

    public async Task<Texture2D> SendImageRequest(string end, string methode, ImageRequest request)
    {
        try
        {
            string address = Config.loaded.serverAddress + end;
            var req = new UnityWebRequest(address);
            req.method = methode;
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
            
            Debug.Log($"Got texture {tex.name}");
            return tex;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return null;
        }
    }
}

public class Methode
{
    public const string POST = "POST";
    public const string GET = "GET";
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
