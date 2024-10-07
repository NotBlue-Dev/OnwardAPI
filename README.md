
# Onward Data API Mod

## Project Overview
This project was developed as a C# mod for the game Onward, designed to expose in-game data through an API that can be polled. It leverages BepInEx as the modloader, and the core logic was built using reverse engineering to understand how to access and retrieve game data.

Unfortunately, the project was cut short due to being banned by the game's anti-cheat system, despite only using the mod in spectator mode. As a result, I’ve decided to release the code to showcase examples of my works on external's API for VR games.

This is an example of what can be done with modding techniques and reverse engineering to retrieve live data about a unity game

## Features
API for Onward Data: Exposes various game-related data that can be accessed by external applications.
BepInEx Integration: Uses BepInEx as the modloader to inject the code into the game.
Reverse Engineering: Reverse-engineered certain game mechanics to expose data for external use.
Project Status
Discontinued: Development stopped due to an anti-cheat ban. The project was functional and only used in spectator mode.

## Data example
```json
{
   "Map":"SnowpeakDay",
   "GameStatus":"SpawnIn",
   "RoundTimer":"02:59",
   "VolkRoundWon":3,
   "MarsocRoundWon":2,
   "TotalRoundCount":5,
   "RoundWinner":"Volk",
   "Teams":[
      {
         "Team":"Marsoc",
         "Stats":{
            "Heals":1,
            "Objectives":0,
            "Deaths":11,
            "Kills":16
         },
         "Players":[
            {
               "Name":"HiddenForGit",
               "State":"NotSpawned",
               "Health":0,
               "Class":"Specialist",
               "Stats":{
                  "Heals":0,
                  "Objectives":0,
                  "Deaths":3,
                  "Kills":1
               }
            },
            "..."
         ]
      },
      {
         "Team":"Volk",
         "Stats":{
            "Heals":1,
            "Objectives":1,
            "Deaths":13,
            "Kills":11
         },
         "Players":[
            {
               "Name":"HiddenForGit",
               "State":"Dead",
               "Health":0,
               "Class":"Rifleman",
               "Stats":{
                  "Heals":0,
                  "Objectives":0,
                  "Deaths":1,
                  "Kills":0
               }
            },
            "..."
         ]
      }
   ],
   "LastKill":{
      "Killer":{
         "Name":"HiddenForGit",
         "Weapon":"AK5C",
         "Team":"Marsoc"
      },
      "Victim":{
         "Name":"HiddenForGit",
         "Team":"Volk",
         "BodyPart":"Head",
         "DamageType":"Weapon"
      }
   }
}
```

## Frameworks
BepInEx Modloader .NET plugin framework aimed at Unity games to make modding easier
HarmonyX to hook game functions during runtime

## Disclaimer
This project should not be used since it will get you banned.
This project is showcased as an educational purpose, showing off my works in creating external API for VR games
This project was conducted for my own use
