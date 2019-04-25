Add-PSSnapin Microsoft.FASTSearch.Powershell -ErrorAction SilentlyContinue 

# Create an eCase Crawled Properties Category
Write-Host "Creating Crawled Properties Category..."
$eCaseCpCategory = Get-FASTSearchMetadataCategory -Name SharePoint 
$guid = "{00130329-0000-0130-c000-000000131346}" # This is the static GUID for the SharePoint category

# Configure Crawled Properties
Write-Host "Creating/Getting Crawled Properties..."
$ows_OriginatorCp = Get-FASTSearchMetadataCrawledProperty -Name "ows_originator" -ErrorAction SilentlyContinue
if (!$ows_OriginatorCp) { $ows_OriginatorCp = New-FASTSearchMetadataCrawledProperty -Name "ows_originator" -Propset $guid -Varianttype 31 }
elseif ($ows_OriginatorCp.Count) { $ows_OriginatorCp = $ows_OriginatorCp | Where-Object {$_.VariantType -eq 31} }

$ows_RelatedIssuesCp = Get-FASTSearchMetadataCrawledProperty -Name "ows_relatedissues" -ErrorAction SilentlyContinue
if (!$ows_RelatedIssuesCp) { $ows_RelatedIssuesCp = New-FASTSearchMetadataCrawledProperty -Name "ows_relatedissues" -Propset $guid -Varianttype 31 }
elseif ($ows_RelatedIssuesCp.Count) { $ows_RelatedIssuesCp = $ows_RelatedIssuesCp | Where-Object {$_.VariantType -eq 31} }

$ows_RelatedLegalIssuesCp = Get-FASTSearchMetadataCrawledProperty -Name "ows_rellglissues" -ErrorAction SilentlyContinue
if (!$ows_RelatedLegalIssuesCp) { $ows_RelatedLegalIssuesCp = New-FASTSearchMetadataCrawledProperty -Name "ows_rellglissues" -Propset $guid -Varianttype 4127 }
elseif ($ows_RelatedLegalIssuesCp.Count) { $ows_RelatedLegalIssuesCp = $ows_RelatedLegalIssuesCp | Where-Object {$_.VariantType -eq 4127} }

$ows_DocumentDateCp = Get-FASTSearchMetadataCrawledProperty -Name "ows_documentdate"  -ErrorAction SilentlyContinue
if (!$ows_DocumentDateCp) { $ows_DocumentDateCp = New-FASTSearchMetadataCrawledProperty -Name "ows_documentdate" -Propset $guid -Varianttype 64 }
elseif ($ows_DocumentDateCp.Count) { $ows_DocumentDateCp = $ows_DocumentDateCp | Where-Object {$_.VariantType -eq 64} }

# Configure Managed Properties
Write-Host "Creating/Getting Managed Properties..."
$ows_OriginatorMp = Get-FASTSearchMetadataManagedProperty -Name "ecaseoriginator"
if (!$ows_OriginatorMp) 
{ 
	$ows_OriginatorMp = New-FASTSearchMetadataManagedProperty -Name "ecaseoriginator" -type 1 -description "eCase Document Author" 
	New-FASTSearchMetadataCrawledPropertyMapping -ManagedProperty $ows_OriginatorMp -CrawledProperty $ows_OriginatorCp
	Set-FASTSearchMetadataManagedProperty -ManagedProperty $ows_OriginatorMp -RefinementEnabled 1 -SortableType 1 -Queryable 1
}

$ows_RelatedIssuesMp = Get-FASTSearchMetadataManagedProperty -Name "ecaserelatedissues"
if (!$ows_RelatedIssuesMp)
{
	$ows_RelatedIssuesMp = New-FASTSearchMetadataManagedProperty -Name "ecaserelatedissues" -type 1 -description "eCase Related Legal Issues"
	New-FASTSearchMetadataCrawledPropertyMapping -ManagedProperty $ows_RelatedIssuesMp -CrawledProperty $ows_RelatedIssuesCp
	New-FASTSearchMetadataCrawledPropertyMapping -ManagedProperty $ows_RelatedIssuesMp -CrawledProperty $ows_RelatedLegalIssuesCp
	Set-FASTSearchMetadataManagedProperty -ManagedProperty $ows_RelatedIssuesMp -RefinementEnabled 1 -SortableType 1 -Queryable 1 -MergeCrawledProperties 1
}

$ows_DocumentDateMp = Get-FASTSearchMetadataManagedProperty -Name "ecasedocumentdate"
if (!$ows_DocumentDateMp)
{
	$ows_DocumentDateMp = New-FASTSearchMetadataManagedProperty -Name "ecasedocumentdate" -type 6 -description "eCase Document DateTime as Text"
	New-FASTSearchMetadataCrawledPropertyMapping -ManagedProperty $ows_DocumentDateMp -CrawledProperty $ows_DocumentDateCp
	Set-FASTSearchMetadataManagedProperty -ManagedProperty $ows_DocumentDateMp -RefinementEnabled 1 -SortableType 1 -Queryable 1
}

# OOTB Managed Property
$assignedto = Get-FASTSearchMetadataManagedProperty -Name "AssignedTo"
Set-FASTSearchMetadataManagedProperty -ManagedProperty $assignedto -RefinementEnabled 1 -SortableType 1 -Queryable 1

# Create Full-Text Index with Lemmatization enabled
Write-Host "Creating eCase Full-Text Index..."
$eCaseFti = New-FASTSearchMetadataFullTextIndex -Name "ecasefti" -Description "Full-Text Index for use by eCase Management System"
$eCaseFti = Set-FASTSearchMetadataFullTextIndex -FullTextIndex $eCaseFti -StemmingEnabled 1

# Create the Rank Profile and associate it with the Full-Text Index
Write-Host "Creating eCase Rank Profile..."
$defaultRp = Get-FASTSearchMetadataRankProfile "default"
$eCaseRp = New-FASTSearchMetadataRankProfile -Name "ecaserp" -Template $defaultRp
$rcList = $eCaseRp.GetFullTextIndexRanks()
$rcList.Create($eCaseFti)

# Map Managed Properties to eCase FTI
Write-Host "Mapping managed properties to eCase Full-Text Index..."
$defaultFti = Get-FASTSearchMetadataFullTextIndex -name "content"
$defaultFtiMap = Get-FASTSearchMetadataFullTextIndexMapping -FullTextIndex $defaultFti
foreach ($mapping in $defaultFtiMap)
{
	if ($mapping.ImportanceLevel -gt 7) # Internal ILs are allowed to break the rules?
	{ $il = 7; }
	else
	{ $il = $mapping.ImportanceLevel }
	
	New-FASTSearchMetadataFullTextIndexMapping -FullTextIndex $eCaseFti -Level $il -ManagedProperty $mapping.ManagedProperty
}
New-FASTSearchMetadataFullTextIndexMapping -FullTextIndex $eCaseFti -Level 5 -ManagedProperty $ows_OriginatorMp
New-FASTSearchMetadataFullTextIndexMapping -FullTextIndex $eCaseFti -Level 5 -ManagedProperty $ows_RelatedIssuesMp
New-FASTSearchMetadataFullTextIndexMapping -FullTextIndex $eCaseFti -Level 5 -ManagedProperty $assignedto