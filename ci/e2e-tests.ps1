docker compose up -d --build

cd ..
$pwd = pwd;

docker run `
    --env IS_CI="true" `
    --net ci_default `
    --name planificateur-e2e-tests-exec `
    -v "${pwd}:/app" `
    -w /app `
    ombrelin/dotnet-playwright:8 `
    /bin/bash -c "dotnet build /app/test/Planificateur.Web.EndToEndTests/Planificateur.Web.EndToEndTests.csproj && ~/bin/playwright install-deps && sleep 5 && ~/bin/playwright install firefox && dotnet test /app/test/Planificateur.Web.EndToEndTests/Planificateur.Web.EndToEndTests.csproj"

$result = docker inspect planificateur-e2e-tests-exec --format='{{.State.ExitCode}}'

docker rm planificateur-e2e-tests-exec
cd ci
docker compose down

Exit $result