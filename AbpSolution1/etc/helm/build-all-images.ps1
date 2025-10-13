./build-image.ps1 -ProjectPath "../../src/AbpSolution1.DbMigrator/AbpSolution1.DbMigrator.csproj" -ImageName abpsolution1/dbmigrator
./build-image.ps1 -ProjectPath "../../src/AbpSolution1.Web.Public/AbpSolution1.Web.Public.csproj" -ImageName abpsolution1/webpublic
./build-image.ps1 -ProjectPath "../../src/AbpSolution1.HttpApi.Host/AbpSolution1.HttpApi.Host.csproj" -ImageName abpsolution1/httpapihost
./build-image.ps1 -ProjectPath "../../angular" -ImageName abpsolution1/angular -ProjectType "angular"
./build-image.ps1 -ProjectPath "../../src/AbpSolution1.AuthServer/AbpSolution1.AuthServer.csproj" -ImageName abpsolution1/authserver
