using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ExamplePlugin.Commands;

struct SignOutRequest: IIntoJson {
    public string player;

    public string IntoJson() {
        return JsonUtility.ToJson(this);
    }
}

// TODO?
struct SignOutResponse {
}

internal class SignOutCommand : ApiConCommand<SignOutRequest, SignOutResponse> {
    public override string Name => "sign_out";
    public override string HelpText => "Sign out of the website. Requires 0 arguments: sign_out";

    protected override Endpoint Endpoint => Endpoints.SignOut;
    protected override HttpMethod Method => HttpMethod.POST;

    protected override void TryParseArgs(
        ConCommandArgs _,
        ref ICollection<string> errors,
        out SignOutRequest parsedArgs
    ) {
        parsedArgs = new SignOutRequest {
            player = State.instance.PlayerName
        };
        if (!State.instance.IsSignedIn) {
            errors.Add(
                "No player to sign out. Please use the sign_up command to join the race first."
            );
        }
    }

    protected override void OnDone(SignOutRequest payload, SignOutResponse res, UnityWebRequest req) {
        State.instance.SignOut();
        Log.LogInfo($"Received {req.downloadHandler.text}");
        Debug.Log("Signed out");
    }
}
