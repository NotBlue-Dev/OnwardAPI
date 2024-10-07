using System.Linq;

namespace OnwardAPIVRML.DTOS.Utils;

public static class TeamDTOUtils
{
    public static void SetTeamStats(this TeamDTO dto)
    {
        dto.Stats.Kills = dto.Players.Sum(player => player.Stats.Kills);
        dto.Stats.Deaths = dto.Players.Sum(player => player.Stats.Deaths);
        dto.Stats.Heals = dto.Players.Sum(player => player.Stats.Heals);
        dto.Stats.Objectives = dto.Players.Sum(player => player.Stats.Objectives);
    }
}