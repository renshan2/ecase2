#Force SharePoint cmdlet errors to register as terminating errors
$ErrorActionPreference = "Stop"

function AddDeploySolution
{
	param ($spSolutionName, $url)
	Write-Host "Adding $spSolutionName..."
    $spSolution = Add-SPSolution "$PWD\$spSolutionName"

    if ($spSolution.ContainsWebApplicationResource -eq $true)
    {
		Write-Host "Deploying to $url..."
        Install-SPSolution -Identity $spSolutionName -GacDeployment -CasPolicies -Force -WebApplication $url -Local
    }
    else
    {
		Write-Host "Deploying..."
        Install-SPSolution -Identity $spSolutionName -GacDeployment -CasPolicies -Force -Local
    }
    
    $spSolution = Get-SPSolution $spSolutionName
    if ($spSolution.Deployed -eq $false)
    {
        $counter = 1
        while (($spSolution.JobExists -eq $true) -and ($counter -lt $maximum))
        {
            Write-Host "Waiting on deployment for $spSolutionName"
            sleep $sleeptime
            $counter++
        }
    }
}

function RetractRemoveSolution
{
	param ($spSolutionName, $url)
	Try 
    { 
        $counter = 1
        $maximum = 100
        $sleeptime= 2

        $spSolution = Get-SPSolution $spSolutionName
        if ($spSolution.Deployed -eq $true)
        {
            if ($spSolution.ContainsWebApplicationResource)
            {
                Uninstall-SPSolution -identity $spSolutionName -Confirm:$false -Webapplication $url
            }
            else
            {
                Uninstall-SPSolution -identity $spSolutionName -Confirm:$false
            }
            
            while (($spSolution.JobExists -eq $true) -and ($counter -lt $maximum))
            {
                Write-Host "Retracting $spSolutionName..."
                sleep $sleeptime
                $counter++
            }
        }
        if ($counter -lt $maximum)
        {
            Remove-SPSolution -Identity $spSolutionName -Force -Confirm:$false
        }
        else
        {
            Write-Host "Unable to remove $spSolutionName"
        }

    }
 #   Catch [system.exception]
 #   {
        
 #   }
    finally {}
}

