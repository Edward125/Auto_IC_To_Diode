using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Edward;

namespace Auto_IC_To_Diode
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            txtICFile.SetWatermark("双击此处，选择IC文档");
            txtInFront.SetWatermark("输入零件前缀,1%,2%或者为空");
             //txtICFile.Text ="双击此处，选择IC文档";
        }

        #region 参数定义

        string currentFolder = string.Empty;//存放当前生成的文件夹路径
        string currentComplileFile = string.Empty;//存放当前compile file的 路径
        string _d_Test = @"diode 1.15, 213m, idc5.0m, co3.0, ar823m";
        string _d_S_bus = @"connect s to ";  //connect s to "#%RTC_AUX_S5"
        string _d_I_bus = @"connect i to ";  //connect i to "#%3D3V_AUX_S5"
        string _d_Comment_1 = string.Empty;
        string _d_Comment_2 = @"! DUT: nominal 500m, plus tol 60.0 %, minus tol 60.0 %
! DUT: high 1.15, low 200m
! TEST: high limit 1.15, low limit 213.355m
! Tolerance Multiplier 3.00
! Remote Sensing is Allowed
! Bias Current is 5";
        string _d_Comment_3 = @"! This test file is auto created by Auto_IC_To_Diode.exe,Author:edward_song@yeah.net";
        string IC_Name = string.Empty;
        List<string> IC_Pin = new List<string>();

        #endregion

        private void btnCreate_Click(object sender, EventArgs e)
        {
            //检查文件是否存在
            if (!File.Exists(txtICFile.Text.Trim()))
            {
                MessageBox.Show("指定的IC文件不存在，请重新选择");
                txtICFile.Focus();
                txtICFile.SelectAll();
                return;
            }

            byte[] file = global::Auto_IC_To_Diode.Properties.Resources.diode;
            string filename = Application.StartupPath + @"\diode";
            DownloadResouceFile(file, filename);
            file = global::Auto_IC_To_Diode.Properties.Resources.Complie;
            filename = Application.StartupPath + @"\Compile";
            DownloadResouceFile(file, filename);
            file = global::Auto_IC_To_Diode.Properties.Resources.lib;
            filename = Application.StartupPath +@"\lib";
            DownloadResouceFile(file, filename);
   

            StreamReader sr = new StreamReader(txtICFile.Text.Trim());
            string line = sr.ReadLine();
           // string lastICName = string.Empty;
            currentFolder = Application.StartupPath +@"\IC_Diode_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            currentComplileFile = currentFolder + @"\compile";
            createfolder(currentFolder);
            File.Copy (Application.StartupPath  + @"\Compile", currentComplileFile);
            
            while (line != null)
            {
               // MessageBox.Show(line);
                line = line.Trim();
                if (!line.Contains(".")) //find IC name 
                {
                    if (IC_Pin.Count != 0 && IC_Name  != string.Empty)
                    {
                        //ReWriteDiode(IC_Name, IC_Pin);
                        ReWriteDiode(IC_Name, txtInFront.Text.Trim(), IC_Pin);
                        ReWriteDiodeLib(IC_Pin);
                        IC_Pin.Clear();
                    }
                   // lastICName = line;
                    IC_Name = line;
                    
                }
                else
                {
                    string pin = line.Substring(line.LastIndexOf('.') + 1, line.Length - line.LastIndexOf('.') - 1);
                    if (pin.EndsWith(";"))
                        pin = pin.Replace(";", "");
                    //MessageBox.Show(pin);
                    IC_Pin.Add(pin);
                }       
               
                line = sr.ReadLine();
            }
            sr.Close();
            //
           // ReWriteDiode(IC_Name, IC_Pin);
            ReWriteDiode(IC_Name, txtInFront.Text.Trim(), IC_Pin);

            ReWriteDiodeLib(IC_Pin);
            MessageBox.Show ("Auto create diode complete,these files save in " + currentFolder  );
            IC_Name = string.Empty;
            IC_Pin.Clear();
            //delete file 
            File.Delete(Application.StartupPath + @"\diode");
            File.Delete(Application.StartupPath + @"\Compile");
            File.Delete(Application.StartupPath + @"\lib");
            OpenFolderAndSelectFile(currentComplileFile);
        }



        #region 释放资源文件
        private bool DownloadComplieResouceFile(string filename)
        {
            if (System.IO.File.Exists(filename))
                return true;
            //byte[] file = global::AutoCalculateATE_AFTE_NTF.Properties.Resources.sample;
            byte[] file = global::Auto_IC_To_Diode.Properties.Resources.Complie;
            try
            {
                System.IO.FileStream fsObj = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                fsObj.Write(file, 0, file.Length);
                fsObj.Close();
                //System.IO.FileInfo fi = new System.IO.FileInfo(filename);
                //fi.Attributes = System.IO.FileAttributes.Hidden;
                return true;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                return false;
            }


        }

        private bool DownloadDiodeResouceFile(string filename)
        {
            if (System.IO.File.Exists(filename))
                return true;
            //byte[] file = global::AutoCalculateATE_AFTE_NTF.Properties.Resources.sample;
            byte[] file = global::Auto_IC_To_Diode.Properties.Resources.diode;
            try
            {
                System.IO.FileStream fsObj = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                fsObj.Write(file, 0, file.Length);
                fsObj.Close();
                //System.IO.FileInfo fi = new System.IO.FileInfo(filename);
                //fi.Attributes = System.IO.FileAttributes.Hidden;
                return true;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                return false;
            }


        }

        private bool DownloadResouceFile(byte[] file, string filename)
        {
            if (System.IO.File.Exists(filename))
                return true;
            //byte[] file = global::AutoCalculateATE_AFTE_NTF.Properties.Resources.sample;
            //byte[] file = global::Auto_IC_To_Diode.Properties.Resources.diode;
            try
            {
                System.IO.FileStream fsObj = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                fsObj.Write(file, 0, file.Length);
                fsObj.Close();
                //System.IO.FileInfo fi = new System.IO.FileInfo(filename);
                //fi.Attributes = System.IO.FileAttributes.Hidden;
                return true;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                return false;
            }


        }
        #endregion

        #region openfile 

        private void OpenFile(TextBox textbox)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.InitialDirectory = Application.StartupPath;
            open.RestoreDirectory = false;
            if (open.ShowDialog() == DialogResult.OK)
                textbox.Text = open.FileName;
            else
            {
                textbox.Focus();
                textbox.SelectAll();
            }
        }
        #endregion

        private void txtICFile_DoubleClick(object sender, EventArgs e)
        {
            OpenFile(txtICFile);
        }


        #region rewriteDiode

        private void ReWriteDiode(string  icname ,List<string> list)
        {
            //===============
            int i = 0;
            int j = 0;        
           // foreach (string st in IC_Pin)
            foreach (string sbus in list )
            {
                i++;
               // foreach (string stt in IC_Pin)
                foreach (string ibus in list)
                {
                    j++;
                    if (i != j && i < j)
                    {
                        if (sbus != ibus && !sbus.EndsWith("NC") && !ibus.EndsWith("NC")) 
                        {
                            string file = Application.StartupPath + @"\diode";
                            string destfile = currentFolder  + @"\1%" + icname + "%cr" + i.ToString() + "-" + j.ToString();
                            //File.Copy(file, Application.StartupPath + @"\1%" + icname + "%cr" + i.ToString() + "-" + j.ToString());
                            File.Copy(file, destfile);
                            _d_S_bus = @"connect s to " + @"""" + @"#%" + sbus + @"""";
                            _d_I_bus = @"connect i to " + @"""" + @"#%" + ibus + @"""";
                            _d_Comment_1 = @"! " + @"""" + @"1%" + icname + "%cr" + i.ToString() + "-" + j.ToString() + @"""" + @"test.";
                            StreamWriter sw = new StreamWriter(destfile, true);
                            // string it = i.ToString() + ":" + j.ToString() + "->" + ibus + ":" + ibus;
                            sw.WriteLine(_d_S_bus);
                            sw.WriteLine(_d_I_bus);
                            sw.WriteLine(_d_Test);
                            sw.WriteLine(_d_Comment_1);
                            sw.WriteLine(_d_Comment_2);
                            sw.WriteLine(_d_Comment_3);
                            sw.Close();
                            // MessageBox.Show(it);
                            StreamWriter swCompile = new StreamWriter(currentComplileFile, true);
                            swCompile.WriteLine("compile " + @""""  + @"analog/" + @"1%" + icname + "%cr" + i.ToString() + "-" + j.ToString() + @"""");
                            swCompile.Close();
                        }
                    }
                }
                j = 0;
            }
            i = 0;
            //sw.Close();
            //===============
        }

        private void ReWriteDiode(string icname,string infornt, List<string> list)
        {
            //===============
            int i = 0;
            int j = 0;
            // foreach (string st in IC_Pin)
            foreach (string sbus in list)
            {
                i++;
                // foreach (string stt in IC_Pin)
                foreach (string ibus in list)
                {
                    j++;
                    if (i != j && i < j)
                    {
                        if (sbus != ibus && !sbus.EndsWith("NC") && !ibus.EndsWith("NC"))
                        {
                            string file = Application.StartupPath + @"\diode";
                            //string destfile = currentFolder + @"\1%" + icname + "%cr" + i.ToString() + "-" + j.ToString();
                            string destfile = currentFolder + @"\"+@infornt + icname + "%cr" + i.ToString() + "-" + j.ToString();
                            //File.Copy(file, Application.StartupPath + @"\1%" + icname + "%cr" + i.ToString() + "-" + j.ToString());
                            File.Copy(file, destfile);
                            if (!string.IsNullOrEmpty(infornt))
                            {
                                _d_S_bus = @"connect s to " + @"""" + @"#%" + sbus + @"""";
                                _d_I_bus = @"connect i to " + @"""" + @"#%" + ibus + @"""";
                            }
                            else
                            {
                                _d_S_bus = @"connect s to " + @""""  + sbus + @"""";
                                _d_I_bus = @"connect i to " + @""""  + ibus + @"""";
                            }
                           
                            _d_Comment_1 = @"! " + @"""" + @infornt + icname + "%cr" + i.ToString() + "-" + j.ToString() + @"""" + @"test.";
                            StreamWriter sw = new StreamWriter(destfile, true);
                            // string it = i.ToString() + ":" + j.ToString() + "->" + ibus + ":" + ibus;
                            sw.WriteLine(_d_S_bus);
                            sw.WriteLine(_d_I_bus);
                            sw.WriteLine(_d_Test);
                            sw.WriteLine(_d_Comment_1);
                            sw.WriteLine(_d_Comment_2);
                            sw.WriteLine(_d_Comment_3);
                            sw.Close();
                            // MessageBox.Show(it);
                            StreamWriter swCompile = new StreamWriter(currentComplileFile, true);
                            swCompile.WriteLine("compile " + @"""" + @"analog/" + @infornt+ icname + "%cr" + i.ToString() + "-" + j.ToString() + @"""");
                            swCompile.Close();
                        }
                    }
                }
                j = 0;
            }
            i = 0;
            //sw.Close();
            //===============
        }


        #endregion



        #region rewriteDiodeLib

        private void ReWriteDiodeLib( List<string> list)
        {
            string libsamplefile = Application.StartupPath + @"\lib";
            if (!File.Exists(libsamplefile))
                return;
            string libfolder = Application.StartupPath + @"\custom_lib";
            if (!Directory.Exists(libfolder))
                Directory.CreateDirectory(libfolder);
            //
            string destlibfile = libfolder + @"\" + getDiodeLibNname(list);
            if (File.Exists(destlibfile))
                return;
            File.Copy(libsamplefile, destlibfile +"_temp");             
            //===============
            int i = 0;
            int j = 0;        
           // foreach (string st in IC_Pin)
            foreach (string sbus in list)
            {
                i++;
                // foreach (string stt in IC_Pin)
                foreach (string ibus in list)
                {
                    j++;
                    if (i != j && i < j)
                    {
                        string st = @"diode " + @"""" + @"cr" + i.ToString() +j.ToString () +@"""" +@", 1.15, 200m, nr"+"\r\n";
                        StreamWriter sw = new StreamWriter(destlibfile+"_temp", true);
                        sw.Write(st);
                        sw.Close();
                    }
                }
                j = 0;
            }
            i = 0;
            //sw.Close();
            File.Copy(destlibfile + "_temp", destlibfile);           
            StreamWriter swLib = new StreamWriter(destlibfile, true);
            for (int k = 1; k <= list.Count; k++)           
            {
                StreamReader sr = new StreamReader(destlibfile + "_temp");
                string content = "\r\n" + @"external pins " + k.ToString() + "\r\n";
                swLib.WriteLine(content);
                content = string.Empty;
                string Line = sr.ReadLine();
                while (Line != null)
                {
                    if (Line.StartsWith("diode"))
                    {
                        int istart = Line.IndexOf(@"""");
                        int iend = Line.LastIndexOf(@"""");
                        string device = Line.Substring(istart + 1, iend - istart - 1);
                        int deviceStart = Convert.ToInt16(device.Substring(2, 1));
                        int deviceEnd = Convert.ToInt16(device.Substring(3, 1));
                        if (deviceStart == k)
                            content = @"   device " + @"""" + device + @" pins " + @"""" + @"C" + @"""" + "\r\n";
                        if (deviceEnd == k)
                            content = @"   device " + @"""" + device + @" pins " + @"""" + @"A" + @"""" + "\r\n";
                        if (!string.IsNullOrEmpty (content.Trim()))
                        swLib.WriteLine(content);
                        content = string.Empty;
                    }
                    Line = sr.ReadLine();
                }
                sr.Close();
            }            
            swLib.Close ();


            File.Delete(destlibfile + "_temp");

        }

        #endregion


        #region asc_chr

        /// <summary>
        /// convert the asc code to character
        /// </summary>
        /// <param name="asciiCode"></param>
        /// <returns></returns>
        public static string Chr(int asciiCode)
        {
            if (asciiCode >= 0 && asciiCode <= 255)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)asciiCode };
                string strCharacter = asciiEncoding.GetString(byteArray);
                return (strCharacter);
            }
            else
            {
                throw new Exception("ASCII Code is not valid.");
            }

        }

        /// <summary>
        /// convert the character to Asc code
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static int Asc(string character)
        {
            if (character.Length == 1)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                int intAsciiCode = (int)asciiEncoding.GetBytes(character)[0];
                return (intAsciiCode);
            }
            else
            {
                throw new Exception("Character is not valid.");
            }
        }




        private void createfolder(string folderpath)
        {
            if (!Directory.Exists(folderpath))
                Directory.CreateDirectory(folderpath);
        }
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Auto IC To Diode,Ver:" + Application.ProductVersion.ToString() + ",Edward";
        }

        #region getDiodeLibName


        private string getDiodeLibNname(List<string> list)
        {
            string name = "diode";

            if (list.Count > 1)
            {
                string allpin = list.Count.ToString();
                int  diodecout = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    diodecout = diodecout + i;
                }
                name = "diode_" + allpin + "p_" + diodecout.ToString() + "d";

            }
            return name;
        }

        #endregion

        private void OpenFolderAndSelectFile(String fileFullName)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
            psi.Arguments = "/e,/select," + fileFullName;
            System.Diagnostics.Process.Start(psi);
        }
    }
}
