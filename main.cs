//##################
//#  Script_WebTQ  #
//#       By       #
//# Slicksilver555 #
//##################

function WebTQ_Core::addClient(%this, %tcp)
{
	for(%i=0;%i<100;%i++)
	{
		if(isObject(%this.client[%i]))
			continue;
		%this.client[%i] = %tcp;
		%tcp.clientNum = %i;
		%this.connectedClients++;
		break;
	}
}

function WebTQ_Core::removeClient(%this, %tcp)
{
	%this.client[%tcp.clientNum] = 0;
	%this.connectedClients--;
	if(isObject(%this.client[%tcp.clientNum+1]))
	{
		%i = %tcp.clientNum;
		while(isObject(%this.client[%i++]))
		{
			%this.client[%i-1] = %this.client[%i];
		}
		%this.client[%i-1] = 0;
	}
}

function WebTQ_Core::onConnectionRequest(%this, %addr, %id)
{
	%this.addClient(
		new TCPObject( WebTQ_Client, %id) { 
			address = %addr;
			id = %id;
			host = %this;
		}
	);
}

function WebTQ_Core::onRequest(%this, %tcp, %req)
{
	echo(%tcp NL %req);
}

function WebTQ_Client::onDisconnect(%this)
{
	%this.host.removeClient(%this);
	%this.schedule(0,delete);
}

function WebTQ_Client::onLine(%this, %line)
{
	%this.host.onRequest(%this, %line);
}