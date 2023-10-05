# run a develop environment with docker contianer
docker run -it --rm \
    -v `pwd`:/app/GameMod \
    bitnami/dotnet-sdk:6 bash \
    --init-file "~/GameMod/.scripts/setup_container.sh"
