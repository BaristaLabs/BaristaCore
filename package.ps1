$CONFIGURATION = "Release"
$FRAMEWORK = "netstandard2.0"

$root = (split-path -parent $MyInvocation.MyCommand.Definition)
$packageRoot = "$root\nuget"
$versionFile = "$packageRoot\.version"
$artifactsPath = "$packageRoot\artifacts"

#Delete any existing artifacts
If (Test-Path $artifactsPath)
{
    Remove-Item $artifactsPath\*.nupkg
}

$common = "$root\src\BaristaLabs.BaristaCore.Common\"
$extensions = "$root\src\BaristaLabs.BaristaCore.Extensions\"
$aspnetcorecommon = "$root\src\BaristaLabs.BaristaCore.AspNetCore.Common\"
dotnet build -c $CONFIGURATION -f $FRAMEWORK $common
dotnet build -c $CONFIGURATION -f $FRAMEWORK $extensions
dotnet build -c $CONFIGURATION -f $FRAMEWORK $aspnetcorecommon

$VERSION = (Get-Content $versionFile)
Write-Host "Setting .nuspec version tag to $VERSION"
$compiledNuspec = "$packageRoot\compiled.nuspec"

# Create new packages for any nuspec files that exist in this directory.
Foreach ($nuspec in $(Get-Item "$packageRoot\*.nuspec"))
{
	$content = (Get-Content $nuspec)
    $content = $content -replace '\$version\$',$VERSION
	$content = $content -replace '\$configuration\$',$CONFIGURATION
	$content = $content -replace '\$framework\$',$FRAMEWORK
	$content = $content -replace '\$root\$',$root
    $content | Out-File $compiledNuspec

    & nuget pack $compiledNuspec -outputdirectory $artifactsPath 
}

# Delete compiled temporary nuspec.
If (Test-Path $compiledNuspec)
{
    Remove-Item $compiledNuspec
}

# Publish new packages for any nupkg files that exist in this directory.
Foreach ($nupkg in $(Get-Item $artifactsPath\*.nupkg))
{
    & nuget push $nupkg -Source https://api.nuget.org/v3/index.json
}