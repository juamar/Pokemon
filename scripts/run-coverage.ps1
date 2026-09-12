<#
.SYNOPSIS
	Runs the full test suite with code coverage collection and generates a local HTML report.

.DESCRIPTION
	Replacement for Sonar-based coverage in this environment. Uses coverlet.collector
	(via `dotnet test --collect:"XPlat Code Coverage"`) plus dotnet-reportgenerator-globaltool
	to produce a browsable HTML coverage report under ./coveragereport.

	Run this at the end of every unit of work that touches Pokemons.Domain logic
	(see .github/copilot-instructions.md - "Testing requirement per unit of work").

.PARAMETER OpenReport
	If set, opens the generated report in the default browser after generation.

.EXAMPLE
	./scripts/run-coverage.ps1
	./scripts/run-coverage.ps1 -OpenReport
#>

param(
	[switch]$OpenReport
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

Write-Host "==> Cleaning previous coverage results..." -ForegroundColor Cyan
Get-ChildItem -Path . -Recurse -Directory -Filter "TestResults" -ErrorAction SilentlyContinue |
	Remove-Item -Recurse -Force -ErrorAction SilentlyContinue

if (Test-Path "./coveragereport") {
	Remove-Item "./coveragereport" -Recurse -Force
}

Write-Host "==> Running tests with coverage collection..." -ForegroundColor Cyan
dotnet test --collect:"XPlat Code Coverage"
if ($LASTEXITCODE -ne 0) {
	Write-Warning "One or more tests failed. Coverage report will still be generated for what ran."
}

$coverageFiles = Get-ChildItem -Path . -Recurse -Filter "coverage.cobertura.xml" -ErrorAction SilentlyContinue
if (-not $coverageFiles) {
	Write-Error "No coverage.cobertura.xml files found. Ensure test projects reference 'coverlet.collector'."
	exit 1
}

Write-Host "==> Ensuring dotnet-reportgenerator-globaltool is installed..." -ForegroundColor Cyan
$installed = dotnet tool list -g | Select-String "dotnet-reportgenerator-globaltool"
if (-not $installed) {
	dotnet tool install -g dotnet-reportgenerator-globaltool
}

Write-Host "==> Generating HTML coverage report..." -ForegroundColor Cyan
reportgenerator `
	-reports:"**/TestResults/**/coverage.cobertura.xml" `
	-targetdir:"coveragereport" `
	-reporttypes:Html`;TextSummary

$summaryPath = "./coveragereport/Summary.txt"
if (Test-Path $summaryPath) {
	Write-Host ""
	Write-Host "==> Coverage summary:" -ForegroundColor Green
	Get-Content $summaryPath
}

$reportPath = Join-Path $repoRoot "coveragereport/index.html"
Write-Host ""
Write-Host "==> Full report: $reportPath" -ForegroundColor Green

if ($OpenReport) {
	Start-Process $reportPath
}
