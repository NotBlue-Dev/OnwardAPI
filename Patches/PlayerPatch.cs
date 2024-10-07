using System;
using System.Collections.Generic;
using System.Linq;
using DPI.Networking;
using HarmonyLib;
using Onward.Networking;
using Onward.TeamManagement;
using OnwardAPIVRML.DTOS;
using OnwardAPIVRML.DTOS.Utils;
using OnwardAPIVRML.Utils;
using UnityEngine;


namespace OnwardAPIVRML.Patches; 

public class PlayerPatch {
    // ////////// // //
    // Init Players  //
    // ////////// // //
    [HarmonyPatch(typeof(TeamManager), nameof(TeamManager.InitializeTeamsFromRoom))]
    [HarmonyPostfix]
    public static void InitializeTeamsFromRoom(TeamManager __instance) {
        try
        {
            var teams = __instance.AllTeams;
            foreach (var team in teams) {
                var dtoTeam = Plugin.RootDTO.Teams.FirstOrDefault(t => t.Team == team.State.Faction);
                if (dtoTeam == null || !dtoTeam.HasValue()) {
                    Plugin.Log.LogError($"Team {team.State.Faction} not found or has no value.");
                    continue;
                }

                Plugin.Log.LogInfo($"Team {team.State.Faction} found");

                foreach (var player in team.Players) {
                    var playerDTO = dtoTeam.Players.FirstOrDefault(p => p.Name == player.NickName);
                    Plugin.Log.LogInfo($"Search for {player.NickName} is done for team {team.State.Faction}");

                    if (playerDTO == null || !playerDTO.HasValue()) {
                        Plugin.Log.LogInfo($"Player {player.NickName} not found in team {team.State.Faction}");

                        if (dtoTeam.Players.Count > 5) {
                            dtoTeam.Players = Extensions.ResetAndRefill(team.Players, dtoTeam);
                        } else {
                            playerDTO = new PlayerDTO { Name = player.NickName };
                            dtoTeam.Players.Add(playerDTO);
                        }

                        Plugin.Log.LogInfo($"Player {player.NickName} added to team {team.State.Faction}");
                    }
                }

                dtoTeam.SetTeamStats();
            }
        } catch (Exception ex) {
            Plugin.Log.LogError($"TeamManager@InitializePlayers");
            Plugin.Log.LogError($"Error while getting data: {ex}");
        }
    }

    // ////////// // /
    // Round score  //
    // ////////// // /
    [HarmonyPatch(typeof(TeamManager), nameof(TeamManager.OnPlayerPropertiesUpdate))]
    [HarmonyPostfix]
    public static void OnPlayerPropertiesUpdate(TeamManager __instance, OnwardPhotonPlayer player, DPIHashtable hashtable) {
        try
        {
            if (__instance.AllTeams == null) 
            {
                Plugin.Log.LogError("TeamManager is null");
                return;
            }

            var teamsCopy = __instance.AllTeams.ToArray(); // Create a copy of the AllTeams collection

            // Update team scores
            int MarsocRoundWon = 0;
            int VolkRoundWon = 0;

            foreach (var team in teamsCopy)
            {
                if (team.State.Faction == Faction.Marsoc)
                {
                    MarsocRoundWon += team.State.Score;
                    Plugin.RootDTO.RoundWinner = Faction.Marsoc;
                }
                else
                {
                    VolkRoundWon += team.State.Score;
                    Plugin.RootDTO.RoundWinner = Faction.Volk; 
                }
                Plugin.Log.LogInfo($"Team {team.State.Faction} has {team.State.Score} points");
            }

            foreach (var team in teamsCopy)
            {
                var teamDto = Plugin.RootDTO.Teams.FirstOrDefault(t => t.Team == team.State.Faction);
                if (teamDto == null || !teamDto.HasValue())
                {
                    Plugin.Log.LogError($"Team {team.State.Faction} not found or has no value.");
                    continue;
                }

                foreach (var playerData in team.Players)
                {
                    // Add missing player to the dto
                    var playerDto = teamDto.Players.FirstOrDefault(p => p.Name == playerData.NickName);
                    if (playerDto == null || !playerDto.HasValue())
                    {
                        playerDto = new PlayerDTO { Name = playerData.NickName };
                        teamDto.Players.Add(playerDto);
                    }
                }

                // Remove players from teamDto.Players that are not in team.Players
                List<PlayerDTO> playersToRemove = new List<PlayerDTO>();
                foreach (var playerDto in teamDto.Players)
                {
                    bool found = false;
                    foreach (var playerData in team.Players)
                    {
                        if (playerDto.Name == playerData.NickName)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        playersToRemove.Add(playerDto);
                    }
                }
                foreach (var playerToRemove in playersToRemove)
                {
                    teamDto.Players.Remove(playerToRemove);
                }
            }
            
            if (player == null || player.PlayerComponent == null)
            {
                Plugin.Log.LogError("Player or PlayerComponent is null");
                return;
            }
            
            var playerFaction = player.Team.State.Faction;
            var playerNickName = player.NickName;
            
            // Update player info
            var dtoTeam = Plugin.RootDTO.Teams.FirstOrDefault(t => t.Team == playerFaction);
            if (dtoTeam == null || !dtoTeam.HasValue())
            {
                Plugin.Log.LogError($"Team {playerFaction} not found or has no value.");
                return;
            }

            var playerDTO = dtoTeam.Players.FirstOrDefault(p => p.Name == playerNickName);
            if (playerDTO != null)
            {
                playerDTO.SetInfos(player.PlayerComponent);
            }
            else
            {
                var newPlayer = new PlayerDTO();
                dtoTeam.Players.Add(newPlayer);
                if (player.PlayerComponent != null)
                {
                    newPlayer.SetInfos(player.PlayerComponent);
                    Plugin.Log.LogInfo($"Player {player.PlayerComponent.OwningPlayer.NickName} added to team {player.PlayerComponent.PlayerFaction}");
                }
                else
                {
                    newPlayer.Name = player.NickName;
                    Plugin.Log.LogError($"PlayerComponent empty {player.NickName}");
                }
            }

            dtoTeam.SetTeamStats();

            // Remove player from wrong team
            var wrongTeam = Plugin.RootDTO.Teams.FirstOrDefault(t => t.Team != playerFaction);
            if (wrongTeam != null)
            {
                var playerDTO2 = wrongTeam.Players.FirstOrDefault(p => p.Name == playerNickName);
                if (playerDTO2 != null)
                {
                    wrongTeam.Players.Remove(playerDTO2);
                    Plugin.Log.LogInfo($"Player {playerNickName} removed from team {wrongTeam.Team}");
                }
            }

            Plugin.RootDTO.MarsocRoundWon = MarsocRoundWon;
            Plugin.RootDTO.VolkRoundWon = VolkRoundWon;
            Plugin.RootDTO.TotalRoundCount = MarsocRoundWon + VolkRoundWon;
        } catch (Exception ex) {
            Plugin.Log.LogError($"TeamManager@OnPlayerPropertiesUpdate");
            Plugin.Log.LogError($"Error while getting data: {ex}");
        }
    }
    
    
    // ////////// //
    // Set state  //
    // ////////// //
    [HarmonyPatch(typeof(WarPlayerScript), nameof(WarPlayerScript.SetState))]
    [HarmonyPostfix]
    public static void SetState(WarPlayerScript __instance, PlayerState newState) {
        try
        {
            var teamDto = Plugin.RootDTO.Teams.FirstOrDefault(t => t.Team == __instance.PlayerFaction);
            if (teamDto == null || !teamDto.HasValue())
            {
                Plugin.Log.LogError($"Team {__instance.PlayerFaction} not found or has no value.");
                return;
            }
            var playerDto = teamDto.Players.FirstOrDefault(p => p.Name == __instance.OwningPlayer.NickName);
            if (playerDto == null || !playerDto.HasValue())
            {
                Plugin.Log.LogError($"Player {__instance.OwningPlayer.NickName} not found or has no value.");
                return;
            }
            
            playerDto.State = newState;
            playerDto.Health = __instance.CurrentHealth;
            playerDto.Armor = __instance.CurrentArmorHealth;

            if(newState == PlayerState.Dead || newState == PlayerState.NotSpawned)
            {
                playerDto.Equipments.Clear();
            }
        } catch (Exception ex) {
            Plugin.Log.LogError($"WarPlayerScript@OnPlayerHit");
            Plugin.Log.LogError($"Error while getting data: {ex}");
        }
    }
    
    
    // ////////// // /
    // Player death //
    // ////////// // /
    [HarmonyPatch(typeof(WarPlayerScript), nameof(WarPlayerScript.OnPlayerHit))]
    [HarmonyPostfix]
    public static void PlayerHit(WarPlayerScript player,
        WarPlayerScript lastDamageGiver,
        bool killingHit,
        bool downingHit,
        bool infractableHit,
        BodyParts damagedPart,
        Vector3 hitPos,
        Vector3 shootPos,
        float damage,
        DamageType damageType,
        WeaponName weaponName = WeaponName.NULL
        ) {
        try
        {
            if (!killingHit) return;
            
            Plugin.RootDTO.LastKill.Killer.Name = lastDamageGiver.OwningPlayer.NickName;
            Plugin.RootDTO.LastKill.Victim.Name = player.OwningPlayer.NickName;
            Plugin.RootDTO.LastKill.Killer.Team = lastDamageGiver.PlayerFaction;
            Plugin.RootDTO.LastKill.Victim.Team = player.PlayerFaction;
            Plugin.RootDTO.LastKill.Victim.BodyPart = damagedPart;
            Plugin.RootDTO.LastKill.Victim.DamageType = damageType;
            Plugin.RootDTO.LastKill.Killer.Weapon = weaponName;
        } catch (Exception ex) {
            Plugin.Log.LogError($"WarPlayerScript@OnPlayerHit");
            Plugin.Log.LogError($"Error while getting data: {ex}");
        }
    }
    
    //find another function that update when player spawn not before and do the same shit with Pickup
    /**
        [HarmonyPatch(typeof(Pickup), nameof(Pickup.OnHolster))]
        [HarmonyPostfix]
        public static void OnHolster(Pickup __instance, PickupHolster holster) {
            try
            {
                var teamDto = Plugin.RootDTO.Teams.FirstOrDefault(t => t.Team == __instance.m_ownerWPS.PlayerFaction);
                if (teamDto == null || !teamDto.HasValue())
                {
                    Plugin.Log.LogError($"Team {__instance.m_ownerWPS.PlayerFaction} not found or has no value.");
                    return;
                }
                
                Plugin.Log.LogInfo($"Holster {__instance.m_ItemType} {__instance.m_ownerWPS == null}");
                
                var playerDto = teamDto.Players.FirstOrDefault(p => p.Name == __instance.m_ownerWPS.OwningPlayer.NickName);
                if (playerDto == null || !playerDto.HasValue())
                {
                    Plugin.Log.LogError($"Player {__instance.m_ownerWPS.OwningPlayer.NickName} not found or has no value.");
                    return;
                }
                
                if(__instance.m_ItemType == Pickup.ItemType.Weapon_Gun_Primary || __instance.m_ItemType == Pickup.ItemType.Weapon_Gun_Secondary) {
                    playerDto.Equipments.Add(__instance.WeaponName.ToString());
                }
                else if (__instance.m_ItemType == Pickup.ItemType.Grenade)
                {
                    playerDto.Equipments.Add(__instance.ToString());
                }
                else
                {
                    playerDto.Equipments.Add(__instance.m_ItemType.ToString());
                }
                
            } catch (Exception ex) {
                Plugin.Log.LogError($"WarPlayerScript@OnPlayerHit");
                Plugin.Log.LogError($"Error while getting data: {ex}");
            }
        }
     */
        
}