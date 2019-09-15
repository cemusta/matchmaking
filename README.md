# Matchmaking Service

Basic player grouping (matchmaking) implementation using .net core 2.2 as web application and cli. Main data and business logic is located inside library project and this lib is used by cli and web app.

This sample uses EF and local Sqlite for simplicity but in theory it can easily run with other EF targets as well.

## CLI app

Cli app is for testing matchmaking algorithms via random feed of users. It creates 0-5 random player every 0.1 seconds and checks if a game is ready to started.

Just run dotnet restore and run inside `matchmaker.cli` folder:

```shell
dotnet restore
dotnet run
```

## Web app

Web app has a single controller for input output needs and starts a background task that implements similar matchmaking as cli tool.

In order to start web app just run dotnet restore and run inside `matchmaker.cli` folder:

```shell
dotnet restore
dotnet run
```

after running the project, there are multiple endpoints available:

- Show Queue: <https://localhost:5001/api/queue>
- Add Player: <https://localhost:5001/api/queue/addUser>
- Add Random Player: <https://localhost:5001/api/queue/Random>
- Show Matches: <https://localhost:5001/api/queue/Matches>
