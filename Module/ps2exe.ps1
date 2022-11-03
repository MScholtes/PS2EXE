#Requires -Version 3.0

<#
.SYNOPSIS
Converts powershell scripts to standalone executables.
.DESCRIPTION
Converts powershell scripts to standalone executables. GUI output and input is activated with one switch,
real windows executables are generated. You may use the graphical front end Win-PS2EXE for convenience.

Please see Remarks on project page for topics "GUI mode output formatting", "Config files", "Password security",
"Script variables" and "Window in background in -noConsole mode".

A generated executable has the following reserved parameters:

-debug              Forces the executable to be debugged. It calls "System.Diagnostics.Debugger.Launch()".
-extract:<FILENAME> Extracts the powerShell script inside the executable and saves it as FILENAME.
										The script will not be executed.
-wait               At the end of the script execution it writes "Hit any key to exit..." and waits for a
										key to be pressed.
-end                All following options will be passed to the script inside the executable.
										All preceding options are used by the executable itself.
.PARAMETER inputFile
Powershell script to convert to executable (file has to be UTF8 or UTF16 encoded)
.PARAMETER outputFile
destination executable file name or folder, defaults to inputFile with extension '.exe'
.PARAMETER prepareDebug
create helpful information for debugging of generated executable. See parameter -debug there
.PARAMETER x86
compile for 32-bit runtime only
.PARAMETER x64
compile for 64-bit runtime only
.PARAMETER lcid
location ID for the compiled executable. Current user culture if not specified
.PARAMETER STA
Single Thread Apartment mode
.PARAMETER MTA
Multi Thread Apartment mode
.PARAMETER nested
internal use
.PARAMETER noConsole
the resulting executable will be a Windows Forms app without a console window.
You might want to pipe your output to Out-String to prevent a message box for every line of output
(example: dir C:\ | Out-String)
.PARAMETER UNICODEEncoding
encode output as UNICODE in console mode, useful to display special encoded chars
.PARAMETER credentialGUI
use GUI for prompting credentials in console mode instead of console input
.PARAMETER iconFile
icon file name for the compiled executable
.PARAMETER title
title information (displayed in details tab of Windows Explorer's properties dialog)
.PARAMETER description
description information (not displayed, but embedded in executable)
.PARAMETER company
company information (not displayed, but embedded in executable)
.PARAMETER product
product information (displayed in details tab of Windows Explorer's properties dialog)
.PARAMETER copyright
copyright information (displayed in details tab of Windows Explorer's properties dialog)
.PARAMETER trademark
trademark information (displayed in details tab of Windows Explorer's properties dialog)
.PARAMETER version
version information (displayed in details tab of Windows Explorer's properties dialog)
.PARAMETER configFile
write a config file (<outputfile>.exe.config)
.PARAMETER noConfigFile
compatibility parameter
.PARAMETER noOutput
the resulting executable will generate no standard output (includes verbose and information channel)
.PARAMETER noError
the resulting executable will generate no error output (includes warning and debug channel)
.PARAMETER noVisualStyles
disable visual styles for a generated windows GUI application. Only applicable with parameter -noConsole
.PARAMETER exitOnCancel
exits program when Cancel or "X" is selected in a Read-Host input box. Only applicable with parameter -noConsole
.PARAMETER DPIAware
if display scaling is activated, GUI controls will be scaled if possible. Only applicable with parameter -noConsole
.PARAMETER requireAdmin
if UAC is enabled, compiled executable will run only in elevated context (UAC dialog appears if required)
.PARAMETER supportOS
use functions of newest Windows versions (execute [Environment]::OSVersion to see the difference)
.PARAMETER virtualize
application virtualization is activated (forcing x86 runtime)
.PARAMETER longPaths
enable long paths ( > 260 characters) if enabled on OS (works only with Windows 10)
.EXAMPLE
Invoke-ps2exe C:\Data\MyScript.ps1
Compiles C:\Data\MyScript.ps1 to C:\Data\MyScript.exe as console executable
.EXAMPLE
ps2exe -inputFile C:\Data\MyScript.ps1 -outputFile C:\Data\MyScriptGUI.exe -iconFile C:\Data\Icon.ico -noConsole -title "MyScript" -version 0.0.0.1
Compiles C:\Data\MyScript.ps1 to C:\Data\MyScriptGUI.exe as graphical executable, icon and meta data
.EXAMPLE
Win-PS2EXE
Start graphical front end to Invoke-ps2exe
.NOTES
Version: 0.5.0.27
Date: 2021-11-21
Author: Ingo Karstein, Markus Scholtes
.LINK
https://www.powershellgallery.com/packages/ps2exe
.LINK
https://github.com/MScholtes/PS2EXE
#>
function Invoke-ps2exe {
	[CmdletBinding()]
	Param([STRING]$inputFile = $NULL, [STRING]$outputFile = $NULL, [SWITCH]$prepareDebug, [SWITCH]$x86, [SWITCH]$x64, [int]$lcid,
		[SWITCH]$STA, [SWITCH]$MTA, [SWITCH]$nested, [SWITCH]$noConsole, [SWITCH]$UNICODEEncoding, [SWITCH]$credentialGUI, [STRING]$iconFile = $NULL,
		[STRING]$title, [STRING]$description, [STRING]$company, [STRING]$product, [STRING]$copyright, [STRING]$trademark, [STRING]$version,
		[SWITCH]$configFile, [SWITCH]$noConfigFile, [SWITCH]$noOutput, [SWITCH]$noError, [SWITCH]$noVisualStyles, [SWITCH]$exitOnCancel,
		[SWITCH]$DPIAware, [SWITCH]$requireAdmin, [SWITCH]$supportOS, [SWITCH]$virtualize, [SWITCH]$longPaths)

	<################################################################################>
	<##                                                                            ##>
	<##      PS2EXE-GUI v0.5.0.27                                                  ##>
	<##      Written by: Ingo Karstein (http://blog.karstein-consulting.com)       ##>
	<##      Reworked and GUI support by Markus Scholtes                           ##>
	<##                                                                            ##>
	<##      This script is released under Microsoft Public Licence                ##>
	<##          that can be downloaded here:                                      ##>
	<##          http://www.microsoft.com/opensource/licenses.mspx#Ms-PL           ##>
	<##                                                                            ##>
	<################################################################################>

	if (!$nested) {
		Write-Output "PS2EXE-GUI v0.5.0.27 by Ingo Karstein, reworked and GUI support by Markus Scholtes`n"
	}
	else {
		Write-Output "PowerShell Desktop environment started...`n"
	}

	if ([STRING]::IsNullOrEmpty($inputFile)) {
		Write-Output "Usage:`n"
		Write-Output "Invoke-ps2exe [-inputFile] '<filename>' [[-outputFile] '<filename>']"
		Write-Output "              [-prepareDebug] [-x86|-x64] [-lcid <id>] [-STA|-MTA] [-noConsole] [-UNICODEEncoding]"
		Write-Output "              [-credentialGUI] [-iconFile '<filename>'] [-title '<title>'] [-description '<description>']"
		Write-Output "              [-company '<company>'] [-product '<product>'] [-copyright '<copyright>'] [-trademark '<trademark>']"
		Write-Output "              [-version '<version>'] [-configFile] [-noOutput] [-noError] [-noVisualStyles] [-exitOnCancel]"
		Write-Output "              [-DPIAware] [-requireAdmin] [-supportOS] [-virtualize] [-longPaths]`n"
		Write-Output "      inputFile = Powershell script that you want to convert to executable (file has to be UTF8 or UTF16 encoded)"
		Write-Output "     outputFile = destination executable file name or folder, defaults to inputFile with extension '.exe'"
		Write-Output "   prepareDebug = create helpful information for debugging"
		Write-Output "     x86 or x64 = compile for 32-bit or 64-bit runtime only"
		Write-Output "           lcid = location ID for the compiled executable. Current user culture if not specified"
		Write-Output "     STA or MTA = 'Single Thread Apartment' or 'Multi Thread Apartment' mode"
		Write-Output "      noConsole = the resulting executable will be a Windows Forms app without a console window"
		Write-Output "UNICODEEncoding = encode output as UNICODE in console mode"
		Write-Output "  credentialGUI = use GUI for prompting credentials in console mode"
		Write-Output "       iconFile = icon file name for the compiled executable"
		Write-Output "          title = title information (displayed in details tab of Windows Explorer's properties dialog)"
		Write-Output "    description = description information (not displayed, but embedded in executable)"
		Write-Output "        company = company information (not displayed, but embedded in executable)"
		Write-Output "        product = product information (displayed in details tab of Windows Explorer's properties dialog)"
		Write-Output "      copyright = copyright information (displayed in details tab of Windows Explorer's properties dialog)"
		Write-Output "      trademark = trademark information (displayed in details tab of Windows Explorer's properties dialog)"
		Write-Output "        version = version information (displayed in details tab of Windows Explorer's properties dialog)"
		Write-Output "     configFile = write a config file (<outputfile>.exe.config)"
		Write-Output "       noOutput = the resulting executable will generate no standard output (includes verbose and information channel)"
		Write-Output "        noError = the resulting executable will generate no error output (includes warning and debug channel)"
		Write-Output " noVisualStyles = disable visual styles for a generated windows GUI application (only with -noConsole)"
		Write-Output "   exitOnCancel = exits program when Cancel or ""X"" is selected in a Read-Host input box (only with -noConsole)"
		Write-Output "       DPIAware = if display scaling is activated, GUI controls will be scaled if possible (only with -noConsole)"
		Write-Output "   requireAdmin = if UAC is enabled, compiled executable run only in elevated context (UAC dialog appears if required)"
		Write-Output "      supportOS = use functions of newest Windows versions (execute [Environment]::OSVersion to see the difference)"
		Write-Output "     virtualize = application virtualization is activated (forcing x86 runtime)"
		Write-Output "      longPaths = enable long paths ( > 260 characters) if enabled on OS (works only with Windows 10)`n"
		Write-Output "Input file not specified!"
		return
	}

	if (!$nested -and ($PSVersionTable.PSEdition -eq "Core")) {
		# starting Windows Powershell
		$CallParam = ""
		foreach ($Param in $PSBoundparameters.GetEnumerator()) {
			if ($Param.Value -is [System.Management.Automation.SwitchParameter]) {
				if ($Param.Value.IsPresent)
				{	$CallParam += " -$($Param.Key):`$TRUE" }
				else
				{ $CallParam += " -$($Param.Key):`$FALSE" }
			}
			else {
				if ($Param.Value -is [STRING]) {
					if (($Param.Value -match " ") -or ([STRING]::IsNullOrEmpty($Param.Value)))
					{	$CallParam += " -$($Param.Key) '$($Param.Value)'" }
					else
					{	$CallParam += " -$($Param.Key) $($Param.Value)" }
				}
				else
				{ $CallParam += " -$($Param.Key) $($Param.Value)" }
			}
		}

		$CallParam += " -nested"

		powershell -Command "&'$($MyInvocation.MyCommand.Name)' $CallParam"
		return
	}

	# retrieve absolute paths independent if path is given relative oder absolute
	$inputFile = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($inputFile)
	if ([STRING]::IsNullOrEmpty($outputFile)) {
		$outputFile = ([System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($inputFile), [System.IO.Path]::GetFileNameWithoutExtension($inputFile) + ".exe"))
	}
	else {
		$outputFile = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($outputFile)
		if ((Test-Path $outputFile -PathType Container)) {
			$outputFile = ([System.IO.Path]::Combine($outputFile, [System.IO.Path]::GetFileNameWithoutExtension($inputFile) + ".exe"))
		}
	}

	if (!(Test-Path $inputFile -PathType Leaf)) {
		Write-Error "Input file $($inputfile) not found!"
		return
	}

	if ($inputFile -eq $outputFile) {
		Write-Error "Input file is identical to output file!"
		return
	}

	if (($outputFile -notlike "*.exe") -and ($outputFile -notlike "*.com")) {
		Write-Error "Output file must have extension '.exe' or '.com'!"
		return
	}

	if (!([STRING]::IsNullOrEmpty($iconFile))) {
		# retrieve absolute path independent if path is given relative oder absolute
		$iconFile = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($iconFile)

		if (!(Test-Path $iconFile -PathType Leaf)) {
			Write-Error "Icon file $($iconFile) not found!"
			return
		}
	}

	if ($requireAdmin -and $virtualize) {
		Write-Error "-requireAdmin cannot be combined with -virtualize"
		return
	}
	if ($supportOS -and $virtualize) {
		Write-Error "-supportOS cannot be combined with -virtualize"
		return
	}
	if ($longPaths -and $virtualize) {
		Write-Error "-longPaths cannot be combined with -virtualize"
		return
	}

	$CFGFILE = $FALSE
	if ($configFile) {
		$CFGFILE = $TRUE
		if ($noConfigFile) {
			Write-Error "-configFile cannot be combined with -noConfigFile"
			return
		}
	}
	if (!$CFGFILE -and $longPaths) {
		Write-Warning "Forcing generation of a config file, since the option -longPaths requires this"
		$CFGFILE = $TRUE
	}

	if ($STA -and $MTA) {
		Write-Error "You cannot use switches -STA and -MTA at the same time!"
		return
	}

	if (!$MTA -and !$STA) {
		# Set default apartment mode for powershell version if not set by parameter
		$STA = $TRUE
	}

	# escape escape sequences in version info
	$title = $title -replace "\\", "\\"
	$product = $product -replace "\\", "\\"
	$copyright = $copyright -replace "\\", "\\"
	$trademark = $trademark -replace "\\", "\\"
	$description = $description -replace "\\", "\\"
	$company = $company -replace "\\", "\\"

	if (![STRING]::IsNullOrEmpty($version)) {
		# check for correct version number information
		if ($version -notmatch "(^\d+\.\d+\.\d+\.\d+$)|(^\d+\.\d+\.\d+$)|(^\d+\.\d+$)|(^\d+$)") {
			Write-Error "Version number has to be supplied in the form n.n.n.n, n.n.n, n.n or n (with n as number)!"
			return
		}
	}

	Write-Output ""

	$type = ('System.Collections.Generic.Dictionary`2') -as "Type"
	$type = $type.MakeGenericType( @( ("System.String" -as "Type"), ("system.string" -as "Type") ) )
	$o = [Activator]::CreateInstance($type)
	$o.Add("CompilerVersion", "v4.0")

	$referenceAssembies = @("System.dll")
	if (!$noConsole) {
		if ([System.AppDomain]::CurrentDomain.GetAssemblies() | Where-Object { $_.ManifestModule.Name -ieq "Microsoft.PowerShell.ConsoleHost.dll" }) {
			$referenceAssembies += ([System.AppDomain]::CurrentDomain.GetAssemblies() | Where-Object { $_.ManifestModule.Name -ieq "Microsoft.PowerShell.ConsoleHost.dll" } | Select-Object -First 1).Location
		}
	}
	$referenceAssembies += ([System.AppDomain]::CurrentDomain.GetAssemblies() | Where-Object { $_.ManifestModule.Name -ieq "System.Management.Automation.dll" } | Select-Object -First 1).Location

	$n = New-Object System.Reflection.AssemblyName("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
	[System.AppDomain]::CurrentDomain.Load($n) | Out-Null
	$referenceAssembies += ([System.AppDomain]::CurrentDomain.GetAssemblies() | Where-Object { $_.ManifestModule.Name -ieq "System.Core.dll" } | Select-Object -First 1).Location

	if ($noConsole) {
		$n = New-Object System.Reflection.AssemblyName("System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
		[System.AppDomain]::CurrentDomain.Load($n) | Out-Null

		$n = New-Object System.Reflection.AssemblyName("System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")
		[System.AppDomain]::CurrentDomain.Load($n) | Out-Null

		$referenceAssembies += ([System.AppDomain]::CurrentDomain.GetAssemblies() | Where-Object { $_.ManifestModule.Name -ieq "System.Windows.Forms.dll" } | Select-Object -First 1).Location
		$referenceAssembies += ([System.AppDomain]::CurrentDomain.GetAssemblies() | Where-Object { $_.ManifestModule.Name -ieq "System.Drawing.dll" } | Select-Object -First 1).Location
	}

	$platform = "anycpu"
	if ($x64 -and !$x86) { $platform = "x64" } else { if ($x86 -and !$x64) { $platform = "x86" } }

	$cop = (New-Object Microsoft.CSharp.CSharpCodeProvider($o))
	$cp = New-Object System.CodeDom.Compiler.CompilerParameters($referenceAssembies, $outputFile)
	$cp.GenerateInMemory = $FALSE
	$cp.GenerateExecutable = $TRUE

	$manifestParam = ""
	if ($requireAdmin -or $DPIAware -or $supportOS -or $longPaths) {
		$manifestParam = "`"/win32manifest:$($outputFile+".win32manifest")`""
		$win32manifest = "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>`r`n<assembly xmlns=""urn:schemas-microsoft-com:asm.v1"" manifestVersion=""1.0"">`r`n"
		if ($DPIAware -or $longPaths) {
			$win32manifest += "<application xmlns=""urn:schemas-microsoft-com:asm.v3"">`r`n<windowsSettings>`r`n"
			if ($DPIAware) {
				$win32manifest += "<dpiAware xmlns=""http://schemas.microsoft.com/SMI/2005/WindowsSettings"">true</dpiAware>`r`n<dpiAwareness xmlns=""http://schemas.microsoft.com/SMI/2016/WindowsSettings"">PerMonitorV2</dpiAwareness>`r`n"
			}
			if ($longPaths) {
				$win32manifest += "<longPathAware xmlns=""http://schemas.microsoft.com/SMI/2016/WindowsSettings"">true</longPathAware>`r`n"
			}
			$win32manifest += "</windowsSettings>`r`n</application>`r`n"
		}
		if ($requireAdmin) {
			$win32manifest += "<trustInfo xmlns=""urn:schemas-microsoft-com:asm.v2"">`r`n<security>`r`n<requestedPrivileges xmlns=""urn:schemas-microsoft-com:asm.v3"">`r`n<requestedExecutionLevel level=""requireAdministrator"" uiAccess=""false""/>`r`n</requestedPrivileges>`r`n</security>`r`n</trustInfo>`r`n"
		}
		if ($supportOS) {
			$win32manifest += "<compatibility xmlns=""urn:schemas-microsoft-com:compatibility.v1"">`r`n<application>`r`n<supportedOS Id=""{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}""/>`r`n<supportedOS Id=""{1f676c76-80e1-4239-95bb-83d0f6d0da78}""/>`r`n<supportedOS Id=""{4a2f28e3-53b9-4441-ba9c-d69d4a4a6e38}""/>`r`n<supportedOS Id=""{35138b9a-5d96-4fbd-8e2d-a2440225f93a}""/>`r`n<supportedOS Id=""{e2011457-1546-43c5-a5fe-008deee3d3f0}""/>`r`n</application>`r`n</compatibility>`r`n"
		}
		$win32manifest += "</assembly>"
		$win32manifest | Set-Content ($outputFile + ".win32manifest") -Encoding UTF8
	}

	[string[]]$CompilerOptions = @();

	if (!([STRING]::IsNullOrEmpty($iconFile))) {
		$CompilerOptions += "`"/win32icon:$($iconFile)`""
	}

	if (!$virtualize) {
		$CompilerOptions += "/platform:$($platform)"
		$CompilerOptions += "/target:$( if ($noConsole){'winexe'}else{'exe'})"
		$CompilerOptions += "/target:$( if ($noConsole){'winexe'}else{'exe'})"
		$CompilerOptions += $manifestParam 
	}
	else {
		Write-Output "Application virtualization is activated, forcing x86 platfom."
		$CompilerOptions = "/platform:x86"
		$CompilerOptions = "/target:$( if ($noConsole) { 'winexe' } else { 'exe' } )"
		$CompilerOptions = "/nowin32manifest"
	}

	$cp.IncludeDebugInformation = $prepareDebug

	if ($prepareDebug) {
		$cp.TempFiles.KeepFiles = $TRUE
	}

	Write-Output "Reading input file $inputFile"
	$content = Get-Content -LiteralPath $inputFile -Encoding UTF8 -ErrorAction SilentlyContinue
	if ([STRING]::IsNullOrEmpty($content)) {
		Write-Error "No data found. May be read error or file protected."
		return
	}
	$scriptInp = [STRING]::Join("`r`n", $content)
	$script = [System.Convert]::ToBase64String(([System.Text.Encoding]::UTF8.GetBytes($scriptInp)))

	[string[]]$Constants = @(); 

	if($lcid) {
		$Constants += "culture"
	}
	if($noError) {
		$Constants += "noError"
	}
	if($noConsole) {
		$Constants += "noConsole"
	}
	if($noOutput) {
		$Constants += "noOutput"
	}
	if($version) {
		$Constants += "version"
	}
	if($credentialGUI) {
		$Constants += "credentialGUI"
	}
	if($noVisualStyles) {
		$Constants += "noVisualStyles"
	}
	if($exitOnCancel) {
		$Constants += "exitOnCancel"
	}
	if($STA) {
		$Constants += "STA"
	}
	if($MTA) {
		$Constants += "MTA"
	}
	if($UNICODEEncoding) {
		$Constants += "UNICODEEncoding"
	}

	$CompilerOptions += "/define:$([string]::Join(";", $Constants))";
	
	$cp.CompilerOptions = [string]::Join(" ", $CompilerOptions);
	
	Write-Verbose "Using Compiler Options: $($cp.CompilerOptions)"

	# Read Script file
	[string]$programFrame = [string]::Join("`n", (Get-Content (Join-Path $PSScriptRoot ./ps2exe.cs)));

	$programFrame = $programFrame.Replace("`$script", $script);
	$programFrame = $programFrame.Replace("`$lcid", $lcid);
	$programFrame = $programFrame.Replace("`$title", $title);
	$programFrame = $programFrame.Replace("`$product", $product);
	$programFrame = $programFrame.Replace("`$copyright", $copyright);
	$programFrame = $programFrame.Replace("`$trademark", $trademark);
	$programFrame = $programFrame.Replace("`$version", $version);
	$programFrame = $programFrame.Replace("`$description", $description);
	$programFrame = $programFrame.Replace("`$company", $company);


	$configFileForEXE3 = "<?xml version=""1.0"" encoding=""utf-8"" ?>`r`n<configuration><startup><supportedRuntime version=""v4.0"" sku="".NETFramework,Version=v4.0"" /></startup></configuration>"
	if ($longPaths) {
		$configFileForEXE3 = "<?xml version=""1.0"" encoding=""utf-8"" ?>`r`n<configuration><startup><supportedRuntime version=""v4.0"" sku="".NETFramework,Version=v4.0"" /></startup><runtime><AppContextSwitchOverrides value=""Switch.System.IO.UseLegacyPathHandling=false;Switch.System.IO.BlockLongPaths=false"" /></runtime></configuration>"
	}

	Write-Output "Compiling file...`n"
	$cr = $cop.CompileAssemblyFromSource($cp, $programFrame)
	if ($cr.Errors.Count -gt 0) {
		if (Test-Path $outputFile) {
			Remove-Item $outputFile -Verbose:$FALSE
		}
		Write-Error -ErrorAction Continue "Could not create the PowerShell .exe file because of compilation errors. Use -verbose parameter to see details."
		$cr.Errors | ForEach-Object { Write-Verbose $_ }
	}
	else {
		if (Test-Path $outputFile) {
			Write-Output "Output file $outputFile written"

			if ($prepareDebug) {
				$cr.TempFiles | Where-Object { $_ -ilike "*.cs" } | Select-Object -First 1 | ForEach-Object {
					$dstSrc = ([System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($outputFile), [System.IO.Path]::GetFileNameWithoutExtension($outputFile) + ".cs"))
					Write-Output "Source file name for debug copied: $($dstSrc)"
					Copy-Item -Path $_ -Destination $dstSrc -Force
				}
				$cr.TempFiles | Remove-Item -Verbose:$FALSE -Force -ErrorAction SilentlyContinue
			}
			if ($CFGFILE) {
				$configFileForEXE3 | Set-Content ($outputFile + ".config") -Encoding UTF8
				Write-Output "Config file for EXE created"
			}
		}
		else {
			Write-Error -ErrorAction "Continue" "Output file $outputFile not written"
		}
	}

	if ($requireAdmin -or $DPIAware -or $supportOS -or $longPaths) {
		if (Test-Path $($outputFile + ".win32manifest")) {
			Remove-Item $($outputFile + ".win32manifest") -Verbose:$FALSE
		}
	}
}
