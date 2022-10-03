using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ExamplePlugin;

/// <summary>
/// Something that can be serialized into a JSON string.
/// </summary>
internal interface IIntoJson {
    string IntoJson();
}

internal class NoInput : IIntoJson {
    public string IntoJson() {
        throw new NotImplementedException();
    }
}

internal class Api {
    // TODO this should be loaded from a config file
    const string BASE_URL = "https://httpbin.org";

    // TODO there should also be one of these, if this ever goes public
    const string API_KEY = "???";

    // TODO should be better typed, but w/e
    /// <summary>
    /// Execute a prepared <see cref="UnityWebRequest"/> as a coroutine.
    /// </summary>
    /// <returns>The coroutine handle.</returns>
    public static Coroutine Send(IEnumerator request) {
        return RoR2.RoR2Application.instance.StartCoroutine(request);
    }

    /// <summary>
    /// Prepare a POST request without any input data.
    /// </summary>
    /// <typeparam name="O"></typeparam>
    /// <param name="endpoint"></param>
    /// <param name="onError"></param>
    /// <param name="onDone"></param>
    /// <returns></returns>
    public static IEnumerator PostRequest<O>(
        Endpoint endpoint,
        Action<UnityWebRequest>? onError,
        Action<O, UnityWebRequest>? onDone
    ) {
        return PostRequest<NoInput, O>(endpoint, null, onError, onDone);
    }

    // TODO generic return type `O` should be constrained but idk how to 
    //      require a type be constructible/convertable from another
    /// <summary>
    /// Prepare a POST request.
    /// </summary>
    /// <typeparam name="I">Input (request payload) type</typeparam>
    /// <typeparam name="O">Ouput (response payload) type</typeparam>
    /// <param name="endpoint">An endpoint to send this request to.</param>
    /// <param name="payload">The payload to send as JSON.</param>
    /// <param name="onError">
    /// Callback invoked on network error or in case of a 
    /// rejected/failed request.
    /// </param>
    /// <param name="onDone">Callback invoked on a successful request.</param>
    /// <returns></returns>
    public static IEnumerator PostRequest<I, O>(
        Endpoint endpoint,
        I? payload, 
        Action<UnityWebRequest>? onError = null, 
        Action<O, UnityWebRequest>? onDone = null
    ) 
        where I: IIntoJson
    {
        var req = new UnityWebRequest($"{BASE_URL}/{endpoint}", "POST");
        req.downloadHandler = new DownloadHandlerBuffer();

        if (payload != null) {
            var payloadString = payload.IntoJson();
            var payloadBytes = Encoding.UTF8.GetBytes(payloadString);
            req.SetRequestHeader("content-type", "application/json");
            req.uploadHandler = new UploadHandlerRaw(payloadBytes);

            Log.LogDebug($"Sending payload text: {payloadString}");
        }
        else {
            req.uploadHandler = new UploadHandler();
        }

        yield return req.SendWebRequest();

        if (IsError(req) && onError != null) {
            Log.LogDebug($"Web request error: {req.error}");
            onError(req);
        } else if (onDone != null){
            Log.LogDebug($"Received response text: {req.downloadHandler.text}");
            var res = JsonUtility.FromJson<O>(req.downloadHandler.text);
            onDone(res, req);
        }
    }

    /// <summary>
    /// Default "error handler" for web request. Just logs the request's
    /// error to the console.
    /// </summary>
    /// <param name="req"></param>
    public static void LogError(UnityWebRequest req) {
        Debug.LogError($"Request failed with error: {req.error}");
    }

    private static bool IsError(UnityWebRequest req) {
        return req.responseCode < 200 || 299 < req.responseCode;
    }
}

/// <summary>
/// A URL path, representing an HTTP endpoint.
/// </summary>
class Endpoint {
    // TODO add option for query string
    public Endpoint(string path) {
        this._path = path;
    }

    private readonly string _path;
    public string Path => _path;

    public static explicit operator Endpoint(string s) => new(s);
}

/// <summary>
/// List of endpoints used by the mod.
/// </summary>
internal static class Endpoints {
    public static Endpoint SignIn = (Endpoint)"/post";
    public static Endpoint SignOut = (Endpoint)"/post";
    public static Endpoint SetupRace = (Endpoint)"/post";
    public static Endpoint Victory = (Endpoint)"/post";
    public static Endpoint ReverseVictory = (Endpoint)"/post";
    public static Endpoint Reset = (Endpoint)"/post";
    public static Endpoint HardReset = (Endpoint)"/post";
    public static Endpoint ClockIn = (Endpoint)"/post";
    public static Endpoint ClockOut = (Endpoint)"/post";
}

internal enum HttpMethod {
    POST,
    //GET,
}
