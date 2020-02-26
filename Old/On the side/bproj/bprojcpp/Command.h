/*
 * Command.h
 *
 *  Created on: Nov 28, 2019
 *      Author: Bas
 */

#ifndef COMMAND_H_
#define COMMAND_H_

#include <vector>
#include <stdint.h>


enum class ResponseType
{
	Ack			= 0,
	Nack		= 1,
	Unknown		= 2,
	Overflow	= 3,
	NotAResp	= 255,
};


class Command
{
private:
	uint8_t _cmd;


public:
	uint8_t seqNo;
	std::vector<uint8_t> data;


	Command()
	{
		_cmd = 0;
		seqNo = 0;
	}

	Command(std::vector<uint8_t>* rawData)
	{
		auto it = rawData->begin();
		_cmd = *it++;
		seqNo = *it++;
		data = std::vector<uint8_t>(it, rawData->end());
	}

	void SetResponse(ResponseType response)
	{
		_cmd = 0x80 | (uint8_t)response;
	}

	void SetRequest(uint8_t cmd)
	{
		_cmd = 0x7F & cmd;
	}

	void ToFrame(std::vector<uint8_t>* rawData)
	{
		rawData->clear();
		*rawData = data;
		rawData->insert(rawData->begin(), seqNo);
		rawData->insert(rawData->begin(), _cmd);
	}

	uint8_t GetCommand()
	{
		return 0x7F & _cmd;
	}

	bool IsResponse()
	{
		return _cmd & 0x80;
	}

	ResponseType GetResponse()
	{
		if(_cmd & 0x80)
			return (ResponseType)(_cmd & 0x7F);
		else
			return ResponseType::NotAResp;
	}

};



#endif
