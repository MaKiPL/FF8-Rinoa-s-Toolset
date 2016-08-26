using System;

//Ported from https://github.com/myst6re/qt-lzs/blob/master/lzs/LZS.cpp
namespace SerahToolkit_SharpGL.FF8_Core
{
    class LZSS
    {
        /// <summary>
        /// Decompiles LZSS. You have to know the OutputSize.
        /// </summary>
        /// <param name="data">buffer</param>
        /// <param name="fileSize">Original filesize of compressed file</param>
        /// <param name="size">Filesize of final file</param>
        /// <returns>Byte array</returns>
        public static byte[] DecompressAll(byte[] data, uint fileSize, int size)
        {
            var result = new byte[size];
            int curResult = 0;
            int curBuff = 4096 - 18, flagByte = 0;
            int fileData = 4,
            endFileData = (int)fileSize;


            var textBuf = new byte[4113];

            while (true)
            {
                if (((flagByte >>= 1) & 256) == 0)
                {
                    flagByte = data[fileData++] | 0xff00;
                }

                if (fileData >= endFileData)
                {
                    return result; // Ending
                }

                if ((flagByte & 1) > 0)
                {
                    result[curResult] = textBuf[curBuff] = data[fileData++];
                    curBuff = (curBuff + 1) & 4095;
                    ++curResult;
                }
                else
                {
                    if (fileData + 1 >= endFileData)
                        return result;
                    int offset = (byte)BitConverter.ToChar(data, fileData++);
                    if (fileData + 1 >= endFileData)
                        return result;
                    int length = (byte)BitConverter.ToChar(data, fileData++);
                    offset |= (length & 0xF0) << 4;
                    length = (length & 0xF) + 2 + offset;

                    int e;
                    for (e = offset; e <= length; e++)
                    {
                        textBuf[curBuff] = result[curResult] = textBuf[e & 4095];
                        curBuff = ((curBuff + 1) & 4095);
                        ++curResult;

                    }
                }
            }
        }
    }
}