# AFL (Australian Football League) MCP Server

This is a Model Context Protocol (MCP) server that provides AFL (Australian Football League) data from Squiggle API. It allows you to retrieve information about past AFL games, current and past standings, and team information.

## Features

This MCP Server offers access to AFL data, providing tools for:

- Retrieving current AFL Standings.
- Retrieving past AFL Standings by round and year.
- Retrieving results from a particular game.
- Retreiving results from a round by year (E.g. All results for Round 1 in 2017).
- Retreiving basic information about a team.
- Retreiving a list of teams who played in a particular season.

## Squiggle API

This server uses the Squiggle API to retrieve AFL data. The API provides methods to fetch basic data about AFL games, including fixtures, ladders, match scores etc. All information about the API (including how to use it responsibly!) can be found [here](https://api.squiggle.com.au/).

## MCP Tools

The server exposes the following tools through the Model Context Protocol:

### Game Information

| **Tool** | **Description** |
|:--------:|:---------------:|
| `GetGameResult` | Gets result from a played game. |
| `GetRoundResultsByYear` | Get the results from a round of a particular year. |

### Standings Information

| **Tool** | **Description** |
|:--------:|:---------------:|
| `GetCurrentStandings` | Gets the current standings of the AFL. |
| `GetStandingsByRoundAndYear` | Get the standings for a particular round and year. |

### Team Information

| **Tool** | **Description** |
|:--------:|:---------------:|
| `GetTeamInfo` | Gets information for a AFL team. |
| `GetTeamsBySeason` | Gets a list of teams who played in a particular season. |

## Usage

**Integration with Claude for Desktop**

To integrate this server with Claude for Desktop:

1. Edit the Claude for Desktop config file, located at:

- macOS: `~/Library/Application Support/Claude/claude_desktop_config.json`
- Windows: `%APPDATA%\Claude\claude_desktop_config.json`

2. Add the server to your configuration:

```json
{
    "mcpServers": {
        "mcp-afl-server": {
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "PATH\\TO\\PROJECT",
                "--no-build"
            ]
        }
    }
}
```

3. Restart Claude for Desktop
