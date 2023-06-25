../scripts/set-env.ps1
cd ../src/Planificateur.Web
dotnet tool run swagger tofile --output swagger.json bin/Debug/net7.0/Planificateur.Web.dll v1

cd ../Planificateur.ClientSdk
kiota generate -l CSharp -c PlanificateurClient -n Planificateur.ClientSdk -d ../Planificateur.Web/swagger.json -o ./ClientSdk
dotnet pack
dotnet nuget push bin/Debug/Planificateur.ClientSdk.1.0.0.nupkg --api-key $Env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json