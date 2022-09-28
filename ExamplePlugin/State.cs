using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ExamplePlugin {
    // TODO could also use BepInEx's Config manager thing, but maybe that should
    //      only be used for actual configuration (like the server URL)
    internal class State {
        public static readonly State instance = new();

        public MonoBehaviour pluginInstance;
        public string playerName;

        //private State() {}

    }
}
