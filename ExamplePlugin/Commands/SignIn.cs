using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ExamplePlugin.Commands;

struct SignInRequest : IIntoJson {
    public string? player;

    public string IntoJson() {
        return JsonUtility.ToJson(this);
    }
}

struct SignInResponse {
}

class SignInCommand : ApiConCommand<SignInRequest, SignInResponse> {
    public override string Name => "sign_up";
    public override string HelpText => "Sign into the website by entering your Twitch Username.";

    protected override Endpoint Endpoint => Endpoints.SignIn;
    protected override HttpMethod Method => HttpMethod.POST;

    protected override void TryParseArgs(
        ConCommandArgs args, 
        ref ICollection<string> errors,
        out SignInRequest payload
    ) {
        payload = new SignInRequest {
            player = args.GetArgString(0)
        };
        if (payload.player == null) {
            errors.Add("No Twitch username has been specified. Please try again.");
        }
    }

    protected override void OnDone(
        SignInRequest payload, 
        SignInResponse res, 
        UnityWebRequest req
    ) {
        State.instance.SignIn(payload.player!);
        Debug.Log("Signed in");
    }
}
