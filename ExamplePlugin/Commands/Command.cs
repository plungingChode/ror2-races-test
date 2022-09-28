using RoR2;
using UnityEngine;

namespace ExamplePlugin.Commands {
    internal abstract class Command {
        public Command(State state) {
            _state = state;
        }

        public abstract string Name { get; }
        public abstract string HelpText { get; }

        public abstract void Execute(MonoBehaviour host, ConCommandArgs args);

        protected State _state;
    }
}
