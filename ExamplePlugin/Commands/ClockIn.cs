using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ExamplePlugin.Commands;

struct ClockInRequest : IIntoJson {
    public string player;

    public string IntoJson() {
        return JsonUtility.ToJson(this);
    }
}

struct ClockInResponse {
}

internal class ClockInCommand : ApiConCommand<ClockInRequest, ClockInResponse> {
    public override string Name => "clock_in";
    public override string HelpText => "Clock in to start your time bank counting down.";

    protected override Endpoint Endpoint => Endpoints.ClockIn;
    protected override HttpMethod Method => HttpMethod.POST;

    protected override void TryParseArgs(
        ConCommandArgs args, 
        ref ICollection<string> errors, 
        out ClockInRequest payload
    ) {
        if (!State.instance.IsSignedIn) {
            errors.Add("No player to clock in. Please use the 'sign_up' command first.");
        }
        // TODO add error if there's no ongoing race
        payload = new ClockInRequest {
            player = State.instance.PlayerName
        };
    }

    protected override void OnDone(
        ClockInRequest payload, 
        ClockInResponse res, 
        UnityWebRequest req
    ) {
        Debug.Log("Clocked in");
    }
}
