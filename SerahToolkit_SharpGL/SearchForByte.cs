namespace SerahToolkit_SharpGL
{
    internal static class SearchForByte
    {
        public static int ByteSearch(byte[] searchIn, byte[] searchBytes, int start = 0)
        {
            int found = -1;

            if (searchIn.Length <= 0 || searchBytes.Length <= 0 || start > (searchIn.Length - searchBytes.Length) ||
                searchIn.Length < searchBytes.Length) return found;
            for (int i = start; i <= searchIn.Length - searchBytes.Length; i++)
            {
                if (searchIn[i] != searchBytes[0]) continue;
                if (searchIn.Length > 1)
                {

                    bool matched = true;
                    for (int y = 1; y <= searchBytes.Length - 1; y++)
                    {
                        if (searchIn[i + y] != searchBytes[y])
                        {
                            matched = false;
                            break;
                        }
                    }

                    if (!matched) continue;
                    found = i;
                    break;
                }
                found = i;
                break;
            }
            return found;
        }
    }
}
