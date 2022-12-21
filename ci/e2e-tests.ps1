docker compose up -d

<#
docker run -d `
    -p 5432:5432 `
    --env POSTGRES_PASSWORD=password `
    --env POSTGRES_DB=planificateur `
    --name postgres `
    --net planificateur `
    postgres 

docker run -d `
    -p 5000:80 `
    --env DB_HOST=postgres `
    --env DB_NAME=planificateur-test `
    --env DB_PASSWORD=password `
    --env DB_PORT=5432 `
    --env DB_USERNAME=postgres `
    --name planificateur-e2e `
    --net planificateur `
    ombrelin/planificateur:latest
#>

cd ..
$pwd = pwd;

docker run `
    --env APP_URL="http://planificateur-e2e/" `
    --env IS_CI="true" `
    --net ci_default `
    --name planificateur-e2e-tests-exec `
    -v "${pwd}:/app" `
    -w /app `
    ombrelin/dotnet7-playwright `
    /bin/bash -c "dotnet build /app/test/Planificateur.Web.EndToEndTests/Planificateur.Web.EndToEndTests.csproj && ~/bin/playwright install-deps && ~/bin/playwright install firefox && dotnet test /app/test/Planificateur.Web.EndToEndTests/Planificateur.Web.EndToEndTests.csproj"

$result = docker inspect planificateur-e2e-tests-exec --format='{{.State.ExitCode}}'

docker rm planificateur-e2e-tests-exec

Exit $result