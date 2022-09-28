using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ExamplePlugin.Commands {
    /// <summary>
    /// Cosole command definition interface.
    /// </summary>
    interface ICommand {
        /// <summary>
        /// The command to be input by the user, e.g.: <c>sign_in</c>.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Help text to display (TODO: where?)
        /// </summary>
        string HelpText { get; }

        /// <summary>
        /// A function called by the game's runtime, performs the designated
        /// command line action.
        /// </summary>
        /// <param name="host">An unspecified <c>MonoBehaviour</c> object (for web requests)</param>
        /// <param name="args">The command line arguments.</param>
        void Execute(MonoBehaviour host, ConCommandArgs args);
    }

    /// <summary>
    /// Abstract base class for a console command.
    /// </summary>
    /// <typeparam name="A">
    /// A type raw command args should be parsed into and passed to the command
    /// implementation
    /// </typeparam>
    internal abstract class Command<A>: ICommand {
        /// <summary>
        /// Initialize this command definition with the shared application state.
        /// </summary>
        /// <param name="state">The shared application state.</param>
        public Command(State state) {
            this.state = state;
        }

        public abstract string Name { get; }
        public abstract string HelpText { get; }
        public void Execute(MonoBehaviour host, ConCommandArgs args) {
            var errors = TryParseArgs(args, out A parsedArgs);

            if (errors != null) {
                foreach (var err in errors) {
                    Debug.LogError(err);
                }
                return;
            }

            ExecuteImpl(host, parsedArgs);
        }

        /// <summary>
        /// Parse the raw command line arguments and return either an iterator of
        /// errors or the successfully parsed arguments.
        /// </summary>
        /// <param name="args">The raw command line args.</param>
        /// <param name="parsedArgs">The sucessfully parsed args.</param>
        /// <returns>Errors encountered during parsing</returns>
        protected abstract IEnumerable<string>? TryParseArgs(ConCommandArgs args, out A parsedArgs);

        /// <summary>
        /// The implementation of the console command.
        /// </summary>
        /// <param name="host">A <c>MonoBehaviour</c> object (for web requests).</param>
        /// <param name="args">The parsed command line arguments.</param>
        protected abstract void ExecuteImpl(MonoBehaviour host, A args);

        /// <summary>
        /// The shared plugin state.
        /// </summary>
        protected State state;
    }
}
