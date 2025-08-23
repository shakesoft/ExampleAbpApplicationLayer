param (
	$Namespace="exampleabpapplicationlayer-local",
    $ReleaseName="exampleabpapplicationlayer-local",
    $User = ""
)

if([string]::IsNullOrEmpty($User) -eq $false)
{
    $Namespace += '-' + $User
    $ReleaseName += '-' + $User
}

helm uninstall ${ReleaseName} --namespace ${Namespace}