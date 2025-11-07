using System;
using System.Collections;
using System.Collections.Generic;
using U9.Utils;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequestHelper {
    
    //-----------------------------------------------------------------------------------------------------------------------------------//
    public static Coroutine PerformWebRequest(string url, float timeout, DownloadHandlerBuffer downloadBuffer, Action<string> onFailure, Action<DownloadHandlerBuffer> onSuccess)
    {
        return PerformWebRequest(null, url, timeout, downloadBuffer, onFailure, onSuccess);
    }


    //-----------------------------------------------------------------------------------------------------------------------------------//
    public static Coroutine PerformWebRequest(MonoBehaviour routineContainer, string url, float timeout, DownloadHandlerBuffer downloadBuffer, Action<string> onFailure, Action<DownloadHandlerBuffer> onSuccess)
    {
        if (routineContainer == null)
            routineContainer = CoroutineHandler.GetInstance(true);

        return routineContainer.StartCoroutine(WebRequestIenumerator(url, timeout, downloadBuffer, onFailure, onSuccess));
    }


    //-----------------------------------------------------------------------------------------------------------------------------------//
    public static Coroutine PerformWebRequest(string url, float timeout, Action<string> onFailure, Action onSuccess)
    {
        return PerformWebRequest(null, url, timeout, onFailure, onSuccess);
    }


    //-----------------------------------------------------------------------------------------------------------------------------------//
    public static Coroutine PerformWebRequest(MonoBehaviour routineContainer, string url, float timeout, Action<string> onFailure, Action onSuccess)
    {
        if (routineContainer == null)
            routineContainer = CoroutineHandler.GetInstance(true);

        return routineContainer.StartCoroutine(WebRequestIenumerator(url, timeout, onFailure, onSuccess));
    }


    //-----------------------------------------------------------------------------------------------------------------------------------//
    public static Coroutine PerformWebTextureRequest(string url, float timeout, Action<string> onFailure, Action<Texture> onSuccess)
    {
        return PerformWebTextureRequest(null, url, timeout, onFailure, onSuccess);
    }


    //-----------------------------------------------------------------------------------------------------------------------------------//
    public static Coroutine PerformWebTextureRequest(MonoBehaviour routineContainer, string url, float timeout, Action<string> onFailure, Action<Texture> onSuccess)
    {
        if (routineContainer == null)
            routineContainer = CoroutineHandler.GetInstance(true);

        return routineContainer.StartCoroutine(WebTextureRequestIenumerator(url, timeout, onFailure, onSuccess));
    }



    //-----------------------------------------------------------------------------------------------------------------------------------//
    /// <summary>
    /// Creates an Ienumerator that performs a Webrequest that expects a download
    /// </summary>
    /// <param name="url">The URL to send the request to</param>
    /// <param name="timeout">How long should the request wait for a responce?</param>
    /// <param name="downloadBuffer">The buffer to handle the downloaded data</param>
    /// <param name="onFailure">On failure action</param>
    /// <param name="onSuccess">On success action</param>
    /// <returns></returns>
    public static IEnumerator WebRequestIenumerator (string url, float timeout, DownloadHandlerBuffer downloadBuffer, Action<string> onFailure, Action<DownloadHandlerBuffer> onSuccess)
    {
        //Base Params
        float timer = 0;
        bool timedOut = false;
        
        //The web request
        UnityWebRequest webRequest = new UnityWebRequest(url);
        webRequest.downloadHandler = downloadBuffer;

        yield return null;

        //Send it
        webRequest.SendWebRequest();
        
        //Wait for it to finish
        while(!webRequest.isDone)
        {

            //Handle the timeout
            timer += Time.deltaTime;
            if(timer > timeout)
            {
                timedOut = true;
                break;
            }

            yield return null;
        }


        //If we succeeded, notify
        if(!timedOut && webRequest.error == null)
        {
            onSuccess(downloadBuffer);
        }
        //If we failed, notify
        else
        {
            if (webRequest.error == null)
                onFailure("Timed out.");
            else
                onFailure(webRequest.error);
        }

        yield return null;

        //Finally, dispose of the request.
        webRequest.Dispose();
    }


    //-----------------------------------------------------------------------------------------------------------------------------------//
    /// <summary>
    /// Creates an Ienumerator that performs a Webrequest
    /// </summary>
    /// <param name="url">The URL to send the request to</param>
    /// <param name="timeout">How long should the request wait for a responce?</param>
    /// <param name="downloadBuffer">The buffer to handle the downloaded data</param>
    /// <param name="onFailure">On failure action</param>
    /// <param name="onSuccess">On success action</param>
    /// <returns></returns>
    public static IEnumerator WebRequestIenumerator(string url, float timeout, Action<string> onFailure, Action onSuccess)
    {
        //Base Params
        float timer = 0;
        bool timedOut = false;

        //The web request
        UnityWebRequest webRequest = new UnityWebRequest(url);

        yield return null;

        //Send it
        webRequest.SendWebRequest();

        //Wait for it to finish
        while (!webRequest.isDone)
        {

            //Handle the timeout
            timer += Time.deltaTime;
            if (timer > timeout)
            {
                timedOut = true;
                break;
            }

            yield return null;
        }


        //If we succeeded, notify
        if (!timedOut && webRequest.error == null)
        {
            onSuccess();
        }
        //If we failed, notify
        else
        {
            if (webRequest.error == null)
                onFailure("Timed out.");
            else
                onFailure(webRequest.error);
        }

        yield return null;

        //Finally, dispose of the request.
        webRequest.Dispose();
    }


    //-----------------------------------------------------------------------------------------------------------------------------------//
    /// <summary>
    /// Creates an Ienumerator that performs a Webrequest for a texture
    /// </summary>
    /// <param name="url">The URL to send the request to</param>
    /// <param name="timeout">How long should the request wait for a responce?</param>
    /// <param name="downloadBuffer">The buffer to handle the downloaded data</param>
    /// <param name="onFailure">On failure action</param>
    /// <param name="onSuccess">On success action</param>
    /// <returns></returns>
    public static IEnumerator WebTextureRequestIenumerator(string url, float timeout, Action<string> onFailure, Action<Texture> onSuccess)
    {
        //Base Params
        float timer = 0;
        bool timedOut = false;

        //The web request
        UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);

        yield return null;

        //Send it
        webRequest.SendWebRequest();

        //Wait for it to finish
        while (!webRequest.isDone)
        {

            //Handle the timeout
            timer += Time.deltaTime;
            if (timer > timeout)
            {
                timedOut = true;
                break;
            }

            yield return null;
        }


        //If we succeeded, notify
        if (!timedOut && webRequest.error == null)
        {
            onSuccess(((DownloadHandlerTexture)webRequest.downloadHandler).texture);
        }
        //If we failed, notify
        else
        {
            if (webRequest.error == null)
                onFailure("Timed out.");
            else
                onFailure(webRequest.error);
        }

        yield return null;

        //Finally, dispose of the request.
        webRequest.Dispose();
    }
}
