param
(
	$webApp = "https://apps.testecm.gov",
	$scUrl = "$webApp/ecase/doi",
	$scName = "SUSDEB DOI",
	$account = [Environment]::UserDomainName + '\' + [Environment]::UserName,
	$email = ""
)

if (!$email)
{
	Import-Module activedirectory
	$userName = [Environment]::UserName
	$user = Get-ADUser $userName -Properties mail
	if ($user.mail) { $email = $user.mail }
	else { $email = "ren.shan@cabinjohnconsulting.com" }
}

Write-Host "Deleting $scUrl..."
Remove-SPSite -Identity "$scUrl" -Confirm:$False -ErrorAction SilentlyContinue

Write-Host "Creating $scUrl for $account, $email"
$gc = Start-SPAssignment
$site = $gc | New-SPSite -Url "$scUrl" -Name $scName -Template "SusDebRootSiteDefinition#0" -OwnerEmail "$email" -OwnerAlias "$account"
Write-Host("Created Site $site")

# Create Owners, Members, Visitors Groups
$site.RootWeb.CreateDefaultAssociatedGroups([System.Security.Principal.WindowsIdentity]::GetCurrent().Name, "", "")

Get-Content eCaseOwners.txt | ForEach-Object { 
    $user = $site.RootWeb.EnsureUser($_)
    Write-Host Adding $user.Name to Site Owners Group
    $site.RootWeb.AssociatedOwnerGroup.AddUser($user)
}

Get-Content eCaseMembers.txt | ForEach-Object { 
    $user = $site.RootWeb.EnsureUser($_)
    Write-Host Adding $user.Name to Site Members Group
    $site.RootWeb.AssociatedMemberGroup.AddUser($user)
}

$gc | Stop-SPAssignment
	
# Launch IE
$ie = New-Object -com internetexplorer.application
$ie.visible = $true;
$ie.navigate($scUrl)