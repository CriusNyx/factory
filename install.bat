(
  cd factory
  dotnet build --configuration Release
  "%USERPROFILE%/bin/factory"
  xcopy "./bin/Release/net8.0" "%USERPROFILE%/bin/factory" /Y /S
)