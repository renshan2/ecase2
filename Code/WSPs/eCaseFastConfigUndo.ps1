Add-PSSnapin Microsoft.FASTSearch.Powershell -ErrorAction SilentlyContinue 

# Clear the Content Collection, no prompt
Write-Host "Clearing the SP Content Collection.  A full crawl will need to be performed"
Clear-FASTSearchContentCollection -Name sp -Confirm:$false

# Check for eCaseFti, if found remove
Write-Host "Removing eCase Full-Text Index..."
$eCaseFti = Get-FASTSearchMetadataFullTextIndex -Name "ecasefti"
if ($eCaseFti)
{ Remove-FASTSearchMetadataFullTextIndex -FullTextIndex $eCaseFti -Confirm:$false }

Write-Host "Removing eCase Rank Profile..."
$eCaseRp = Get-FASTSearchMetadataRankProfile -Name "ecaserp"
if ($eCaseRp)
{ Remove-FASTSearchMetadataRankProfile -RankProfile $eCaseRp -Confirm:$false }
