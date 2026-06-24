@echo off
echo Starting CoffeeMachine API...

start cmd /k "cd CoffeeMachine.API/CoffeeMachine.API && dotnet restore && dotnet run"

timeout /t 5 > nul

start http://localhost:5298/swagger/index.html

echo.
echo ✅ API is running...
pause
