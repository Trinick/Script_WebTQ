$WebTQ::Error::Jade::LevelFloodError = "You may only have 99 levels of indentation in Jade scripts";

$WebTQ::TabCharacter = "	";


function WebTQ_Jade::displayFile(%fileObject, %calledBy)
{
	%tab = $WebTQ::TabCharacter;
	%dataContainer = new scriptObject();
	%level = 0;
	while(!%fileObject.isEOF())
	{
		//%lastLevel = %level;
		if(strLen(%writeLine) > 0)
		{
			if((%pos = strPos(%writeLine, "#{")) >= 0)
			{
				%pos += 2;
				%endpos = strPos(%writeLine, "}", %pos);
				%code = getSubStr(%writeLine, %pos, %endpos-%pos);
				%return = eval("return " @ %code @ ";");
				%writeLine = strReplace(%writeLine, getSubStr(%writeLine, %pos-2, %endpos-(%pos-3)), %return);
			}
			%dataContainer.line[%dataContainer.lineCount++] = %writeLine;
			%writeLine = "";
		}
		%line = %fileObject.readLine();
		if(%line $= "")
		{
			%writeLine = " ";
			continue;
		}
		%level = 0;
		while(%level < 100)
			if(getSubStr(%line, %level, 1) $= %tab)
				%level++;
			else
				break;
		%line = getSubStr(%line, %level, strLen(%line)-%level);
		if(%level == 100)
		{
			error($WebTQ::Error::Jade::LevelFloodError);
			%writeLine = $WebTQ::Error::Jade::LevelFloodError;
			continue;
		}
		if(%level <= %lastLevel && %lastElement[%lastLevel] !$= "")
		{
			%difference = %lastLevel - %level;
			echo(%difference);
			for(%i = 0; %i <= %difference; %i++)
				%writeLine = %writeLine @ repeatCharacter(%tab, %lastLevel) @ "</" @ %lastElement[%lastLevel-%i] @ ">\n";
		}
//		if(%level < %lastLevel && %lastElement[%lastLevel] !$= "")
//			continue;
		if(getSubStr(%line, 0, 1) $= "|")
		{
			%writeLine = %writeLine @ repeatCharacter(%tab, %level) @ getSubStr(%line, 1, strLen(%line)-(%level-1)) @ "\n";
			echo(1);
			continue;
		}
		if(getSubStr(%line, 0, 1) $= ":")
		{
			%eval = getSubStr(%line, 1, strLen(%line)-1) @ "return \"\";";
			%return = eval(%eval);
			%writeLine = %writeLine @ %return;
			continue;
		}
		%element = getWord(%line,0);
		%args = "";
		%value = "";
		if((%pos = strPos(getWord(%line,0), "(")) > 0)
		{
			%endpos = strPos(%line, ")", %pos)-5;
			%args = getSubStr(%line, %pos+1, %endpos+1);
			%args = strReplace(%args, ",", "");
			%element = getSubStr(%line, 0, %pos);
			%rest = getSubStr(%line, %endpos+7, strLen(%line)-(%endpos+7));
			echo(%rest @ "...");
		}
		else
		{
			%element = getWord(%line, 0);
			%rest = getWords(%line, 1);
		}
		if(getSubStr(%element, strLen(%element)-1, 1) $= "=")
		{
			%value = eval("return " @ getWords(%line,1) @ ";");
			%element = getSubStr(%element, 0, strLen(%element)-1);
		}
		else if(getSubStr(%rest, 0, 1) $= "=")
			%value = eval("return " @ getWords(%rest, 1) @ ";");
		else if(getWordCount(%line) > 1)
			%value = %rest; //getWords(%line, 1);
		%lastElement[%level] = %element;
		if(%element $= "extends")
			if(getWords(%line,1) != %calledBy)
			{
				%cancel = 1;
				continue;
			}
			else
			{
				%donotcancel = 1;
			}
		if(%args !$= "")
			%args = " " @ %args;
		echo("ELE" SPC %element NL "ARGS" SPC %args NL "VALUE" SPC %value);
		%writeLine = %writeLine @ repeatCharacter(%tab, %level) @ "<" @ %element @ %args @ ">" @ %value;
		%lastLevel = %level;
		if(%cancel && !%donotcancel)
			%writeLine = "";
	}
	if(strLen(%writeLine) > 0)
	{
		if((%pos = strPos(%writeLine, "#{")) >= 0)
		{
			%pos += 2;
			%endpos = strPos(%writeLine, "}", %pos);
			%code = getSubStr(%writeLine, %pos, %endpos-%pos);
			%return = eval("return " @ %code @ ";");
			%writeLine = strReplace(%writeLine, getSubStr(%writeLine, %pos-2, %endpos-(%pos-3)), %return);
		}
		%dataContainer.line[%dataContainer.lineCount++] = %writeLine;
		%writeLine = "";
	}
	%writeLine = "";
	echo(%level);
	if(%level >= 0)
	{
		%difference = %level;
		for(%i = 0; %i <= %difference; %i++)
		{
			if(%lastElement[%level-%i] !$= "")
				%writeLine = %writeLine @ repeatCharacter(%tab, %lastLevel) @ "</" @ %lastElement[%level-%i] @ ">\n";
		}
	}
	%dataContainer.line[%dataContainer.lineCount++] = %writeLine;
	return %dataContainer;
}