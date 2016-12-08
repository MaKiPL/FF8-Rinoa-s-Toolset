using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace SerahToolkit_SharpGL
{
    /*
     * Handles the alternative GF texture
     * 
     * Getting real image:
     * offset = 0x14 - pointer to texture
pointerToTextureREAL = offset + UNKNOWN * 4
RealTexture = offset + pointerToTextureREAL

        How to draw it? x42 SKEW + 2       8 8 8    R G B (FAKE!)
*/

    class GF_AlternativeTexture
    {
        private string path;
        private FileStream fs;
        private BinaryReader br;

        public GF_AlternativeTexture(string path)
        {
            this.path = path;
        }

        internal bool Valid()
        {
            return false;
        }

        internal Bitmap DrawTexture()
        {
            return null;
        }
    }
}
