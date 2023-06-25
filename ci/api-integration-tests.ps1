docker network create planificateur-integration-tests-net

docker run `
    -d `
    -p 5432:5432 `
    -e POSTGRES_PASSWORD=password `
    -e POSTGRES_DB=planificateur  `
    --net planificateur-integration-tests-net `
    --name postgres `
    postgres

Start-Sleep -Seconds 5;

$pwd = pwd;

docker run `
    -v "${pwd}:/app" `
    -e DB_HOST=postgres `
    -e DB_PORT=5432 `
    -e DB_USERNAME=postgres `
    -e DB_PASSWORD=password `
    -e DB_NAME=planificateur `
    -e JWT_SECRET=this-is-a-secret `
    --net planificateur-integration-tests-net `
    --name planificateur-integration-tests-net `
    ombrelin/dotnet7-node dotnet test /app/test/Planificateur.Web.IntegrationTests/Planificateur.Web.IntegrationTests.csproj


$result = docker inspect planificateur-integration-tests-net --format='{{.State.ExitCode}}'

docker stop planificateur-integration-tests-net
docker rm planificateur-integration-tests-net

docker stop postgres
docker rm postgres

docker network rm planificateur-integration-tests-net

Exit $result