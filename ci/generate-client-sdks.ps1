docker compose up -d

cd ..
$pwd = pwd;

docker run `
    --rm `
    --net ci_default `
    -v "${pwd}:/app" `
    openapitools/openapi-generator-cli generate -i http://planificateur-e2e/swagger/v1/swagger.json -g typescript -o /app/client-sdk