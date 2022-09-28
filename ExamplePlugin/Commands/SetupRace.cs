using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Globalization;
using UnityEngine.Networking;

namespace ExamplePlugin.Commands {
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

    internal class SetupRaceCommand : Command<SetupRaceRequest> {
        public SetupRaceCommand(State state) : base(state) {
        }

        public override string Name => "setup_race";
        public override string HelpText => (
            "Signals the website to setup a race and listen for the races to be " +
            "included. [local datetime (yyyy-MM-dd_hh:mi) for race start] [local " +
            "datetime (yyyy-MM-dd_hh:mi) for race end] [time in hours for each day, " +
            "or the total alloted time] [whether the race is a set time or an " +
            "alloted bank of time false/true]"
        );

        protected override void ExecuteImpl(MonoBehaviour host, SetupRaceRequest payload) {
            var req = Api.PostRequest(
                endpoint: "/post",
                payload,
                onError: Api.LogError,
                onDone: (object res, UnityWebRequest req) => {
                    state.race = new RaceSetup {
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
                    } else {
                        Debug.Log("with a set time");
                    }
                }
            );
            Api.Send(host, req);
        }

        protected override IEnumerable<string>? TryParseArgs(ConCommandArgs args, out SetupRaceRequest parsedArgs) {
            var errors = new List<string>();

            if (!state.IsPlayedSignedIn) {
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

            parsedArgs = new SetupRaceRequest {
                start = localStart,
                end = localEnd,
                timeInc = timeInc ?? 0,
                overall = overall ?? ""
            };

            if (errors.Count > 0) {
                return errors;
            }
            return null;
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
}
