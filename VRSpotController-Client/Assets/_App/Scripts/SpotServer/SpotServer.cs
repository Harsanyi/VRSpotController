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
            
            var req = new UnityWebRequest(Config.loaded.serverAddress+End.HELLO);
            req.method = "POST";
            req.timeout = 4000;
            req.downloadHandler = new DownloadHandlerBuffer();

            string json = new HelloRequest("Unity Client").ToString();
            byte[] data = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(data);
            req.uploadHandler.contentType = "application/json";

            await req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                throw new Exception(req.error);
            }
            
            Debug.Log("Got resp:"+req.downloadHandler.text);
            json = req.downloadHandler.text;
            Response resp = JsonUtility.FromJson<Response>(json);

            if (!resp.success)
            {
                throw new Exception(resp.message);
            }
            
            Debug.Log(resp.message);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
