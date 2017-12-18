$root = (split-path -parent $MyInvocation.MyCommand.Definition) + '\.'

$packageRoot = "$root"
$packageArtifacts = "$packageRoot\Artifacts"
$targetNugetExe = "$packageRoot\nuget.exe"

If (Test-Path $packageArtifacts)
{
    # Delete any existing output.
    Remove-Item $packageArtifacts\*.nupkg
}

If (!(Test-Path $targetNugetExe))
{
    $sourceNugetExe = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"

    Write-Host "NuGet.exe not found - downloading latest from $sourceNugetExe"

    $sourceNugetExe = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"

    Invoke-WebRequest $sourceNugetExe -OutFile $targetNugetExe
}

# Create new packages for any nuspec files that exist in this directory.
Foreach ($nuspec in $(Get-Item $packageRoot\*.nuspec))
{
    & $targetNugetExe pack $nuspec -outputdirectory $packageArtifacts
}