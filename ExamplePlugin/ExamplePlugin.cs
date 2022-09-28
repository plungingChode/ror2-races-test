using BepInEx;
using RoR2;
using System;
using UnityEngine;
using ExamplePlugin.Commands;

namespace ExamplePlugin {
    //[BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    //[R2APISubmoduleDependency(nameof(ItemAPI), nameof(LanguageAPI))]

    public class ExamplePlugin : BaseUnityPlugin {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "RiskofResources";
        public const string PluginName = "Races";
        public const string PluginVersion = "1.0.0";

        public void Awake() {
            Log.Init(Logger);
            Log.LogWarning($"Races plugin is awake");

            // Attach hooks
            On.RoR2.NetworkUser.SetSurvivorPreferenceClient += NetworkUser_SetSurvivorPreferenceClient;
            On.RoR2.UserProfile.SetLoadout += UserProfile_SetLoadout;
            On.RoR2.Console.InitConVars += Console_InitConVars;
        }

        /// <summary>
        /// Initialize console commands. Works the same way as R2API's <c>ConCommand</c>
        /// helper attribute, but without any of the reflection.
        /// </summary>
        private static void Console_InitConVars(
            On.RoR2.Console.orig_InitConVars orig, 
            RoR2.Console self
        ) {
            orig(self);

            // List of commands to add
            var commands = new ICommand[] {
                new SignInCommand(state),
                new SignOutCommand(state),
                new SetupRaceCommand(state),
            };

            foreach (var cmd in commands) {
                var command = new RoR2.Console.ConCommand {
                    flags = ConVarFlags.None,
                    helpText = cmd.HelpText,
                    action = (args) => {
                        // Use console (self) instead of ExamplePlugin instance,
                        // because it actually exists
                        cmd.Execute(self, args);
                    }
                };
                var name = cmd.Name.ToLower();
                self.concommandCatalog[name] = command;
                Log.LogInfo($"Added console command '{name}'");
            }
        }

        private static void UserProfile_SetLoadout(
            On.RoR2.UserProfile.orig_SetLoadout orig, 
            UserProfile self, 
            Loadout newLoadout
        ) {
            // TODO
            // Set all skills to variant 0
            for (int i = 0; i < 99; i++) {
                try {
                    newLoadout
                        .bodyLoadoutManager
                        .SetSkillVariant(forced.BodyIndex, i, 0);
                } catch (Exception) {
                    // Doesn't matter
                }
            }
            orig(self, newLoadout);
        }

        private static void NetworkUser_SetSurvivorPreferenceClient(
            On.RoR2.NetworkUser.orig_SetSurvivorPreferenceClient orig, 
            NetworkUser self, 
            SurvivorDef survivorDef
        ) {
            // TODO throws a bunch of GetRequiredExpansion / GetRequiredEntitlement
            //      null reference errors, but it works (though it doesn't actually
            //      display the selected character)
            Log.LogWarning($"Forcing survivor: {forced.Name}");
            orig(self, forced.Load());
        }

        static readonly State state = new();
        static Survivor forced = SurvivorTable.Captain;
    }
}
