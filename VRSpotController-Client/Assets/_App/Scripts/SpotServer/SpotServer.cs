using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;
using Ping = System.Net.NetworkInformation.Ping;

public class SpotServer
{
    private const int TIMEOUT = 4000;
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
            var req = new MoveRequest(move);
            var resp = await SendRequest<Response>(End.MOVE, Methode.POST, req);
            
            if(!resp.success)throw new Exception(resp.message);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
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
}

public class Methode
{
    public const string POST = "POST";
    public const string GET = "GET";
}
