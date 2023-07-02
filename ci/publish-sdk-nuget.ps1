cd ../src/Planificateur.ClientSdk
dotnet pack
dotnet nuget push bin/Debug/Planificateur.ClientSdk.1.0.0.nupkg --api-key $Env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json