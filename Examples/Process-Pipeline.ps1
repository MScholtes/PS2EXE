# Example script to process pipeline

# Type of pipeline object gets lost for compiled scripts, pipeline objects are always strings

[CmdletBinding()]
Param(
  [parameter(Mandatory=$FALSE, ValueFromPipeline=$TRUE)] [AllowEmptyString()]$Pipeline
)
BEGIN
{
	"Reading pipeline as array of strings"
	$COUNTER = 0
}
PROCESS
{
	if ($Pipeline -eq $NULL)
	{ Write-Output "No element found in the pipeline" }
	else
	{
		$COUNTER++
		Write-Output "$COUNTER`: $Pipeline"
	}
}
