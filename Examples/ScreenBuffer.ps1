# Example script for screen operations

function Get-CharFromConsolePosition([int]$X, [int]$Y)
{ # function to get the character of a position in the console buffer
  $RECT = New-Object System.Management.Automation.Host.Rectangle $X, $Y, $X, $Y
  $host.UI.RawUI.GetBufferContents($RECT)[0,0]
}


# fill block with a character
$BufferCell = New-Object System.Management.Automation.Host.BufferCell "O", "White", "Red", "Complete"
# Complete - The character occupies one BufferCell structure.
# Leading - The character occupies two BufferCell structures, with this cell being the leading cell (UNICODE)
# Trailing - The character occupies two BufferCell structures, with this cell being the trailing cell  (UNICODE)
$Source = New-Object System.Management.Automation.Host.Rectangle 10, 10, 29, 29

$host.UI.RawUI.SetBufferContents($Source, $BufferCell)


# read block into buffer
$ScreenBuffer = New-Object -TypeName 'System.Management.Automation.Host.BufferCell[,]' -ArgumentList ($Source.Bottom - $Source.Top + 1),($Source.Right - $Source.Left + 1)
$ScreenBuffer = $host.UI.RawUI.GetBufferContents($Source)


# modify block in buffer
$MAXDIMENSION = [Math]::Min(($Source.Bottom - $Source.Top + 1),($Source.Right - $Source.Left + 1))
for ($COUNTER = 0; $COUNTER -lt $MAXDIMENSION; $COUNTER++)
{
	$ScreenBuffer[$COUNTER,$COUNTER] = New-Object System.Management.Automation.Host.BufferCell "X", "White", "Red", "Complete"
	$ScreenBuffer[($MAXDIMENSION - $COUNTER - 1),$COUNTER] = New-Object System.Management.Automation.Host.BufferCell "X", "White", "Red", "Complete"
}


# write back buffer to screen
$host.UI.RawUI.SetBufferContents((New-Object System.Management.Automation.Host.Coordinates $Source.Left, $Source.Top), $ScreenBuffer)


# move block
# define fill character for source range
$BufferCell.Character = "-"
$BufferCell.ForegroundColor = $host.UI.RawUI.ForegroundColor
$BufferCell.BackgroundColor = $host.UI.RawUI.BackgroundColor
# define clipping area (a ten character wide border)
$Clip = New-Object System.Management.Automation.Host.Rectangle 10, 10, ($host.UI.RawUI.WindowSize.Width - 10), ($host.UI.RawUI.WindowSize.Height - 10)

# repeat ten times
for ($i = 1; $i -le 10; $i++)
{
	for ($X = $Source.Left + 1; $X -le ($host.UI.RawUI.WindowSize.Width - $Source.Right + $Source.Left); $X++)
	{
		$Destination = New-Object System.Management.Automation.Host.Coordinates $X, 10
		$host.UI.RawUI.ScrollBufferContents($Source, $Destination, $Clip, $BufferCell)
		$Source.Right++
		$Source.Left++
	}
	
	for ($Y = $Source.Top + 1; $Y -le ($host.UI.RawUI.WindowSize.Height - $Source.Bottom + $Source.Top); $Y++)
	{
		$Destination = New-Object System.Management.Automation.Host.Coordinates $Source.Left, $Y
		$host.UI.RawUI.ScrollBufferContents($Source, $Destination, $Clip, $BufferCell)
		$Source.Bottom++
		$Source.Top++
	}
	
	for ($X = $Source.Left - 1; $X -ge 10; $X--)
	{
		$Destination = New-Object System.Management.Automation.Host.Coordinates $X, $Source.Top
		$host.UI.RawUI.ScrollBufferContents($Source, $Destination, $Clip, $BufferCell)
		$Source.Right--
		$Source.Left--
	}
	
	for ($Y = $Source.Top - 1; $Y -ge 10; $Y--)
	{
		$Destination = New-Object System.Management.Automation.Host.Coordinates $Source.Left, $Y
		$host.UI.RawUI.ScrollBufferContents($Source, $Destination, $Clip, $BufferCell)
		$Source.Bottom--
		$Source.Top--
	}
}


# get character from screen
"Character at position (10/10): "
Get-CharFromConsolePosition 10 10
