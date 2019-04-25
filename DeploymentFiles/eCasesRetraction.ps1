Add-PsSnapin Microsoft.SharePoint.PowerShell

param
(
	$webApp = "https://apps.testecm.com/ecase/doi"
)

. .\SolutionManagement.ps1

#Retrive all WSPs in the local directory
[array] $spSolutionNames = Get-ChildItem -name | Where-Object {$_ -like "*.wsp"}

if ($spSolutionNames.count -gt 0)
{
	#Iterate over each WSP, attempting to retract it
	foreach ($spSolutionName in $spSolutionNames)
	{ RetractRemoveSolution $spSolutionName $webApp }

	Write-Host "Retraction Completed"
}

Write-Host "Please verify solutions have been retracted before executing any other powershell scripts"
