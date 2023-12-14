$ErrorActionPreference="Stop"

docker-compose -f "docker-compose.Staging.yml" build ; docker-compose -f "docker-compose.Staging.yml" push

Read-Host -Prompt "Press any key to continue"