using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ExamplePlugin.Commands;

struct ClockOutRequest : IIntoJson {
    public string player;

    public string IntoJson() {
        return JsonUtility.ToJson(this);
    }
}

struct ClockOutResponse {
}

internal class ClockOutCommand : ApiConCommand<ClockOutRequest, ClockOutResponse> {
    public override string Name => "clock_out";
    public override string HelpText => "Clock out to stop your time bank counting down.";

    protected override Endpoint Endpoint => Endpoints.ClockOut;
    protected override HttpMethod Method => HttpMethod.POST;

    protected override void TryParseArgs(
        ConCommandArgs args,
        ref ICollection<string> errors,
        out ClockOutRequest payload
    ) {
        if (!State.instance.IsSignedIn) {
            errors.Add("No player to clock out. Please use the 'sign_up' command first.");
        }

        // TODO error if there's no race

        payload = new ClockOutRequest {
            player = State.instance.PlayerName
        };

    }
}
