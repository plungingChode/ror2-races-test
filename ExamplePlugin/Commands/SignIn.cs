using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ExamplePlugin.Commands {
    struct SignInRequest : IIntoJson {
        public string? player;

        public string IntoJson() {
            return JsonUtility.ToJson(this);
        }
    }

    struct SignInResponse {
        public string player;
    }

    class SignInCommand : Command<SignInRequest> {
        public SignInCommand(State state) : base(state) {
        }

        public override string Name => "sign_up";
        public override string HelpText => "Sign into the website by entering your Twitch Username.";

        protected override IEnumerable<string>? TryParseArgs(
            ConCommandArgs args, 
            out SignInRequest parsed
        ) {
            parsed = new SignInRequest {
                player = args.GetArgString(0)
            };
            if (parsed.player == null) {
                return new string[] {
                    "No Twitch username has been specified. Please try again."
                };
            }
            return null;
        }

        protected override void ExecuteImpl(MonoBehaviour host, SignInRequest payload) {
            var req = Api.PostRequest(
                "/post", 
                payload, 
                Api.LogError, 
                (SignInResponse o, UnityWebRequest _) => {
                    // TODO update state (from response?)
                    state.SignIn(payload.player!);
                    Debug.Log(JsonUtility.ToJson(o));
                }
            );
            Api.Send(host, req);
        }
    }
}
