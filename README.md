# RazorOverAndUnder
with xunit tests

To run:

```
dotnet run --project OrderBotPage

```

To test:

```
dotnet test
```

To test with coverage:

```
dotnet test --collect:"XPlat Code Coverage"
```

Then to make a report:

```
dotnet tool install --global dotnet-reportgenerator-globaltool --version 4.8.13
~/.dotnet/tools/reportgenerator -reports:OrderBot.tests/TestResults/781f9e92-31a8-4a0e-8a4a-35c884c0e5fe/coverage.cobertura.xml -targetdir:out
```
