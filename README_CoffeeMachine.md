# How to Run

## One-Click Run

Double-click:

run-coffee.cmd

This will:
- Start the API
- Run unit tests
- Open browser at http://localhost:5298/swagger/index.html

---

##  Manual Run

### Run API
cd CoffeeMachine.API
dotnet restore
dotnet run

### Run Tests
cd CoffeeMachine.API
dotnet test

Then open:
http://localhost:5298/swagger/index.html

---

Make sure .NET SDK is installed.