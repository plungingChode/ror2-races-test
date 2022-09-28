using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ExamplePlugin.Commands {
    struct SignOutRequest: IIntoJson {
        public string player;

        public string IntoJson() {
            return JsonUtility.ToJson(this);
        }
    }

    // TODO?
    struct SignOutResponse {
    }

    internal class SignOutCommand : Command<SignOutRequest> {
        public SignOutCommand(State state) : base(state) {
        }

        public override string Name => "sign_out";
        public override string HelpText => "Sign out of the website. Requires 0 arguments: sign_out";

        protected override void ExecuteImpl(MonoBehaviour host, SignOutRequest payload) {
            var req = Api.PostRequest(
                endpoint: "/post", // TODO replace with actual endpoint 
                payload: payload, 
                onError: Api.LogError, 
                onDone: (object res, UnityWebRequest req) => {
                    state.SignOut();
                    Log.LogInfo($"Received {req.downloadHandler.text}");
                    Debug.Log("Signed out");
                }
            );
            Api.Send(host, req);
        }

        protected override IEnumerable<string>? TryParseArgs(ConCommandArgs _, out SignOutRequest parsedArgs) {
            parsedArgs = new SignOutRequest {
                player = state.PlayerName
            };
            if (!state.IsPlayedSignedIn) {
                return new string[] {
                    "No player to sign out, please use the sign_up command to join the race first."
                };
            }
            return null;
        }
    }
}
