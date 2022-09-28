using RoR2;
using System;
using UnityEngine;

namespace ExamplePlugin.Commands {
    class SignInCommand : Command {
        public SignInCommand(State state) : base(state) {
        }

        public override string HelpText {
            get => "Sign into the website by entering your Twitch Username.";
        }

        public override string Name {
            get => "sign_up";
        }

        public override void Execute(MonoBehaviour host, ConCommandArgs args) {
            var req = Api.PostRequest<IIntoJson, object>("/post", null, null, (o) => {
                Debug.Log(JsonUtility.ToJson(o));
            });
            //Api.Send(host, req);
            Log.LogWarning($"ExamplePlugin instance in external unit: {ExamplePlugin.instance}");
        }
    }
}
