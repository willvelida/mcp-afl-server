using System.Text.Json;

namespace mcp_afl_server.UnitTests.Helpers
{
    public static class TestData
    {
        // Create methods that the tests are looking for
        public static string CreatePowerRankingsResponseJson()
        {
            return """
            {
                "power": [
                    {
                        "sourceid": 1,
                        "power": "1650.5",
                        "team": "Richmond",
                        "source": "Expert Tips",
                        "rank": 1,
                        "teamid": 1,
                        "dummy": 0,
                        "updated": "2024-03-15",
                        "year": 2024,
                        "round": 10
                    },
                    {
                        "sourceid": 1,
                        "power": "1620.3",
                        "team": "Geelong",
                        "source": "Expert Tips",
                        "rank": 2,
                        "teamid": 2,
                        "dummy": 0,
                        "updated": "2024-03-15",
                        "year": 2024,
                        "round": 10
                    }
                ]
            }
            """;
        }

        public static string CreateGameResponseJson(int gameId = 12345)
        {
            return $@"{{
                ""games"": [
                    {{
                        ""hteam"": ""Richmond"",
                        ""ateam"": ""Carlton"",
                        ""hscore"": 95,
                        ""hgoals"": 14,
                        ""hbehinds"": 11,
                        ""ascore"": 87,
                        ""agoals"": 13,
                        ""abehinds"": 4,
                        ""venue"": ""MCG"",
                        ""round"": 1,
                        ""roundname"": ""Round 1"",
                        ""date"": ""2024-03-15"",
                        ""winner"": ""Richmond"",
                        ""is_final"": 0,
                        ""is_grand_final"": 0
                    }}
                ]
            }}";
        }

        public static string CreateStandingsResponseJson()
    {
            return """
            {
                "standings": [
                    {
                        "rank": 1,
                        "behinds_for": 150,
                        "goals_for": 200,
                        "behinds_against": 120,
                        "goals_against": 180,
                        "pts": 32,
                        "wins": 8,
                        "draws": 0,
                        "losses": 2,
                        "played": 10,
                        "percentage": 125.5,
                        "for": 1350,
                        "against": 1200,
                        "name": "Richmond"
                    },
                    {
                        "rank": 2,
                        "behinds_for": 140,
                        "goals_for": 190,
                        "behinds_against": 130,
                        "goals_against": 190,
                        "pts": 28,
                        "wins": 7,
                        "draws": 0,
                        "losses": 3,
                        "played": 10,
                        "percentage": 115.2,
                        "for": 1280,
                        "against": 1270,
                        "name": "Geelong"
                    }
                ]
            }
            """;
        }

        public static string CreateLadderResponseJson()
        {
            return """
            {
                "ladder": [
                    {
                        "team": "Richmond",
                        "swarms": ["Expert", "AI"],
                        "wins": "8",
                        "teamid": 1,
                        "updated": "2024-03-15",
                        "dummy": 0,
                        "year": 2024,
                        "round": 10,
                        "sourceid": 1,
                        "mean_rank": "1.5",
                        "source": "Expert Tips",
                        "rank": 1
                    }
                ]
            }
            """;
        }

        public static string CreateTeamResponseJson(int teamId = 1)
        {
            return $@"{{
                ""teams"": [
                    {{
                        ""name"": ""Richmond"",
                        ""abbrev"": ""RIC"",
                        ""logo"": ""richmond_logo.png"",
                        ""id"": {teamId},
                        ""debut"": 1908,
                        ""retirement"": 0
                    }}
                ]
            }}";
        }

        public static string CreateTipsResponseJson()
        {
            return """
            {
                "tips": [
                    {
                        "correct": 1,
                        "round": 1,
                        "year": 2024,
                        "updated": "2024-03-15",
                        "bits": "tip_data",
                        "err": "",
                        "gameid": 123,
                        "source": "Expert Tips",
                        "tipteamid": 1,
                        "ateam": "Carlton",
                        "hteam": "Richmond",
                        "hmargin": "12",
                        "sourceid": 1,
                        "margin": "12",
                        "venue": "MCG",
                        "hteamid": 1,
                        "ateamid": 2,
                        "date": "2024-03-15",
                        "tip": "Richmond",
                        "hconfidence": "75.5",
                        "confidence": "75.5"
                    }
                ]
            }
            """;
        }

        public static string CreateSourcesResponseJson()
        {
            return """
            {
                "sources": [
                    {
                        "name": "Expert Tips",
                        "url": "https://experttips.com",
                        "int": 1,
                        "icon": "expert_icon.png"
                    }
                ]
            }
            """;
        }

        public static string CreateSourceResponseJson(string sourceId = "1")
        {
            return $@"{{
                ""sources"": [
                    {{
                        ""name"": ""Expert Tips"",
                        ""url"": ""https://experttips.com"",
                        ""int"": {sourceId},
                        ""icon"": ""expert_icon.png""
                    }}
                ]
            }}";
        }

        public static string CreateEmptyResponseJson(string propertyName)
        {
            return $@"{{
                ""{propertyName}"": []
            }}";
        }

        // Keep the original Get methods for backward compatibility
        public static string GetGameResponseJson()
        {
            return """
            {
                "hteam": "Richmond",
                "ateam": "Carlton",
                "hscore": 95,
                "hgoals": 14,
                "hbehinds": 11,
                "ascore": 87,
                "agoals": 13,
                "abehinds": 4,
                "venue": "MCG",
                "round": 1,
                "roundname": "Round 1",
                "date": "2024-03-15",
                "winner": "Richmond",
                "is_final": 0,
                "is_grand_final": 0
            }
            """;
        }

        public static string GetStandingsResponseJson()
        {
            return """
            [
                {
                    "rank": 1,
                    "behinds_for": 150,
                    "goals_for": 200,
                    "behinds_against": 120,
                    "goals_against": 180,
                    "pts": 32,
                    "wins": 8,
                    "draws": 0,
                    "losses": 2,
                    "played": 10,
                    "percentage": 125.5,
                    "for": 1350,
                    "against": 1200,
                    "name": "Richmond"
                }
            ]
            """;
        }

        public static string GetLadderResponseJson()
        {
            return """
            [
                {
                    "team": "Richmond",
                    "swarms": ["Expert", "AI"],
                    "wins": "8",
                    "teamid": 1,
                    "updated": "2024-03-15",
                    "dummy": 0,
                    "year": 2024,
                    "round": 10,
                    "sourceid": 1,
                    "mean_rank": "1.5",
                    "source": "Expert Tips",
                    "rank": 1
                }
            ]
            """;
        }

        public static string GetPowerRankingsResponseJson()
        {
            return """
            [
                {
                    "sourceid": 1,
                    "power": "1650.5",
                    "team": "Richmond",
                    "source": "Expert Tips",
                    "rank": 1,
                    "teamid": 1,
                    "dummy": 0,
                    "updated": "2024-03-15",
                    "year": 2024,
                    "round": 10
                }
            ]
            """;
        }

        public static string GetTeamResponseJson()
        {
            return """
            {
                "name": "Richmond",
                "abbrev": "RIC",
                "logo": "richmond_logo.png",
                "id": 1,
                "debut": 1908,
                "retirement": 0
            }
            """;
        }

        public static string GetTeamsResponseJson()
        {
            return """
            [
                {
                    "name": "Richmond",
                    "abbrev": "RIC",
                    "logo": "richmond_logo.png",
                    "id": 1,
                    "debut": 1908,
                    "retirement": 0
                }
            ]
            """;
        }

        public static string GetTipsResponseJson()
        {
            return """
            [
                {
                    "correct": 1,
                    "round": 1,
                    "year": 2024,
                    "updated": "2024-03-15",
                    "bits": "tip_data",
                    "err": "",
                    "gameid": 123,
                    "source": "Expert Tips",
                    "tipteamid": 1,
                    "ateam": "Carlton",
                    "hteam": "Richmond",
                    "hmargin": "12",
                    "sourceid": 1,
                    "margin": "12",
                    "venue": "MCG",
                    "hteamid": 1,
                    "ateamid": 2,
                    "date": "2024-03-15",
                    "tip": "Richmond",
                    "hconfidence": "75.5",
                    "confidence": "75.5"
                }
            ]
            """;
        }

        public static string GetSourcesResponseJson()
        {
            return """
            [
                {
                    "name": "Expert Tips",
                    "url": "https://experttips.com",
                    "int": 1,
                    "icon": "expert_icon.png"
                }
            ]
            """;
        }

        public static string GetSourceResponseJson()
        {
            return """
            {
                "name": "Expert Tips",
                "url": "https://experttips.com",
                "int": 1,
                "icon": "expert_icon.png"
            }
            """;
        }

        public static string GetRoundResultsResponseJson()
        {
            return """
            [
                {
                    "hteam": "Richmond",
                    "ateam": "Carlton",
                    "hscore": 95,
                    "hgoals": 14,
                    "hbehinds": 11,
                    "ascore": 87,
                    "agoals": 13,
                    "abehinds": 4,
                    "venue": "MCG",
                    "round": 1,
                    "roundname": "Round 1",
                    "date": "2024-03-15",
                    "winner": "Richmond",
                    "is_final": 0,
                    "is_grand_final": 0
                }
            ]
            """;
        }
    }
}