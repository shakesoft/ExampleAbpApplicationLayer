#!/bin/bash

if [[ ! -d certs ]]
then
    mkdir certs
    cd certs/
    if [[ ! -f localhost.pfx ]]
    then
        dotnet dev-certs https -v -ep localhost.pfx -p 1d011f9a-235f-4ab3-be3f-a8f7b6bd06d2 -t
    fi
    cd ../
fi

docker-compose up -d
