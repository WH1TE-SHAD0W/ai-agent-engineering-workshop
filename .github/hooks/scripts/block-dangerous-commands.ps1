# preToolUse gate (PowerShell). See the .sh file for the contract.
$ErrorActionPreference = 'Stop'

$raw = [Console]::In.ReadToEnd()
if ([string]::IsNullOrWhiteSpace($raw)) { exit 0 }
try { $payload = $raw | ConvertFrom-Json } catch { exit 0 }

$cmd = $payload.toolArgs.command
if ([string]::IsNullOrWhiteSpace($cmd)) { exit 0 }

$deny = 'rm\s+-rf|git\s+push\s+--force|git\s+clean\s+-fd(\s|$)|DROP\s+TABLE|curl[^|]*\|\s*(ba)?sh|Remove-Item\s+.*-Recurse.*-Force'

if ($cmd -imatch $deny) {
    Write-Output '{"permissionDecision":"deny","permissionDecisionReason":"Blocked by repository policy: destructive or unreviewed-execution command."}'
}
exit 0