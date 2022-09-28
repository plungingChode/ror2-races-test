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

    internal class Api {
        // TODO this should be loaded from a config file
        const string BASE_URL = "https://httpbin.org";

        // TODO should be better typed, but w/e
        public static Coroutine Send(MonoBehaviour instance, IEnumerator request) {
            return instance.StartCoroutine(request);
        }

        public static IEnumerator PostRequest(string endpoint) {
            // Generic parameter required. Weird, but ok.
            return PostRequest<IIntoJson, object>(endpoint, null, null, null);
        }

        public static IEnumerator PostRequest<I>(string endpoint, I payload)
        where I: IIntoJson
        {
            return PostRequest<I, object>(endpoint, payload, null, null);
        }

        public static IEnumerator PostRequest<I>(
            string endpoint, 
            I payload, 
            Action<UnityWebRequest> onError
        )
        where I: IIntoJson 
        {
            return PostRequest<I, object>(endpoint, payload, onError, null);
        }

        // TODO generic return type `O` should be constrained but idk how to 
        //      require a type be constructible/convertable from another
        public static IEnumerator PostRequest<I, O>(
            string endpoint, 
            I payload, 
            Action<UnityWebRequest> onError, 
            Action<O> onDone
        ) 
        where I: IIntoJson
        {
            var req = new UnityWebRequest($"{BASE_URL}/{endpoint}", "POST");
            var payloadBytes = payload != null 
                ? Encoding.UTF8.GetBytes(payload.IntoJson())
                : new byte[0];

            req.SetRequestHeader("content-type", "application/json");
            req.uploadHandler = new UploadHandlerRaw(payloadBytes);

            yield return req.SendWebRequest();

            if (req.isNetworkError && onError != null) {
                onError(req);
            } else if (onDone != null){
                var res = JsonUtility.FromJson<O>(req.downloadHandler.text);
                onDone(res);
            }
        }
    }
}
