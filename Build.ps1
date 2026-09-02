<#
.SYNOPSIS
    Builds NMSRetroInstaller.exe into bin\ and runs the self-check over it.

.DESCRIPTION
    Wraps "dotnet publish" with the three things that are easy to get wrong by hand: closing a
    running copy that would otherwise lock the single output file, waiting properly on the
    self-check (a GUI-subsystem executable, so PowerShell does not wait for it on its own), and
    saying where the finished executable is.

.EXAMPLE
    .\Build.ps1
    .\Build.ps1 -Configuration Debug -NoCheck
#>
[CmdletBinding()]
param(
    [ValidateSet('Release', 'Debug')]
    [string] $Configuration = 'Release',

    # Skip the self-check and just produce the executable.
    [switch] $NoCheck,

    # Start the installer once it is built.
    [switch] $Run
)

$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot

$exe = Join-Path $PSScriptRoot 'bin\NMSRetroInstaller.exe'

# bin\ holds one file and a running copy locks it, which fails the publish half way through.
$running = Get-Process NMSRetroInstaller -ErrorAction SilentlyContinue
if ($running) {
    Write-Host 'Closing the running installer...' -ForegroundColor DarkGray
    $running | Stop-Process -Force
    Start-Sleep -Milliseconds 500
}

Write-Host "Publishing $Configuration..." -ForegroundColor Cyan
dotnet publish NMSRetroInstaller.csproj -c $Configuration --nologo
if ($LASTEXITCODE -ne 0) { throw "Build failed with exit code $LASTEXITCODE" }

if (-not (Test-Path $exe)) { throw "Build reported success but $exe is missing" }

$size = '{0:N1} MB' -f ((Get-Item $exe).Length / 1MB)
Write-Host "  $exe  ($size)" -ForegroundColor Green

if (-not $NoCheck) {
    Write-Host 'Running the self-check...' -ForegroundColor Cyan

    # A WinExe writes to stderr but PowerShell does not wait on it, so redirect and wait explicitly.
    $log = Join-Path $env:TEMP 'nmsretro-selfcheck.txt'
    $check = Start-Process $exe -ArgumentList '--selfcheck' `
        -NoNewWindow -Wait -PassThru -RedirectStandardError $log

    Get-Content $log | ForEach-Object {
        $colour = if ($_ -match 'FAIL') { 'Red' } elseif ($_ -match '^\s+ok') { 'DarkGray' } else { 'White' }
        Write-Host $_ -ForegroundColor $colour
    }

    if ($check.ExitCode -ne 0) { throw 'Self-check failed' }
}

if ($Run) {
    Write-Host 'Starting the installer...' -ForegroundColor Cyan
    Start-Process $exe
}
