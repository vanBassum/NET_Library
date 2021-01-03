using System;
using System.Collections.Generic;



namespace STDLib.JBVProtocol
{
    public class Sequencer
    {
        HashSet<UInt16> pendingSequences = new HashSet<ushort>();
        UInt16 nextSeq = 0;

        public UInt16 RequestSequenceID()
        {
            lock (pendingSequences)
            {
                while (pendingSequences.Contains(nextSeq++)) ;
                pendingSequences.Add(nextSeq);
            }
            return nextSeq;
        }

        public void FreeSequenceID(UInt16 seqId)
        {
            lock (pendingSequences)
            {
                pendingSequences.Remove(seqId);
            }
        }
    }
}


