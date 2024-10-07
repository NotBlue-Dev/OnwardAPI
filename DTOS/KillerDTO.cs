using System.Text.Json.Serialization;
using OnwardAPIVRML.DTOS.Utils;


namespace OnwardAPIVRML.DTOS {
    public class KillerDTO {
        public string Name { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public WeaponName Weapon { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public Faction Team { get; set; }
    }
}
