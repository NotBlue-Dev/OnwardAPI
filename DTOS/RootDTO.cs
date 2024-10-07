using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Onward.GameStates;

namespace OnwardAPIVRML.DTOS {
    public class RootDTO {
        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public OnwardMap Map { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public GameStateTypes GameStatus { get; set; }
        public string RoundTimer { get; set; }
        public int VolkRoundWon { get; set; }
        public int MarsocRoundWon { get; set; }
        public int TotalRoundCount { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Faction RoundWinner { get; set; }
        public List<TeamDTO> Teams { get; set; }
        public LastKillDTO LastKill { get; set; }
    }
}
