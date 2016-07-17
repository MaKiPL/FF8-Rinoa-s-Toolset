using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static System.IO.Path;

namespace SerahToolkit_SharpGL
{
    internal class ToSingleObj
    {
        private readonly string _path;
        private FileInfo[] _forged;

        private int _lastVt;
        private int _lastV;

        public ToSingleObj(string path, int howMany)
        {
            _path = path;
            ForgePath();
        }

        public void PerformSingleOBJ()
        {
            StringBuilder sb = new StringBuilder();
            foreach(FileInfo a in _forged)
            {
                if(a==_forged[0])
                {
                    string[] tempLinesBef = File.ReadAllLines(a.FullName);
                    for (int i = 0; i != tempLinesBef.Length; i++)
                    {
                        if (tempLinesBef[i].StartsWith("v "))
                                _lastV++;

                        if (tempLinesBef[i].StartsWith("vt"))
                                _lastVt++;

                    }
                    foreach (string s in tempLinesBef)
                        sb.AppendLine(s);
                }
                else
                {
                    string[] tempLines = File.ReadAllLines(a.FullName);
                    int maxVerts = 0;
                    int maxVt = 0;
                    for (int i = 0; i != tempLines.Length; i++)
                    {
                        if (tempLines[i].StartsWith("v "))
                            maxVerts++;
                        if (tempLines[i].StartsWith("vt"))
                            maxVt++;
                        if (tempLines[i].StartsWith("f"))
                        {
                            if (a.FullName.EndsWith("t.obj"))
                            {
                                string ss = tempLines[i].Substring(2, tempLines[i].Length - 2);
                                string[] temp = ss.Split('/');
                                int t1 = int.Parse(temp[0]) + _lastV;
                                string[] a1 = temp[1].Split(' ');
                                string[] a2 = temp[2].Split(' ');
                                int u1 = int.Parse(a1[0]) + _lastVt;
                                int t2 = int.Parse(a1[1]) + _lastV;
                                int u2 = int.Parse(a2[0]) + _lastVt;
                                int t3 = int.Parse(a2[1]) + _lastV;
                                int u3 = int.Parse(temp[3]) + _lastVt;
                                tempLines[i] = $"f {t1}/{u1} {t2}/{u2} {t3}/{u3}";
                            }
                            else
                            {
                                string ss = tempLines[i].Substring(2, tempLines[i].Length - 2);
                                string[] temp = ss.Split('/');
                                int t1 = int.Parse(temp[0]) + _lastV;
                                string[] a1 = temp[1].Split(' ');
                                string[] a2 = temp[2].Split(' ');
                                string[] a3 = temp[3].Split(' ');
                                int u1 = int.Parse(a1[0]) + _lastVt;
                                int t2 = int.Parse(a1[1]) + _lastV;
                                int u2 = int.Parse(a2[0]) + _lastVt;
                                int t3 = int.Parse(a2[1]) + _lastV;
                                int u3 = int.Parse(a3[0]) + _lastVt;
                                int t4 = int.Parse(a3[1]) + _lastV;
                                int u4 = int.Parse(temp[4]) + _lastVt;
                                tempLines[i] = $"f {t1}/{u1} {t2}/{u2} {t3}/{u3} {t4}/{u4}";
                            }
                        }
                    }
                    _lastV += maxVerts;
                    _lastVt += maxVt;
                    foreach (string s in tempLines)
                        sb.AppendLine(s);
                }
            }
            SaveFileDialog sfd = new SaveFileDialog {Filter = "Stage.obj|*.obj"};
            if (sfd.ShowDialog() != DialogResult.OK) return;
            if (File.Exists(sfd.FileName))
                File.Delete(sfd.FileName);
            File.WriteAllText(sfd.FileName, sb.ToString());
            string buildMtl = _path.Substring(0, _path.Length - 2) + ".MTL";
            string buildPng = _path.Substring(0, _path.Length - 2) + "_col.png";
            if(buildMtl!= GetDirectoryName(sfd.FileName) + @"\" + GetFileNameWithoutExtension(_path) + ".MTL")
                File.Copy(buildMtl, GetDirectoryName(sfd.FileName) + @"\" + GetFileNameWithoutExtension(_path) +  ".MTL",true);
            if(buildPng!= GetDirectoryName(sfd.FileName) + @"\" + GetFileNameWithoutExtension(_path) + "_col.png")
                File.Copy(buildPng, GetDirectoryName(sfd.FileName) + @"\" + GetFileNameWithoutExtension(_path) + "_col.png",true);
        }

        private void ForgePath()
        {
            DirectoryInfo di = new DirectoryInfo(GetDirectoryName(_path));
            _forged = di.GetFiles(GetFileNameWithoutExtension(_path) + "*.obj");
        }
    }
}
