# Markus Scholtes 2017
# Create examples for PS2EXE

$SCRIPTPATH = Split-Path $SCRIPT:MyInvocation.MyCommand.Path -parent
ls "$SCRIPTPATH\*.ps1" | %{
	Invoke-ps2exe "$($_.Fullname)" "$($_.Fullname -replace '.ps1','.exe')" -verbose
	Invoke-ps2exe "$($_.Fullname)" "$($_.Fullname -replace '.ps1','-GUI.exe')" -verbose -noConsole
}

Remove-Item "$SCRIPTPATH\BuildExamples*.exe*"
Remove-Item "$SCRIPTPATH\Progress.exe*"
Remove-Item "$SCRIPTPATH\ScreenBuffer-GUI.exe*"

$NULL = Read-Host "Press enter to exit"
