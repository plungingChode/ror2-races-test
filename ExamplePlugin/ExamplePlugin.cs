using BepInEx;
using RoR2;
using System;
using UnityEngine;
using ExamplePlugin.Commands;

namespace ExamplePlugin;

//[BepInDependency(R2API.R2API.PluginGUID)]
//[R2APISubmoduleDependency(nameof(ItemAPI), nameof(LanguageAPI))]

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public class ExamplePlugin : BaseUnityPlugin {
    public const string PluginGUID = PluginAuthor + "." + PluginName;
    public const string PluginAuthor = "RiskOfResources";
    public const string PluginName = "Races";
    public const string PluginVersion = "1.0.0";

    public void Awake() {
        Log.Init(Logger);

        // Attach hooks
        On.RoR2.NetworkUser.SetSurvivorPreferenceClient += NetworkUser_SetSurvivorPreferenceClient;
        On.RoR2.UserProfile.SetLoadout += UserProfile_SetLoadout;
        On.RoR2.Console.InitConVars += Console_InitConVars;

        // Only triggered at the end of runs
        //On.RoR2.Run.EndStage += Run_EndStage;
        //On.RoR2.Run.BeginGameOver += Run_BeginGameOver;

        // Never called?
        //On.RoR2.Stage.BeginAdvanceStage += Stage_BeginAdvanceStage;
        // Adding the hook here causes the 
        //On.RoR2.SceneExitController.Begin += SceneExitController_Begin;

        // Works as expected, called in this order
        //On.RoR2.Run.BeginStage += Run_BeginStage;
        //On.RoR2.Run.OnServerSceneChanged += Run_OnServerSceneChanged;
    }

    private static Survivor? forcedSurvivor = SurvivorTable.Captain;
    private static Loadout? forcedLoadout = new(new uint[] { 0, 0, 0, 0 });

    // List of commands to add
    private static readonly IConCommand[] commands = new IConCommand[] {
        new SignInCommand(),
        new SignOutCommand(),
        new SetupRaceCommand(),
        new ReverseVictoryCommand(),
    };

    /// <summary>
    /// Initialize console commands. Works the same way as R2API's <c>ConCommand</c>
    /// helper attribute, but without any of the reflection.
    /// </summary>
    private static void Console_InitConVars(
        On.RoR2.Console.orig_InitConVars orig, 
        RoR2.Console self
    ) {
        orig(self);

        // force_survivor [off | <survivor name>]
        self.concommandCatalog["force_survivor"] = new RoR2.Console.ConCommand {
            flags = ConVarFlags.None,
            helpText = "",
            action = (args) => {
                var a = args.GetArgString(0);
                if (a == null) {
                    return;
                }
                switch (a) {
                    case "off": forcedSurvivor = null; break;
                    case "commando": forcedSurvivor = SurvivorTable.Commando; break;
                    case "huntress": forcedSurvivor = SurvivorTable.Huntress; break;
                }
                // TODO find current user and set survivor/loadout preference
            }
        };

        foreach (var cmd in commands) {
            var command = new RoR2.Console.ConCommand {
                flags = ConVarFlags.None,
                helpText = cmd.HelpText,
                action = cmd.Execute
            };
            var name = cmd.Name.ToLower();
            self.concommandCatalog[name] = command;
            Log.LogInfo($"Added console command '{name}'");
        }
    }

   
    private static void UserProfile_SetLoadout(
        On.RoR2.UserProfile.orig_SetLoadout orig, 
        UserProfile self, 
        RoR2.Loadout newLoadout
    ) {
        ForceLoadout(newLoadout);
        orig(self, newLoadout);
    }

    private static void NetworkUser_SetSurvivorPreferenceClient(
        On.RoR2.NetworkUser.orig_SetSurvivorPreferenceClient orig, 
        NetworkUser self, 
        SurvivorDef survivorDef
    ) {
        ForceSurvivor(survivorDef);
        ForceLoadout(self.localUser.userProfile.loadout);
        orig(self, survivorDef);
    }

    /// <summary>
    /// Overwrite the survivor ID of a <see cref="SurvivorDef"/>, causing it
    /// to load a different survivor.
    /// </summary>
    /// <param name="survivorDef"></param>
    private static void ForceSurvivor(SurvivorDef survivorDef) {
        // Apparently, survivors are all loaded and available on startup,
        // so it's just a matter of overriding the index.
        if (forcedSurvivor != null) {
            Log.LogWarning($"Forcing survivor: {forcedSurvivor.Name}");
            survivorDef.survivorIndex = forcedSurvivor.SurvivorIndex;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="loadout"></param>
    private static void ForceLoadout(RoR2.Loadout loadout) {
        if (forcedLoadout == null || forcedSurvivor == null) {
            return;
        }

        if (forcedLoadout.IsValidForSurvivor(forcedSurvivor)) {
            Log.LogWarning($"Forcing loadout:");
            var skillNames = forcedLoadout.Names(loadout, forcedSurvivor.BodyIndex);
            foreach (var name in skillNames) {
                Log.LogWarning($"  - {name}");
            }

            forcedLoadout.Force(loadout, forcedSurvivor.BodyIndex);
        }
    }

}
