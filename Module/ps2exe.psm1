﻿<#
.SYNOPSIS
ps2exe is a module to compile powershell scripts to executables.
.NOTES
Version: 1.0.16
Date: 2025-07-20
Author: Markus Scholtes
#>

# Load module manually for security reasons
. "$PSScriptRoot/ps2exe.ps1"

# Define aliases
Set-Alias ps2exe Invoke-ps2exe -Scope Global
Set-Alias ps2exe.ps1 Invoke-ps2exe -Scope Global
Set-Alias Win-PS2EXE "$PSScriptRoot\Win-PS2EXE.exe" -Scope Global
Set-Alias Win-PS2EXE.exe "$PSScriptRoot\Win-PS2EXE.exe" -Scope Global

# Export functions
Export-ModuleMember -Function @('Invoke-PS2EXE')
# Export aliases
Export-ModuleMember -Alias @('ps2exe', 'ps2exe.ps1', 'Win-PS2EXE', 'Win-PS2EXE.exe')
