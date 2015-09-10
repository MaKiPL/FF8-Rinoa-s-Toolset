using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SerahToolkit_SharpGL
{
    class toSingleOBJ
    {
        private string path;
        private int HowMany;
        private FileInfo[] Forged;

        private int LastVT;
        private int LastV;

        public toSingleOBJ(string Path, int HowMany)
        {
            this.path = Path;
            this.HowMany = HowMany;
            ForgePath();
        }

        public void JustDoIt()
        {
            StringBuilder sb = new StringBuilder();
            int FileNumber = 0;
            foreach(FileInfo a in Forged)
            {
                if(a==Forged[0])
                {
                    String[] tempLinesBEF = File.ReadAllLines(a.FullName);
                    for (int i = 0; i != tempLinesBEF.Length; i++)
                    {
                        if (tempLinesBEF[i].StartsWith("v "))
                                LastV++;

                        if (tempLinesBEF[i].StartsWith("vt"))
                                LastVT++;

                    }
                    foreach (string s in tempLinesBEF)
                    {
                        sb.AppendLine(s);
                    }
                    FileNumber++;

                }
                else
                { 
                String[] tempLines = File.ReadAllLines(a.FullName);
                    int MaxVerts = 0;
                    int MaxVT = 0;
                    for (int i = 0; i != tempLines.Length; i++)
                    {
                        if (tempLines[i].StartsWith("v "))
                        {
                                MaxVerts++;
                        }

                        if (tempLines[i].StartsWith("vt"))
                        {
                                MaxVT++;
                        }
                        if (tempLines[i].StartsWith("f"))
                        {
                            if (a.FullName.EndsWith("t.obj"))
                            {
                                string ss = tempLines[i].Substring(2, tempLines[i].Length - 2);
                                string[] temp = ss.Split('/');

                                int T1 = int.Parse(temp[0]) + LastV;
                                string[] A1 = temp[1].Split(' ');
                                string[] A2 = temp[2].Split(' ');
                                int U1 = int.Parse(A1[0]) + LastVT;
                                int T2 = int.Parse(A1[1]) + LastV;
                                int U2 = int.Parse(A2[0]) + LastVT;
                                int T3 = int.Parse(A2[1]) + LastV;
                                int U3 = int.Parse(temp[3]) + LastVT;

                                tempLines[i] = string.Format("f {0}/{1} {2}/{3} {4}/{5}", T1, U1, T2, U2, T3, U3);
                            }
                            else
                            {
                                string ss = tempLines[i].Substring(2, tempLines[i].Length - 2);
                                string[] temp = ss.Split('/');

                                int T1 = int.Parse(temp[0]) + LastV;
                                string[] A1 = temp[1].Split(' ');
                                string[] A2 = temp[2].Split(' ');
                                string[] A3 = temp[3].Split(' ');
                                int U1 = int.Parse(A1[0]) + LastVT;
                                int T2 = int.Parse(A1[1]) + LastV;
                                int U2 = int.Parse(A2[0]) + LastVT;
                                int T3 = int.Parse(A2[1]) + LastV;
                                int U3 = int.Parse(A3[0]) + LastVT;
                                int T4 = int.Parse(A3[1]) + LastV;
                                int U4 = int.Parse(temp[4]) + LastVT;

                                tempLines[i] = string.Format("f {0}/{1} {2}/{3} {4}/{5} {6}/{7}", T1, U1, T2, U2, T3, U3, T4, U4);
                            }
                        }

                    }
                    LastV += MaxVerts;
                    LastVT += MaxVT;
                    FileNumber++;
                    foreach (string s in tempLines)
                        sb.AppendLine(s);
                }
            }

            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "Stage.obj|*.obj";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (File.Exists(sfd.FileName))
                    File.Delete(sfd.FileName);
                File.WriteAllText(sfd.FileName, sb.ToString());
                string buildMTL = path.Substring(0, path.Length - 2) + ".MTL";
                string buildPNG = path.Substring(0, path.Length - 2) + "_col.png";
                /*
                if (File.Exists(Path.GetDirectoryName(sfd.FileName) + @"\" + Path.GetFileNameWithoutExtension(path) + ".MTL"))
                    File.Delete(Path.GetDirectoryName(sfd.FileName) + @"\" + Path.GetFileNameWithoutExtension(path) + ".MTL");
                if (File.Exists(Path.GetDirectoryName(sfd.FileName) + @"\" + Path.GetFileNameWithoutExtension(path) + "_col.png"))
                    File.Delete(Path.GetDirectoryName(sfd.FileName) + @"\" + Path.GetFileNameWithoutExtension(path) + "_col.png");*/
                    if(buildMTL!= Path.GetDirectoryName(sfd.FileName) + @"\" + Path.GetFileNameWithoutExtension(path) + ".MTL")
                        File.Copy(buildMTL, Path.GetDirectoryName(sfd.FileName) + @"\" + Path.GetFileNameWithoutExtension(path) +  ".MTL",true);
                    if(buildPNG!= Path.GetDirectoryName(sfd.FileName) + @"\" + Path.GetFileNameWithoutExtension(path) + "_col.png")
                        File.Copy(buildPNG, Path.GetDirectoryName(sfd.FileName) + @"\" + Path.GetFileNameWithoutExtension(path) + "_col.png",true);
            }
        }

        private void ForgePath()
        {
            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(path));
            Forged = di.GetFiles(Path.GetFileNameWithoutExtension(path) + "*.obj");
        }
    }
}
