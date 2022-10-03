using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ExamplePlugin.Commands;

struct ReverseVictoryRequest : IIntoJson {
    public string player;

    public string IntoJson() {
        return JsonUtility.ToJson(this);
    }
}

struct ReverseVictoryResponse {
}

internal class ReverseVictoryCommand 
    : ApiConCommand<ReverseVictoryRequest, ReverseVictoryResponse> 
{
    public override string Name => "reverse_victory";
    public override string HelpText => "Remove one victory from the tracker.";

    protected override Endpoint Endpoint => Endpoints.ReverseVictory;
    protected override HttpMethod Method => HttpMethod.POST;

    protected override void TryParseArgs(
        ConCommandArgs args, 
        ref ICollection<string> errors, 
        out ReverseVictoryRequest payload
    ) {
        if (!State.instance.IsSignedIn) {
            errors.Add("No player is signed in. Please use the 'sign_up' to join the race first.");
        }

        // TODO error when no race was joined
        // TODO error when no victories to remove

        payload = new ReverseVictoryRequest {
            player = State.instance.PlayerName
        };
    }

    protected override void OnError(
        ReverseVictoryRequest payload, 
        UnityWebRequest req
    ) {
        Log.LogFatal("TODO store reverse request locally, try to upload later");
        Api.LogError(req);
    }

    protected override void OnDone(
        ReverseVictoryRequest payload, 
        ReverseVictoryResponse res, 
        UnityWebRequest req
    ) {
        Debug.Log($"One victory removed for {payload.player}");
    }
}
