using System;
using System.Text;

namespace SerahToolkit_SharpGL.RosettaStone
{
    internal class CharTableProvider
    {
        readonly string[] _charstable;
        private readonly string _chartable =
        @" , ,1,2,3,4,5,6,7,8,9,%,/,:,!,?,…,+,-,=,*,&,「,」,(,),·,.,,,~,“,”,‘,#,$,',_,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,À,Á,Â,Ä,Ç,È,É,Ê,Ë,Ì,Í,Î,Ï,Ñ,Ò,Ó,Ô,Ö,Ù,Ú,Û,Ü,Œ,ß,à,á,â,ä,ç,è,é,ê,ë,ì,í,î,ï,ñ,ò";

        public CharTableProvider()
        {
            _charstable = _chartable.Split(',');
        }

        public string[] Decipher(byte[] _in)
        {
            if (_in.Length == 0)
                return null;

            string[] process = new string[_in.Length];
            int index = 0;
            foreach(byte a in _in)
            {
                if(a-31>=0 && a<_charstable.Length)
                    process[index] = _charstable[a - 31];
                index++;
            }
            return process;
        }
        public string Decipher(string _in)
        {
            if (_in.Length == 0)
                return null;
            StringBuilder sb = new StringBuilder(_in.Length);
            foreach (byte a in _in)
            {
                if (a - 31 >= 0 && a < _charstable.Length)
                    sb.Append(_charstable[a - 31]);
            }
            return sb.ToString();
        }

        //broken
        //public string Cipher(string _in)
        //{
        //    if (_in.Length == 0)
        //        return null;
        //    StringBuilder sb = new StringBuilder(_in.Length);
        //    foreach (byte a in _in)
        //        sb.Append((byte)(LocateChar(a)+31));
        //    return sb.ToString();
        //}

        public byte[] Cipher(string _in)
        {
            if (_in.Length == 0)
                return null;
            byte[] buffer = new byte[_in.Length];
            for(int i=0; i!=buffer.Length; i++)
                buffer[i] = (byte)(LocateChar((byte)_in[i]) + 31);
            return buffer;
        }

        private uint LocateChar(byte a)
        {
            uint index = 0;
            while (true)
            {
                again:
                if (index >= _charstable.Length)
                    break;
                if (_charstable[index].Length == 0)
                {
                    index++;
                    goto again;
                }
                if ((char)a == _charstable[index][0])
                    return index;
                else index++;
            }
            return 0;
        }
    }
}
