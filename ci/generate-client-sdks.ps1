
docker compose up -d --build

cd ..
$pwd = pwd;

Start-Sleep -Seconds 5;

$buildNumber = $args[0]

docker run `
    --rm `
    --net ci_default `
    -v "${pwd}:/app" `
    openapitools/openapi-generator-cli "generate --artifact-id planificateur-client-sdk --artifact-version 1.0.$buildNumber -i http://planificateur-e2e/swagger/v1/swagger.json -g typescript -o client-sdk-typescript"

cd client-sdk-ypescript

$npmToken = $args[1]

docker run `
    --rm `
    -v "${pwd}:/app" `
    node "cd /app && npm i && npm run build && npm set //npmjs.com/:_authToken $npmToken && npm publish --access public"

cd ../ci
docker compose down