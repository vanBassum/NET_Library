/*
 * Bytestuffing.h
 *
 *  Created on: Nov 27, 2019
 *      Author: Bas
 */

#ifndef BYTESTUFFING_H_
#define BYTESTUFFING_H_

#include <vector>
#include <stdint.h>
#include "callback.h"

#define SOF '&'
#define EOF '%'
#define ESC '\\'
#define NOP '*'

class ByteStuffing
{


	bool startFound = false;
	bool esc = false;

	std::vector<uint8_t> dataBuffer;

public:
	Callback<void, std::vector<uint8_t>*> FrameComplete;

	bool UnStuff(std::vector<uint8_t> *dataIn)
	{
		int len = dataIn->size();
		for(int i=0; i<len; i++)
		{
			bool record = false;
			if(esc)
			{
				record = true;
				esc = false;
			}
			else
			{
				switch((*dataIn)[i])
				{
				case ESC:
					esc = true;
					break;
				case SOF:
					startFound = true;
					dataBuffer.clear();
					break;
				case EOF:
					startFound = false;
					FrameComplete(&dataBuffer);
					break;
				case NOP:
					break;
				default:
					record = true;
					break;
				}
			}

			try
			{
				if(record && startFound)
					dataBuffer.push_back((*dataIn)[i]);
			}
			catch(std::bad_alloc const&)
			{
				return false;
			}
		}
		return true;
	}

	void Stuff(std::vector<uint8_t> *dataIn, std::vector<uint8_t> *dataOut)
	{
		int len = dataIn->size();
		dataOut->push_back(SOF); //Start of frame
		for(int i=0; i<len; i++)
		{
			if((*dataIn)[i] == SOF || (*dataIn)[i] == EOF || (*dataIn)[i] == ESC|| (*dataIn)[i] == NOP)
				dataOut->push_back(ESC);

			dataOut->push_back((*dataIn)[i]);
		}
		dataOut->push_back(EOF); //End of frame
	}
};


#endif
