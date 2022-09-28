using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ExamplePlugin {
    // TODO could also use BepInEx's Config manager thing, but maybe that should
    //      only be used for actual configuration (like the server URL)
    internal class State {
        private string? _playerName;
        public string PlayerName => _playerName ?? "";
        public bool IsPlayedSignedIn => _playerName != null;

        public RaceSetup? race;

        public void SignIn(string playerName) {
            _playerName = playerName;
        }

        public void SignOut() {
            _playerName = null;
        }
    }

    struct RaceSetup {
        public DateTime localStart;
        public DateTime localEnd;
        public long timeInc;
        public string overall;
    }
}
