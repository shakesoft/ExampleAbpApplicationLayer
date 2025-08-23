./build-image.ps1 -ProjectPath "../../src/ExampleAbpApplicationLayer.DbMigrator/ExampleAbpApplicationLayer.DbMigrator.csproj" -ImageName exampleabpapplicationlayer/dbmigrator
./build-image.ps1 -ProjectPath "../../src/ExampleAbpApplicationLayer.HttpApi.Host/ExampleAbpApplicationLayer.HttpApi.Host.csproj" -ImageName exampleabpapplicationlayer/httpapihost
./build-image.ps1 -ProjectPath "../../angular" -ImageName exampleabpapplicationlayer/angular -ProjectType "angular"
