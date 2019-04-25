Add-PSSnapin Microsoft.SharePoint.Powershell
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.Office.DocumentManagement")
[System.Reflection.Assembly]::LoadWithPartialName("Treasury.ECM.eCase.SusDeb.DOI.Common")

$site = Get-SPSite http://spv4-mad-treas/sites/bureaufour
$docIdProvider = New-Object Treasury.ECM.eCase.SusDeb.DOI.Common.eCaseDocIdProvider
[Microsoft.Office.DocumentManagement.DocumentID]::SetProvider($site, $docIdProvider)