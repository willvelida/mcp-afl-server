# 🏈 AFL (Australian Football League) MCP Server

This is a Model Context Protocol (MCP) server that provides AFL (Australian Football League) data from the Squiggle API. It allows you to retrieve information about past AFL games, current and past standings, team information, power rankings, tips, and projections.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](LICENSE)
[![MCP](https://img.shields.io/badge/MCP-Compatible-green.svg?style=flat-square)](https://modelcontextprotocol.io)

## ✨ Features

This MCP Server offers comprehensive access to AFL data, providing tools for:

- 🏆 Retrieving current AFL standings
- 📊 Retrieving past AFL standings by round and year
- 🎯 Retrieving results from particular games or rounds
- 👥 Retrieving basic information about teams
- 📅 Retrieving lists of teams who played in particular seasons
- 🔮 Getting tips and predictions for games
- 📈 Accessing power rankings and projected ladders
- 🔍 Managing data sources and configurations

## 📋 Prerequisites

- [.NET 9.0](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- A Model Context Protocol (MCP) compatible client (e.g., Claude for Desktop)
- Internet connection for accessing the Squiggle API

## ⚡ Quick Install

### 🎯 Claude Desktop Integration

1. Edit the Claude for Desktop config file:
   - **macOS**: `~/Library/Application Support/Claude/claude_desktop_config.json`
   - **Windows**: `%APPDATA%\Claude\claude_desktop_config.json`

2. Add the server configuration:

```json
{
    "mcpServers": {
        "mcp-afl-server": {
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "C:\\path\\to\\mcp-afl-server\\src",
                "--no-build"
            ]
        }
    }
}
```

## 🚀 What can you do with the AFL MCP Server?

Here are some example prompts you can try once connected:

### 🏆 Current Season Information

- "What are the current AFL standings?"
- "Show me the results from Round 10, 2024"
- "Get information about the Richmond Tigers"
- "List all teams that played in the 2024 season"

### 📊 Historical Data

- "What were the standings after Round 15 in 2023?"
- "Show me all results from Round 1, 2022"
- "Get the result of a specific game"

### 🔮 Tips & Predictions

- "What are the current tips for upcoming games?"
- "Show me tips for Round 5, 2024"
- "Get tips for a specific game"

### 📈 Advanced Analytics

- "Show me the power rankings for Round 20, 2023"
- "Get the projected ladder for Round 15, 2024"
- "List all available data sources"

## 📡 Squiggle API

This server uses the Squiggle API to retrieve AFL data. The API provides comprehensive methods to fetch:

- 🎯 Live scores and fixtures
- 📊 Historical match results
- 👥 Team information and statistics
- 🏆 Ladder positions and standings
- 🔮 Tips and predictions
- 📈 Power rankings and projections

Please use the API responsibly! All information about proper usage can be found at [api.squiggle.com.au](api.squiggle.com.au).

## 🛠️ MCP Tools

The server exposes the following tools through the Model Context Protocol:

### 🎯 Game Information

| **Tool** | **Description** |
|:--------:|:---------------:|
| `GetGameResult` | Gets result from a played game |
| `GetRoundResultsByYear` | Get the results from a round of a particular year |

### 🏆 Standings Information

| **Tool** | **Description** |
|:--------:|:---------------:|
| `GetCurrentStandings` | Gets the current standings of the AFL |
| `GetStandingsByRoundAndYear` | Get the standings for a particular round and year |

### 👥 Team Information

| **Tool** | **Description** |
|:--------:|:---------------:|
| `GetTeamInfo` | Gets information for an AFL team |
| `GetTeamsBySeason` | Gets a list of teams who played in a particular season |

### 🔮 Tips & Predictions

| **Tool** | **Description** |
|:--------:|:---------------:|
| `GetFutureTips` | Get the tips for current and future games |
| `GetTipsByGame` | Get the tips of a particular game |
| `GetTipsByRoundAndYear` | Get the tips for a particular round and year |

### 📈 Power Rankings & Projections

| **Tool** | **Description** |
|:--------:|:---------------:|
| `GetPowerRankingByRoundAndYear` | Get Power Ranking by Round and Year |
| `GetPowerRankingByRoundYearAndSource` | Get Power Ranking by Round, Year, and Model Source |
| `GetTeamPowerRankingByRoundAndYear` | Get Power Ranking for Team by Round, Year, and Model Source |
| `GetProjectedLadderByRoundAndYear` | Get the projected ladder for a particular round and year |
| `GetProjectedLadderByRoundAndYearBySource` | Get the projected ladder by source for a particular round and year |

### 🔍 Sources & Configuration

| **Tool** | **Description** |
|:--------:|:---------------:|
| `GetSources` | Gets a list of sources |
| `GetSourceById` | Gets a source |

## 📜 License

This project is licensed under the [MIT License](LICENSE) - see the LICENSE file for details.

## 💬 Feedback

We're building this in the open and your feedback is much appreciated! 

🐛 [Report a bug or request a feature](https://github.com/willvelida/mcp-afl-server/issues/new)

Your input helps shape the future of the AFL MCP Server and makes it better for everyone in the AFL community.

## 🏈 About AFL

The Australian Football League (AFL) is the premier professional Australian rules football competition in Australia. This server provides programmatic access to comprehensive AFL data to enhance your analysis, applications, and understanding of the game.

---

**Built with ❤️ for the AFL community**