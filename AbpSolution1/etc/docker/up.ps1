docker network create abpsolution1 --label=abpsolution1
docker-compose -f containers/rabbitmq.yml up -d
docker-compose -f containers/redis.yml up -d
exit $LASTEXITCODE