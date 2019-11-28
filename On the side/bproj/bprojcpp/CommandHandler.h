/*
 * CommandHandler.h
 *
 *  Created on: Nov 28, 2019
 *      Author: Bas
 */

#ifndef COMMANDHANDLER_H_
#define COMMANDHANDLER_H_


#include <stdint.h>
#include <vector>
#include <map>
#include "callback.h"
#include "Command.h"



class CommandHandler
{
private:
	std::map<uint8_t, Callback<bool, std::vector<uint8_t>*>> commandList;


public:
	/*
	template<typename T>
	bool SetCommand(uint8_t cmd, T *instance, bool (T::*Method)(std::vector<uint8_t>* data))
	{
		if(cmd & 0x80)
			return false;

		if(commandList.count(cmd))
			return false;

		commandList[0].bind(instance, Method);
		return true;
	}

	bool SetCommand(uint8_t cmd, bool (*func)(std::vector<uint8_t>* data))
	{
		if(cmd & 0x80)
			return false;

		if(commandList.count(cmd))
			return false;

		commandList[0].bind(func);
		return true;
	}
	*/

	CommandHandler()
	{
		//SetCommand(0x00, this, &CommandHandler::TestCommand);
	}

	void ExecCommand(Command *cmd)
	{
		if(commandList.count(cmd->GetCommand()))
		{
			if(commandList[cmd->GetCommand()](&cmd->data))
				cmd->SetResponse(ResponseType::Ack);
			else
				cmd->SetResponse(ResponseType::Nack);
		}
		else
		{
			//Command not found in 'list', return unknown without data and the same seqnr.
			cmd->data.clear();
			cmd->SetResponse(ResponseType::Unknown);
		}
	}
};





#endif 
