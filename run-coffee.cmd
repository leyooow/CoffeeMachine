@echo off
echo Starting CoffeeMachine API...

:: Start API
start cmd /k "cd CoffeeMachine.API/CoffeeMachine.API && dotnet restore && dotnet run"

:: Wait for API
timeout /t 5 > nul

:: Run Tests
echo.
echo Running Unit Tests...
start cmd /k "cd CoffeeMachine.API && dotnet test"

:: Open Browser
start start http://localhost:5298/swagger/index.html

echo.
echo ✅ API and Tests are running...
pause
