using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using OnwardAPIVRML.DTOS;
using OnwardAPIVRML.Networking;
using OnwardAPIVRML.Patches;
using HarmonyLib;

namespace OnwardAPIVRML;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static RootDTO RootDTO;
    internal new static ManualLogSource Log;
    private readonly Server server;
    
    public Plugin() {
        server = new Server();
        RootDTO = new RootDTO {
            Teams = new List<TeamDTO>() {
                new TeamDTO() {
                    Team = Faction.Marsoc, 
                    Stats = new StatsDTO(),
                    Players = new List<PlayerDTO>(),
                },
                new TeamDTO() {
                    Team = Faction.Volk,
                    Stats = new StatsDTO(),
                    Players = new List<PlayerDTO>()
                }
            },
            LastKill = new LastKillDTO() {
                Killer = new KillerDTO(),
                Victim = new VictimDTO()
            },
        };
    }
    
    public override void Load()
    {
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded successfully!");

        server.CreateAndStartServer();

        Harmony.CreateAndPatchAll(typeof(GamePatch));
        Harmony.CreateAndPatchAll(typeof(PlayerPatch));

        // Affichage de toutes les méthodes qu'on a patché
        foreach (MethodBase method in Harmony.GetAllPatchedMethods()) {
            Log.LogInfo($"Hook: {method.Name}");
        }
    }
    
    public override bool Unload() {
        server.CloseServer();
        return false;
    }
}