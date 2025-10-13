param (
	$Namespace="abpsolution1-local",
    $ReleaseName="abpsolution1-local",
    $User = ""
)

if([string]::IsNullOrEmpty($User) -eq $false)
{
    $Namespace += '-' + $User
    $ReleaseName += '-' + $User
}

helm uninstall ${ReleaseName} --namespace ${Namespace}
exit $LASTEXITCODE