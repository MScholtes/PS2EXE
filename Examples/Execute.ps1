# Markus Scholtes, 2020
# Execute parameters and pipeline as powershell commands

if ($Args)
{ # arguments found, arguments are commands, pipeline elements are input
	$COMMAND = $Args -join ' '
	foreach ($ITEM in $INPUT)
	{ # build string out of pipeline (if any)
  	if ($PIPELINE)
  	{ $PIPELINE = "$PIPELINE,`"$ITEM`"" }
  	else
  	{ $PIPELINE = "`"$ITEM`"" }
	}
	if ($PIPELINE) { $COMMAND = "$PIPELINE|$COMMAND" }
}
else
{ # no arguments passed, pipeline elements are commands
	foreach ($ITEM in $INPUT)
	{ # build string out of pipeline (if any)
  	if ($COMMAND)
  	{ $COMMAND = "$COMMAND;$ITEM" }
  	else
  	{ $COMMAND = $ITEM }
	}
}

# execute the passed commands
if ($COMMAND)
{ Invoke-Expression $COMMAND | Out-String }
else
{ Write-Output "Pass PowerShell commands as parameters or in pipeline" }