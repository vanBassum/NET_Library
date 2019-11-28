/*
 * Main.h
 *
 *  Created on: Nov 28, 2019
 *      Author: Bas
 */

#ifndef MAIN_H_
#define MAIN_H_

#include <stdint.h>
#include <vector>
#include <map>
#include "callback.h"
#include "ByteStuffing.h"
#include "command.h"


class BVProtocol
{
	ByteStuffing bs;
	std::map<uint8_t, Callback<void, Command>> pendingRequests;

	void bsFrameColledted(std::vector<uint8_t>* data)
	{
		//Bytestuffing has found a complete frame, whatever is in data its complete.
		Command cmd = Command(data);

		if(cmd.IsResponse())
		{
			//Well, we received a response to an earlier send command.
			pendingRequests[cmd.seqNo](cmd);
			pendingRequests.erase(cmd.seqNo);
		}
		else
		{
			//Execute the command.
			OnCommandRecieved(cmd);
		}
	}

	void Send(Command *cmd)
	{
		std::vector<uint8_t> buff;
		std::vector<uint8_t> buffb;

		cmd->ToFrame(&buff);
		bs.Stuff(&buff, &buffb);

		OnRawDataOut(&buffb);
	}

public:

	Callback<void, Command> OnCommandRecieved;
	Callback<void, std::vector<uint8_t>*> OnRawDataOut;

	BVProtocol()
	{
		bs.FrameComplete.bind(this, &BVProtocol::bsFrameColledted);
	}

	void RawDataIn(std::vector<uint8_t>* data)
	{
		//We recieved some data, use bytestuffing to handle this into complete frames.
		bs.UnStuff(data);
	}


	bool SendRequest(uint8_t cmdNo, std::vector<uint8_t>* data, void(*fp)(Command))
	{
		Command cmd;
        cmd.SetRequest(cmdNo);
        cmd.data = *data;

        for (cmd.seqNo = 0; cmd.seqNo < 255; cmd.seqNo++)
            if (!pendingRequests.count(cmd.seqNo))
                break;

        if (cmd.seqNo < 255)
        {
            pendingRequests[cmd.seqNo].bind(fp);
            Send(&cmd);

            return true;
        }
        else
            return false;
	}

	void SendResponse(ResponseType response, std::vector<uint8_t>* data)
	{
		Command cmd;
		cmd.SetResponse(response);
		cmd.data = *data;
		Send(&cmd);
	}


};




#endif
