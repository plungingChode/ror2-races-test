using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Globalization;
using UnityEngine.Networking;

namespace ExamplePlugin.Commands;

struct SetupRaceRequest : IIntoJson {
    public DateTime start;
    public DateTime end;
    public long timeInc;
    public string overall;

    public string IntoJson() {
        var epoch = new DateTime(1970, 1, 1).ToUniversalTime();
        var utcStart = start.ToUniversalTime().Subtract(epoch).TotalMilliseconds;
        var utcEnd = start.ToUniversalTime().Subtract(epoch).TotalMilliseconds;

        return JsonUtility.ToJson(new {
            // UTC start time, unix timestamp
            startTime = utcStart,
            endTime = utcEnd,

            // TODO idk what these are
            timeInc = timeInc * 3_600_000,
            overall,
        });
    }
}

struct SetupRaceResponse {
}

internal class SetupRaceCommand 
    : ApiConCommand<SetupRaceRequest, SetupRaceResponse> 
{
    public override string Name => "setup_race";
    public override string HelpText => (
        "Signals the website to setup a race and listen for the races to be " +
        "included. [local datetime (yyyy-MM-dd_hh:mi) for race start] [local " +
        "datetime (yyyy-MM-dd_hh:mi) for race end] [time in hours for each day, " +
        "or the total alloted time] [whether the race is a set time or an " +
        "alloted bank of time false/true]"
    );

    protected override Endpoint Endpoint => Endpoints.SetupRace;
    protected override HttpMethod Method => HttpMethod.POST;

    protected override void TryParseArgs(
        ConCommandArgs args,
        ref ICollection<string> errors,
        out SetupRaceRequest payload
    ) {
        if (!State.instance.IsSignedIn) {
            errors.Add("Sign in before setting up a race");
        }
        if (!TryParseDate(args.TryGetArgString(0), out DateTime localStart)) {
            errors.Add("Could not read start datetime. Use 'yyyy-MM-dd_hh:mi' format.");
        }
        if (!TryParseDate(args.TryGetArgString(1), out DateTime localEnd)) {
            errors.Add("Could not read end datetime. Use 'yyyy-MM-dd_hh:mi' format.");
        }

        if (localEnd.CompareTo(localStart) < 0) {
            errors.Add("Race end cannot be earlier than race start.");
        }

        var timeInc = args.TryGetArgInt(2);
        if (timeInc == null) {
            errors.Add("Could not read 'timeInc' parameter as number.");
        }

        var overall = args.TryGetArgString(3);
        if (overall == null) {
            errors.Add("Coudl not read 'overall' parameter.");
        }

        payload = new SetupRaceRequest {
            start = localStart,
            end = localEnd,
            timeInc = timeInc ?? 0,
            overall = overall ?? ""
        };
    }

    protected override void OnDone(
        SetupRaceRequest payload, 
        SetupRaceResponse res, 
        UnityWebRequest req
    ) {
        State.instance.raceTiming = new RaceTiming {
            localStart = payload.start,
            localEnd = payload.end,
            timeInc = payload.timeInc,
            overall = payload.overall
        };

        var start = payload.start.ToString("yyyy-MM-dd HH:mm");
        var end = payload.end.ToString("yyyy-MM-dd HH:mm");

        Debug.Log("Successful race setup");
        Debug.Log($"Race scheduled between {start} and {end},");
        Debug.Log($"with up to {payload.timeInc} hours per day(?),");

        if (payload.overall != "true") {
            Debug.Log("using a time bank");
        }
        else {
            Debug.Log("with a set time");
        }
    }

    const string DATE_FORMAT = "yyyy-MM-dd_HH:mm";
    private static bool TryParseDate(string? s, out DateTime date) {
        if (s == null) {
            date = new DateTime();
            return false;
        }
        return DateTime.TryParseExact(
            s, 
            DATE_FORMAT, 
            CultureInfo.CurrentCulture, 
            DateTimeStyles.None, 
            out date
        );
    }
}
