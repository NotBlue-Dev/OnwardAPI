using System.Text.Json.Serialization;

namespace OnwardAPIVRML.DTOS {
    public class VictimDTO {
        public string Name { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public Faction Team { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BodyParts BodyPart { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DamageType DamageType { get; set; }

    }
}
