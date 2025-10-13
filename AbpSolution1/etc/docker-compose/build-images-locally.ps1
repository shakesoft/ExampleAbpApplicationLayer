param ($version='latest')

$currentFolder = $PSScriptRoot
$slnFolder = Join-Path $currentFolder "../../"

Write-Host "********* BUILDING DbMigrator *********" -ForegroundColor Green
$dbMigratorFolder = Join-Path $slnFolder "src/AbpSolution1.DbMigrator"
Set-Location $dbMigratorFolder
dotnet publish -c Release
docker build -f Dockerfile.local -t abpsolution1-db-migrator:$version .



Write-Host "********* BUILDING Angular Application *********" -ForegroundColor Green
$angularAppFolder = Join-Path $slnFolder "./angular"
Set-Location $angularAppFolder
npx yarn
npm run build:prod
docker build -f Dockerfile.local -t abpsolution1-angular:$version .

Write-Host "********* BUILDING Api.Host Application *********" -ForegroundColor Green
$hostFolder = Join-Path $slnFolder "src/AbpSolution1.HttpApi.Host"
Set-Location $hostFolder
dotnet publish -c Release
docker build -f Dockerfile.local -t abpsolution1-api:$version .


Write-Host "********* BUILDING Public Web Application *********" -ForegroundColor Green
$webPublicFolder = Join-Path $slnFolder "src/AbpSolution1.Web.Public"
Set-Location $webPublicFolder
dotnet publish -c Release
docker build -f Dockerfile.local -t abpsolution1-web-public:$version .



Write-Host "********* BUILDING AuthServer Application *********" -ForegroundColor Green
$authServerAppFolder = Join-Path $slnFolder "src/AbpSolution1.AuthServer"
Set-Location $authServerAppFolder
dotnet publish -c Release
docker build -f Dockerfile.local -t abpsolution1-authserver:$version .

### ALL COMPLETED
Write-Host "COMPLETED" -ForegroundColor Green
Set-Location $currentFolder
exit $LASTEXITCODE