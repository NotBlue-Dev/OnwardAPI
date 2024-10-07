using System.Collections.Generic;

namespace OnwardAPIVRML.DTOS.Utils; 

public static class PlayerDTOUtils {
    
    /// <summary>
    /// Remplit un PlayerDTO avec les infos d'un joueur
    /// </summary>
    /// <param name="dto">DTO à modifier</param>
    /// <param name="player">Infos</param>
    public static void SetInfos(this PlayerDTO dto, WarPlayerScript player) {
        dto.Name = player.OwningPlayer.NickName;
        dto.State = player.currentState;
        dto.Health = player.CurrentHealth;
        dto.Class = player.MySoldierClass;
        dto.Armor = player.CurrentArmorHealth;
        dto.Stats ??= new StatsDTO();
        dto.Equipments = new List<string>();
        dto.Stats.Kills = int.Parse(player.OwningPlayer.PlayerBox.PlayerKills.text);
        dto.Stats.Deaths = int.Parse(player.OwningPlayer.PlayerBox.PlayerDeaths.text);
        dto.Stats.Objectives = int.Parse(player.OwningPlayer.PlayerBox.PlayerObjectives.text);
        dto.Stats.Heals = int.Parse(player.OwningPlayer.PlayerBox.PlayerHeals.text);
    }
}