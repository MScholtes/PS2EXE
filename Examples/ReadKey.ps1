"ReadKey-Demo`n`nWait for KeyDown event first, then for KeyUp-Event`n(only in KeyUp event modification keys are visible)"

$Host.UI.RawUI.ReadKey("IncludeKeyDown,NoEcho")

Read-Host "`nAfter pressing Enter there will a pause of two seconds before waing for the KeyUp event"
sleep 2

$Host.UI.RawUI.ReadKey("IncludeKeyUp")

if ($Host.UI.RawUI.KeyAvailable) { "Key in key buffer found" } else { "No key in key buffer found" }
