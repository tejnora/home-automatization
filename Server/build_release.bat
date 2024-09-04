rm ./bin/Release/ -R
dotnet publish -c Release --sc true -r linux-arm64 -p:PublishTrimmed=false

if errorlevel 1 (
   echo Compilation error %errorlevel%
   pause
   exit /b %errorlevel%
)
pause