docker network create network
docker run \
    --env IS_CI="true" \
    --env TZ="Europe/Paris" \
    --net network \
    --name planificateur-e2e-tests-runner \
    -v "$(pwd):/app" \
    -v /var/run/docker.sock:/var/run/docker.sock \
    mcr.microsoft.com/playwright/dotnet:v1.42.0-jammy \
    /bin/bash -c "dotnet test /app/test/Planificateur.Web.EndToEndTests/Planificateur.Web.EndToEndTests.csproj"
    
result=$(sudo docker inspect planificateur-e2e-tests-runner --format='{{.State.ExitCode}}')

docker stop planificateur-e2e-tests-runner
docker rm planificateur-e2e-tests-runner
docker network rm network

echo "$result"
exit "$result"
