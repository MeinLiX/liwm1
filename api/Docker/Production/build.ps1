$ErrorActionPreference="Stop"

docker-compose -f "docker-compose.yml" build ; docker-compose -f "docker-compose.yml" push

Read-Host -Prompt "Press any key to continue"