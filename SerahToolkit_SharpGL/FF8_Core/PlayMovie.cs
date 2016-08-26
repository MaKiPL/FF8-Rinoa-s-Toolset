using System.Text;

namespace SerahToolkit_SharpGL.FF8_Core
{
    class PlayMovie
    {
        //static Video MP4Video;
        static float delay = 1.00f;
        public static bool isplaying;
        //static VideoPlayer VP;
        //public static Texture2D Frame;

        static public void Play_BINK(byte PAKfile, byte MovieID) //FF8 vanilla
        {
            //BIK?
        }

        /*static public void PlayMP4(byte MovieID)
        {
            delay = 1.00f;
            VP = new VideoPlayer();
            MP4Video = Singleton.contentManager.Load<Video>(BuildPath(MovieID));
            while (true)
            {
                if (delay < 0.0f && !isplaying)
                {
                    isplaying = true;
                    VP.Play(MP4Video);
                    break;
                }

                else
                    delay -= 0.05f;
            }

        }*/

        /*static public Texture2D GetFrame()
        {
            if (VP.State != MediaState.Stopped)
                return Frame = VP.GetTexture();
            else
                return null;
        }*/

        private static string BuildPath(byte MovieID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"Movies/");
            sb.Append("disc");
            sb.Append(MovieID <= 30 ? "00" :
                 MovieID > 30 && MovieID <= 30 + 34 ? "01" :
                 MovieID > 30 + 34 && MovieID <= 64 + 32 ? "02" :
                 MovieID > 64 + 32 && MovieID <= 64 + 32 + 7 ? "04" : "01");
            sb.Append(@"_");
            string temp;
            if (MovieID > 9)
                temp = MovieID.ToString();
            else
                temp = "0" + MovieID.ToString();
            sb.Append(temp + @"h");

            return sb.ToString();
        }

        /*public static void DrawMovie(GameTime gametime)
        {
            Singleton.spriteBatch.Begin();
            Singleton.spriteBatch.Draw(GetFrame(), new Rectangle(0, 0, Singleton.graphicsDevice.Viewport.Width, Singleton.graphicsDevice.Viewport.Height), Color.White);
            Singleton.spriteBatch.End();
        }*/
    }
}
