#!/bin/bash

if [[ ! -d certs ]]
then
    mkdir certs
    cd certs/
    if [[ ! -f localhost.pfx ]]
    then
        dotnet dev-certs https -v -ep localhost.pfx -p 0674b402-718f-4c23-80e3-d577dd9e305b -t
    fi
    cd ../
fi

docker-compose up -d
