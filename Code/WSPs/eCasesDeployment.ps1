param
(
	#$webApp = "http://spv4-mad-treas"
	$webApp = "http://portal.contoso.com"
)

. .\SolutionManagement.ps1

#Retrive all WSPs in the local directory
[array] $spSolutionNames = Get-ChildItem -name | Where-Object {$_ -like "*.wsp"}

if ($spSolutionNames.count -gt 0)
{
	#Iterate over each WSP, attempting to deploy it
	foreach ($spSolutionName in $spSolutionNames)
	{ AddDeploySolution $spSolutionName $webApp }

	Write-Host "Deployment Completed"
}

Write-Host "Please verify solutions have been deployed, then execute eCasesSiteProvisioner.ps1"
