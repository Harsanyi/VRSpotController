using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
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
}
