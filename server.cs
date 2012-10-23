//##################
//#  Script_WebTQ  #
//#       By       #
//# Slicksilver555 #
//##################

function initialize()
{
	%count = getFileCount("./extensions/*.cs");
	for(%i=0;%i<%count;%i++)
	{
		%file = findNextFile("./extensions/*.cs");
		%success = exec(%file);
		if(%success)
			%succ++;
	}
	echo("Successfully executed " @ %succ @ " extensions. " @ %count-%succ @ " failed.");
}
initialize();