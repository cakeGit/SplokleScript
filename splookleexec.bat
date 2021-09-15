@echo off
cd %~dp0
FOR /F %%i IN (debugmode.txt) DO (
  echo %%i
  IF %%i == on (
    dotnet build
    dotnet run -- %* debug
  ) ELSE (
    dotnet run -- %*
  )
)
pause