using System.Collections.Generic;
using System.Text.Json.Serialization;
using Onward;

namespace OnwardAPIVRML.DTOS {
    public class PlayerDTO {
        public string Name { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PlayerState State { get; set; }
        
        public float Armor { get; set; }
        
        public List<string> Equipments { get; set; }
        public float Health { get; set; } = 100.0f;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SoldierClass Class { get; set; }
        public StatsDTO Stats { get; set; } = new StatsDTO();
    }
}
