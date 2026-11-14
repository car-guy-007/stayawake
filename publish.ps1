$ErrorActionPreference = "Stop"
dotnet publish .\StayAwake\StayAwake.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o .\artifacts\win-x64
Write-Host "Published: .\artifacts\win-x64\StayAwake.exe"
