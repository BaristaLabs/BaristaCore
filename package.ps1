$CONFIGURATION = "Release"
$FRAMEWORK = "netcoreapp1.0"
dotnet restore
dotnet build -c $CONFIGURATION

# Create new packages for any nuspec files that exist in this directory.
Foreach ($nuspec in $(Get-Item *.nuspec))
{
    & nuget pack $nuspec -Properties Configuration=$CONFIGURATION
}

# Publish new packages for any nupkg files that exist in this directory.
Foreach ($nupkg in $(Get-Item *.nupkg))
{
    & nuget push $nupkg -Source https://api.nuget.org/v3/index.json
}