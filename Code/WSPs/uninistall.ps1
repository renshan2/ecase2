function Uninstall-AllSPSolutions {
    param (
        [switch] $Local,
        [switch] $Confirm
    ) 

  Start-SPAssignment -Global;
  foreach($solution in (Get-SPSolution | Where-Object { $_.Deployed })) {
    write-host "Uninstalling Solution " $solution.Name;
    if($solution.DeployedWebApplications.Count -gt 0) {
      Uninstall-SPSolution $solution –AllWebApplications -Local:$Local -Confirm:$Confirm;
    } else {
      Uninstall-SPSolution $solution -Local:$Local -Confirm:$Confirm;
    }
    do {
      Start-Sleep 5;
      $solution = Get-SPSolution $solution;
    } while($solution.JobExists -and $solution.Deployed) 
  } 
  Stop-SPAssignment -Global;
}

function Remove-AllSPSolutions {
    param (
        [switch] $Confirm
    ) 
    Get-SPSolution | Where-Object { !$_.Deployed } | Remove-SPSolution -Confirm:$Confirm
}

Uninstall-AllSPSolutions -Confirm
stsadm -o execadmsvcjobs
Remove-AllSPSolutions -Confirm
stsadm -o execadmsvcjobs