using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OnwardAPIVRML.DTOS {
    public class TeamDTO {
        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public Faction Team { get; set; }
        public StatsDTO Stats { get; set; }
        public List<PlayerDTO> Players { get; set; }
    }
}
