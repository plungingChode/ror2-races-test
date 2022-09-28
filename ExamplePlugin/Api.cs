using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ExamplePlugin {
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

        // TODO should be better typed, but w/e
        public static Coroutine Send(MonoBehaviour instance, IEnumerator request) {
            return instance.StartCoroutine(request);
        }

        public static IEnumerator PostRequest<O>(
            string endpoint,
            Action<UnityWebRequest>? onError,
            Action<O, UnityWebRequest>? onDone
        ) {
            return PostRequest<NoInput, O>(endpoint, null, onError, onDone);
        }

        public static IEnumerator PostRequest(string endpoint) {
            // Generic parameter required. Weird, but ok.
            return PostRequest<IIntoJson, object>(endpoint, null, null, null);
        }

        public static IEnumerator PostRequest<I>(string endpoint, I? payload)
        where I: IIntoJson
        {
            return PostRequest<I, object>(endpoint, payload, null, null);
        }

        public static IEnumerator PostRequest<I>(
            string endpoint, 
            I? payload, 
            Action<UnityWebRequest>? onError
        )
        where I: IIntoJson 
        {
            return PostRequest<I, object>(endpoint, payload, onError, null);
        }

        // TODO generic return type `O` should be constrained but idk how to 
        //      require a type be constructible/convertable from another
        public static IEnumerator PostRequest<I, O>(
            string endpoint, 
            I? payload, 
            Action<UnityWebRequest>? onError, 
            Action<O, UnityWebRequest>? onDone
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

                Log.LogInfo($"Sending payload text: {payloadString}");
            }
            else {
                req.uploadHandler = new UploadHandler();
            }

            yield return req.SendWebRequest();

            if (IsError(req) && onError != null) {
                Log.LogInfo($"Web request error: {req.error}");
                onError(req);
            } else if (onDone != null){
                Log.LogInfo($"Received response text: {req.downloadHandler.text}");
                var res = JsonUtility.FromJson<O>(req.downloadHandler.text);
                onDone(res, req);
            }
        }

        public static void LogError(UnityWebRequest req) {
            Debug.LogError($"Request failed with error: {req.error}");
        }

        private static bool IsError(UnityWebRequest req) {
            return req.responseCode < 200 || 299 < req.responseCode;
        }
    }
}
