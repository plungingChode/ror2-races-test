using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ExamplePlugin {
    internal class EventHandlers {
        public EventHandlers(State globalState) {
            this.globalState = globalState;
        }

        struct OnVictoryRequest : IIntoJson {
            public string player;

            public string IntoJson() {
                return JsonUtility.ToJson(player);
            }
        }

        struct OnVictoryResponse {
        }

        public void OnVictory() {
            // TODO use globalState.IsRacing instead
            if (!globalState.IsSignedIn) {
                return;
            }

            var payload = new OnVictoryRequest {
                player = globalState.PlayerName
            };

            var req = Api.PostRequest(
                Endpoints.Victory,
                payload,
                onError: (UnityWebRequest req) => {
                    Log.LogFatal("TODO store results locally, try to upload later");
                    Api.LogError(req);
                },
                onDone: (OnVictoryResponse _, UnityWebRequest _) => {
                    Debug.Log($"Victory added for {payload.player}");
                    // TODO remove current loadout from list
                    // TODO set current loadout to an available one
                }
            );
            Api.Send(req);
        }

        private readonly State globalState;
    }
}
