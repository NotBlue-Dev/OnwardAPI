using System;
using System.Collections.Generic;
using System.Linq;
using DPI.Utilities;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Onward.GameStates;
using Onward.Spectating.Desktop;


namespace OnwardAPIVRML.Patches; 

public class GamePatch {
    // //////////
    // State  //
    // /////////
    [HarmonyPatch(typeof(GameState), nameof(GameState.Enter))]
    [HarmonyPostfix]
    public static void ChangeState(GameState __instance, Il2CppReferenceArray<Il2CppSystem.Object> data) {
        try
        {
            Plugin.RootDTO.GameStatus = __instance.StateType;
        } catch (Exception ex) {
            Plugin.Log.LogError($"GameStateManager@ChangeState");
            Plugin.Log.LogError($"Error while getting data: {ex}");
        }
    }
    
    // //////////
    // Timer   //
    // /////////
    [HarmonyPatch(typeof(SpectateDesktopInstance), nameof(SpectateDesktopInstance.OnTimeTextChanged))]
    [HarmonyPostfix]
    public static void OnTimeTextChanged(DPIStringBuilder sb) {
        try
        {
            Plugin.RootDTO.RoundTimer = sb.ToString();
        } catch (Exception ex) {
            Plugin.Log.LogError($"SpectateDesktopInstance@OnTimeTextChanged");
            Plugin.Log.LogError($"Error while getting data: {ex}");
        }
    }
    
    
    // //////////
    // Map    //
    // /////////
    [HarmonyPatch(typeof(GlobalScripts), nameof(GlobalScripts.OnMapLoaded))]
    [HarmonyPostfix]
    public static void OnMapLoaded(OnwardMap map) {
        try
        {
            Plugin.RootDTO.Map = map;
        } catch (Exception ex) {
            Plugin.Log.LogError($"GlobalScripts@OnMapLoaded");
            Plugin.Log.LogError($"Error while getting data: {ex}");
        }
    }
    
    
}