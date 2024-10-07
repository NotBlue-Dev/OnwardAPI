using Il2CppSystem.Collections.Generic;
using Onward.Networking;
using OnwardAPIVRML.DTOS.Utils;
using OnwardAPIVRML.DTOS;

namespace OnwardAPIVRML.Utils; 

public static class Extensions {
    public static bool HasValue(this string str) {
        return !string.IsNullOrEmpty(str);
    }
    
    public static bool HasValue(this object obj) {
        return obj != null;
    }
    
    public static System.Collections.Generic.List<PlayerDTO> ResetAndRefill(List<OnwardPhotonPlayer> players, TeamDTO team)
    {
        var tempDTO = new System.Collections.Generic.List<PlayerDTO>();
        foreach (OnwardPhotonPlayer player in players)
        {
            if (team.HasValue() && player.Team.State.Faction == team.Team)
            {
                PlayerDTO newPlayer = new PlayerDTO();
                newPlayer.Name = player.NickName;
                tempDTO.Add(newPlayer);
            }
        }

        return tempDTO;
    }
     
}