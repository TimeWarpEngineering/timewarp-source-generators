#!/usr/bin/env pwsh

# Stop on first error
$ErrorActionPreference = "Stop"

# Store current location and move to script directory
Push-Location
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

try {
    Write-Host "Building Solution..."
    dotnet restore
    dotnet build --configuration Release --no-restore

    Write-Host "Creating NuGet Package..."
    # Create the Packages directory if it doesn't exist
    $packagesDir = "source/timewarp-source-generators/bin/Packages"
    New-Item -ItemType Directory -Force -Path $packagesDir | Out-Null

    # Pack the project and output to the Packages directory
    # NoWarn=NU5128 is added to suppress the warning about lib folder matching
    dotnet pack source/timewarp-source-generators/timewarp-source-generators.csproj `
        --configuration Release `
        --no-build `
        --output $packagesDir `
        -p:NoWarn=NU5128

    Write-Host "Build and Pack completed successfully"
    Write-Host "Packages have been placed in $packagesDir"
}
finally {
    # Return to original location
    Pop-Location
}
