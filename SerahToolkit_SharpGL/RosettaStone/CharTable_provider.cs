using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerahToolkit_SharpGL.RosettaStone
{

    string[] charstable;

    class CharTable_provider
    {
        private string chartable =
        @" , ,1,2,3,4,5,6,7,8,9,%,/,:,!,?,…,+,-,=,*,&,「,」,(,),·,.,,,~,“,”,‘,#,$,',_,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,À,Á,Â,Ä,Ç,È,É,Ê,Ë,Ì,Í,Î,Ï,Ñ,Ò,Ó,Ô,Ö,Ù,Ú,Û,Ü,Œ,ß,à,á,â,ä,ç,è,é,ê,ë,ì,í,î,ï,ñ,ò";

        public CharTable_provider()
        {
            charstable = chartable.Split(',');
        }

        public string[] Decipher(byte[] _in)
        {
            if (_in.Length == 0)
                goto error;

            String[] Process = new string[_in.Length];
            int index = 0;
            foreach(var a in _in)
            {
                //UInt16 _P = BitConverter.ToUInt16(_in, index);
                if(a-31>=0 && a<charstable.Length)
                    Process[index] = charstable[a - 31];
                index++;
            }
            return Process;

            error:
            return null;
        }
    }
}
