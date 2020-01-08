# demo program for Write-Progress

1..10 | % { Write-Progress -Activity "Activity $_" -Status "State $_" -Id 1 -CurrentOperation "Operation $_" -PercentComplete ([int]10*$_) -SecondsRemaining (10-$_) ;
	Start-Sleep 1 }

Start-Sleep 3
Write-Progress -Activity "Activity" -Status "State" -Id 1 -Completed
Write-Host "Completed"
Start-Sleep 1

Write-Progress -Activity "New progress" -Status "New state" -PercentComplete 33 -SecondsRemaining 734
Start-Sleep 3
Write-Output "Exiting program"
