using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;

namespace Part4Project
{
    public partial class Form1 : Form
    {
        TcpClient tcpclnt = new TcpClient();

        //global
        public byte[] b1 = { 8, 64, 0, 0, 237, 18, 0, 0 };
        public byte[] b3 = { 8, 64, 6, 0, 241, 2, 0, 0 };
        public byte[] b5 = { 8, 64, 1, 0, 252, 6, 0, 0 };
        public byte[] b7 = { 8, 64, 2, 0, 4, 4, 0, 0 };
        public byte[] b10 = { 8, 64, 3, 0, 155, 0, 0, 0 };
        public byte[] b12 = { 8, 64, 4, 0, 231, 50, 0, 0 };
        public byte[] b14 = { 8, 64, 5, 0, 236, 3, 1, 0 };


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //TcpClient tcpclnt = new TcpClient();
                statusBox.Text += "Connecting....\r\n";
                //get the ip address and the port
                String ipaddr = "192.168.10.12";
                int portaddr = 1122;
                tcpclnt.Connect(ipaddr, portaddr);
                statusBox.Text += "Connected\r\n";
                statusBox.Text += "You can start transmission now\r\n";
                //transferButton.Enabled = true;
                //initialiseToolStripMenuItem.Enabled = true;

                /*
                terminateToolStripMenuItem.Enabled = true;
                // enable jog.. see if it works
                moveXup.Enabled = true;
                moveXdown.Enabled = true;
                moveYup.Enabled = true;
                moveYdown.Enabled = true;
                moveZdown.Enabled = true;
                moveZup.Enabled = true;
                */

                //read the returning file
                NetworkStream stm = tcpclnt.GetStream();
                byte[] bc = new byte[1000];
                int k = stm.Read(bc, 0, 1000);
                for (int i = 0; i < k; i++)
                {
                    statusBox.Text += Convert.ToChar(bc[i]);
                }
                initialiseCNC();
                /*
                startToolStripMenuItem.Enabled = false;
                */
            }
            catch (Exception ex)
            {
                statusBox.Text += "Error \r\n" + ex.StackTrace;
            }
        }

        public void initialiseCNC()
        {
            //here i should send the files that is sent initially

            sendFile(b1);
            //sendFile(PappachanNC3.Properties.Resources.MILL1052);
            sendFile(b3);
            //sendFile(PappachanNC3.Properties.Resources.MILL105_ACC);
            sendFile(b5);
            //sendFile(PappachanNC3.Properties.Resources.MILL105);
            sendFile(b7);

            //sendFile(PappachanNC3.Properties.Resources.MILL1053);
            sendFile(b10);

            //sendFile(PappachanNC3.Properties.Resources.part11);
            sendFile(b12);
            //sendFile(PappachanNC3.Properties.Resources.PCMASCH0);
            sendFile(b14);
            //sendFile(PappachanNC3.Properties.Resources.Mill1051);
            //sendFile(PappachanNC3.Properties.Resources.lastpacket);

            statusBox.Text += "CNC Has Been Initialised..\r\n";
            //referenceToolStripMenuItem1.Enabled = true;
        }

        //sends the byte array to the remote location
        public void sendFile(byte[] bb)
        {
            //code here for sending files
            try
            {

                NetworkStream stm = tcpclnt.GetStream();
                tcpclnt.SendBufferSize = 512;
                tcpclnt.NoDelay = true;

                if (bb.Length > 512)
                {
                    int count = bb.Length / 512;
                    for (int i = 0; i <= count; i++)
                    {
                        int length = 0;
                        byte[] bb_temp = null;
                        if (i == count)
                        {
                            length = bb.Length - (i * 512);
                            bb_temp = new byte[length];
                        }
                        else
                        {
                            bb_temp = new byte[512];
                            length = 512;
                        }
                        Array.ConstrainedCopy(bb, i * 512, bb_temp, 0, length);
                        statusBox.Text += "Transmitting....Part" + i.ToString() + "\r\n";
                        stm.Write(bb_temp, 0, bb_temp.Length);
                    }
                }
                else
                {
                    statusBox.Text += "Transmitting...." + "\r\n";
                    stm.Write(bb, 0, bb.Length);

                }
            }
            catch (Exception ex)
            {
                statusBox.Text += "Error \r\n" + ex.StackTrace;
            }
        }

    }


    class locToHex
    {
        public byte[] hex = new byte[7];
        public locToHex(double loc, string axis)
        {
            hex = getLocBytes(loc, axis);
        }
        private byte[] getLocBytes(double loc, string axis)
        {
            byte[] tempbytes = new byte[7];
            double location = loc;
            double axisLocation = 0;
            switch (axis)
            {
                case "X":
                    //case for X axis
                    if (location >= 250)
                    {
                        axisLocation = (1.8014398509474E+13 * location) +
                       5.404318832268880E+16;
                    }
                    else
                    {
                        axisLocation = (3.60287970189636000E+13 * location) +
                       4.9539581489556700E+16;
                    }
                    break;
                case "Y":
                    //case for Y axis
                    if (location >= 63)
                    {
                        axisLocation = (7.205759403792790000E+13 * location) +
                       4.50360320208648000E+16;
                    }
                    else
                    {
                        axisLocation = (1.44115188075853000E+14 * location) +
                       4.05324687039287000E+16;
                    }
                    break;
                case "Z":
                    //case for Z axis
                    if (location >= 125)
                    {
                        axisLocation = (3.602879701896300E+13 * location) +
                       4.9539595901075600E+16;
                    }
                    else
                    {
                        axisLocation = (7.2057594037925000E+13 * location) +
                       4.5035996273705300E+16;
                    }
                    break;
                default:
                    //default is X
                    if (location >= 250)
                    {
                        axisLocation = (1.8014398509474E+13 * location) +
                       5.404318832268880E+16;

                    }
                    else
                    {
                        axisLocation = (3.60287970189636000E+13 * location) +
                       4.9539581489556700E+16;
                    }
                    break;
            }
            Int64 roundAxisLoc = Convert.ToInt64(axisLocation);
            string hexOutput = String.Format("{0:X}", roundAxisLoc);
            tempbytes[0] = Convert.ToByte(Convert.ToInt32(hexOutput.Substring(12, 2), 16));
            tempbytes[1] = Convert.ToByte(Convert.ToInt32(hexOutput.Substring(10, 2), 16));
            tempbytes[2] = Convert.ToByte(Convert.ToInt32(hexOutput.Substring(8, 2), 16));
            tempbytes[3] = Convert.ToByte(Convert.ToInt32(hexOutput.Substring(6, 2), 16));
            tempbytes[4] = Convert.ToByte(Convert.ToInt32(hexOutput.Substring(4, 2), 16));
            tempbytes[5] = Convert.ToByte(Convert.ToInt32(hexOutput.Substring(2, 2), 16));
            tempbytes[6] = Convert.ToByte(Convert.ToInt32(hexOutput.Substring(0, 2), 16));
            return tempbytes;
        }
    }
}


   