using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using dllinject;
using System.IO;

namespace zm
{
    public partial class Form1 : Form
    {
        [Flags]
        private enum KeyStates
        {
            None = 0,
            Down = 1,
            Toggled = 2
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetKeyState(int keyCode);

        private static KeyStates GetKeyState(Keys key)
        {
            KeyStates state = KeyStates.None;

            short retVal = GetKeyState((int)key);

            //If the high-order bit is 1, the key is down
            //otherwise, it is up.
            if ((retVal & 0x8000) == 0x8000)
                state |= KeyStates.Down;

            //If the low-order bit is 1, the key is toggled.
            if ((retVal & 1) == 1)
                state |= KeyStates.Toggled;

            return state;
        }

        public static bool IsKeyDown(Keys key)
        {
            return KeyStates.Down == (GetKeyState(key) & KeyStates.Down);
        }

        public static bool IsKeyToggled(Keys key)
        {
            return KeyStates.Toggled == (GetKeyState(key) & KeyStates.Toggled);
        }


        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        memory m = new memory();
        
        float X, Y, Z, X_p2, Y_p2, Z_p2, X_p3, Y_p3, Z_p3, X_p4, Y_p4, Z_p4;
        //float savex, savey, savez, savex2, savey2, savez2;
        float undox, undoy, undoz, undox2, undoy2, undoz2, undox3, undoy3, undoz3, undox4, undoy4, undoz4;
        float undox_p2, undoy_p2, undoz_p2, undox_p3, undoy_p3, undoz_p3, undox_p4, undoy_p4, undoz_p4;
        float[] savex = new float[10];
        float[] savey = new float[10];
        float[] savez = new float[10];
        
        float[] X_z = new float[address.posz_X.Length];
        float[] Y_z = new float[address.posz_X.Length];
        float[] Z_z = new float[address.posz_X.Length];

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            address.findadd("money_p1");
            m.WriteInt32(address.soldi, Convert.ToInt32(moneyval.Text));
            //MessageBox.Show(address.soldi.ToString("X"));
        }

        int basea = 0;
        
        private void timer1_Tick(object sender, EventArgs e)
        {        
            if(m.IsOpen())
            {
                label1.Text = "Game Found";
                label1.ForeColor = Color.Blue;
                process.Interval = 500;
                if(pos.Enabled==false)
                {
                    pos.Start();
                }
                if (basea == 0)
                {
                    address.setadd();
                    basea = 1;
                }
            }
            else
            {              
                m.AttackProcess("s2_mp64_ship");
                label1.Text = "Game not Found";
                label1.ForeColor = Color.Red;
                process.Interval = 100;
                basea = 0;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked==true)
            {
                vita.Start();
                checkBox4.Checked = false;
                
            }
            else
            {
                vita.Stop();
                m.WriteInt32(address.vita, 100);
            }
        }

        private void vita_Tick(object sender, EventArgs e)
        {
            //m.WriteInt32(address.vita, 2147483647);
            m.WriteInt32(address.vita, 10000);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            //address.findadd("ammo");
            //address.setadd();
            //MessageBox.Show(address.main_pattern.ToString("X"));
            if(checkBox2.Checked==true)
            {
                ammo.Start();
            }
            else
            {
                ammo.Stop();
                for(int i=0;i<address.ammo.Length;i++)
                {
                    m.WriteInt32(address.ammo[i], 10);
                }
            }
        }

        private void ammo_Tick(object sender, EventArgs e)
        {
            ammo.Interval = 1000;
            for (int i = 0; i < address.ammo.Length; i++)
            {
                m.WriteInt32(address.ammo[i], 65535);
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            //address.findadd("rapid");
            if(checkBox3.Checked==true)
            {
                //m.WriteXBytes(address.rapid_test, new byte []{ 0xE9, 0xF4, 0x0E, 0xB8, 0xFF, 0x90, 0x90, 0x49 });
                rapid.Start();
            }
            else
            {
                //m.WriteXBytes(address.rapid_test, new byte[] { 0x8B, 0x8C, 0xB3, 0xC8, 0x03, 0x00, 0x00, 0x49 });
                rapid.Stop();
            }
        }

        private void rapid_Tick(object sender, EventArgs e)
        {
            m.WriteInt32(address.rapid, 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void pos_Tick(object sender, EventArgs e)
        {
            X = m.ReadFloat(address.X);
            Y = m.ReadFloat(address.Y);
            Z = m.ReadFloat(address.Z);

            X_p2 = m.ReadFloat(address.p2.X);
            Y_p2 = m.ReadFloat(address.p2.Y);
            Z_p2 = m.ReadFloat(address.p2.Z);

            X_p3 = m.ReadFloat(address.p3.X);
            Y_p3 = m.ReadFloat(address.p3.Y);
            Z_p3 = m.ReadFloat(address.p3.Z);

            X_p4 = m.ReadFloat(address.p4.X);
            Y_p4 = m.ReadFloat(address.p4.Y);
            Z_p4 = m.ReadFloat(address.p4.Z);

            label3.Text = "X: " + X.ToString();
            label4.Text = "Y: " + Y.ToString();
            label5.Text = "Z: " + Z.ToString();
            label2.Text = m.ReadInt32(address.curweap).ToString();

            /*tabControl1.TabPages[2].Text = "Player 2 Hacks - " + m.ReadString(address.p2.nome, 32);
            tabControl1.TabPages[3].Text = "Player 3 Hacks - " + m.ReadString(address.p3.nome, 32);
            tabControl1.TabPages[4].Text = "Player 4 Hacks - " + m.ReadString(address.p4.nome, 32);*/
            if (checkBox5.Checked == true)
            {
                if (IsKeyDown(Keys.NumPad8))
                {
                    m.WriteFloat(address.X, X + 30);
                }
                if (IsKeyDown(Keys.NumPad2))
                {
                    m.WriteFloat(address.X, X - 30);
                }
                if (IsKeyDown(Keys.NumPad4))
                {
                    m.WriteFloat(address.Y, Y + 30);
                }
                if (IsKeyDown(Keys.NumPad6))
                {
                    m.WriteFloat(address.Y, Y - 30);
                }
                if (IsKeyDown(Keys.NumPad9))
                {
                    m.WriteFloat(address.Z, Z + 50);
                }
                if (IsKeyDown(Keys.NumPad3))
                {
                    m.WriteFloat(address.Z, Z - 50);
                }


            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int t = 0;
            switch(comboBox1.Text)
            {
                case "Pos 1":
                    savex[0] = X;
                    savey[0] = Y;
                    savez[0] = Z;
                    break;
                case "Pos 2":
                    savex[1] = X;
                    savey[1] = Y;
                    savez[1] = Z;
                    break;
                case "Pos 3":
                    savex[2] = X;
                    savey[2] = Y;
                    savez[2] = Z;
                    break;
                case "Pos 4":
                    savex[3] = X;
                    savey[3] = Y;
                    savez[3] = Z;
                    break;
                case "Pos 5":
                    savex[4] = X;
                    savey[4] = Y;
                    savez[4] = Z;
                    break;
                case "Pos 6":
                    savex[5] = X;
                    savey[5] = Y;
                    savez[5] = Z;
                    break;
                case "Pos 7":
                    savex[6] = X;
                    savey[6] = Y;
                    savez[6] = Z;
                    break;
                case "Pos 8":
                    savex[7] = X;
                    savey[7] = Y;
                    savez[7] = Z;
                    break;
                case "Pos 9":
                    savex[8] = X;
                    savey[8] = Y;
                    savez[8] = Z;
                    break;
                case "Pos 10":
                    savex[9] = X;
                    savey[9] = Y;
                    savez[9] = Z;
                    break;
                default: t = 1; break;
            }
            if (t == 0)
                button5.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int t = 0;
            switch (comboBox1.Text)
            {
                case "Pos 1":
                    if (savex[0] != 0 && savey[0] != 0 && savez[0] != 0)
                    {
                        m.WriteFloat(address.X, savex[0]);
                        m.WriteFloat(address.Y, savey[0]);
                        m.WriteFloat(address.Z, savez[0]);
                    }
                    else
                        t = 1;
                    break;
                case "Pos 2":
                    if (savex[1] != 0 && savey[1] != 0 && savez[1] != 0)
                    {
                        m.WriteFloat(address.X, savex[1]);
                        m.WriteFloat(address.Y, savey[1]);
                        m.WriteFloat(address.Z, savez[1]);
                    }
                    else
                        t = 1;
                    break;
                case "Pos 3":
                    if (savex[2] != 0 && savey[2] != 0 && savez[2] != 0)
                    {
                        m.WriteFloat(address.X, savex[2]);
                        m.WriteFloat(address.Y, savey[2]);
                        m.WriteFloat(address.Z, savez[2]);
                    }
                    else
                        t = 1;
                    break;
                case "Pos 4":
                    if (savex[3] != 0 && savey[3] != 0 && savez[3] != 0)
                    {
                        m.WriteFloat(address.X, savex[3]);
                        m.WriteFloat(address.Y, savey[3]);
                        m.WriteFloat(address.Z, savez[3]);
                    }
                    else
                        t = 1;
                    break;
                case "Pos 5":
                    if (savex[4] != 0 && savey[4] != 0 && savez[4] != 0)
                    {
                        m.WriteFloat(address.X, savex[4]);
                        m.WriteFloat(address.Y, savey[4]);
                        m.WriteFloat(address.Z, savez[4]);
                    }
                    else
                        t = 1;
                    break;
                case "Pos 6":
                    if (savex[5] != 0 && savey[5] != 0 && savez[5] != 0)
                    {
                        m.WriteFloat(address.X, savex[5]);
                        m.WriteFloat(address.Y, savey[5]);
                        m.WriteFloat(address.Z, savez[5]);
                    }
                    else
                        t = 1;
                    break;
                case "Pos 7":
                    if (savex[6] != 0 && savey[6] != 0 && savez[6] != 0)
                    {
                        m.WriteFloat(address.X, savex[6]);
                        m.WriteFloat(address.Y, savey[6]);
                        m.WriteFloat(address.Z, savez[6]);
                    }
                    else
                        t = 1;
                    break;
                case "Pos 8":
                    if (savex[7] != 0 && savey[7] != 0 && savez[7] != 0)
                    {
                        m.WriteFloat(address.X, savex[7]);
                        m.WriteFloat(address.Y, savey[7]);
                        m.WriteFloat(address.Z, savez[7]);
                    }
                    else
                        t = 1;
                    break;
                case "Pos 9":
                    if (savex[8] != 0 && savey[8] != 0 && savez[8] != 0)
                    {
                        m.WriteFloat(address.X, savex[8]);
                        m.WriteFloat(address.Y, savey[8]);
                        m.WriteFloat(address.Z, savez[8]);
                    }
                    else
                        t = 1;
                    break;
                case "Pos 10":
                    if (savex[9] != 0 && savey[9] != 0 && savez[9] != 0)
                    {
                        m.WriteFloat(address.X, savex[9]);
                        m.WriteFloat(address.Y, savey[9]);
                        m.WriteFloat(address.Z, savez[9]);
                    }
                    else
                        t = 1;
                    break;
                default: t = 1; break;
            }
            if (t == 0)
            {
                undox = X;
                undoy = Y;
                undoz = Z;
                button6.Enabled = true;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            m.WriteFloat(address.X, undox);
            m.WriteFloat(address.Y, undoy);
            m.WriteFloat(address.Z, undoz);
            button6.Enabled = false;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox4.Checked==true)
            {
                checkBox1.Checked = false;
                m.WriteNOP(address.vita);
            }
            else
            {
                m.WriteInt32(address.vita, 100);
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox5.Checked==true)
            {
                label6.Visible = true;
                label7.Visible = true;
            }
            else
            {
                label6.Visible = false;
                label7.Visible = false;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
        }

        private void button12_Click(object sender, EventArgs e)
        {
            MessageBox.Show("By MisterY" + "\n" + "Game Version: 1.22.223", "About");
        }

        static System.Threading.Mutex singleton = new Mutex(true, "zm_1_22");

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.WindowsShutDown) return;
            if (singleton.WaitOne(TimeSpan.Zero, true))
            {
                
            }
        }
      
        private void Form1_Load(object sender, EventArgs e)
        {
            if (!singleton.WaitOne(TimeSpan.Zero, true))
            {
                MessageBox.Show("Program already running");
                Application.Exit();
            }

            this.Text = "WWII Zombie Trainer 1.1.2";
            toolTip1.SetToolTip(checkBox6, "Freeze money");
            toolTip1.SetToolTip(numericX, "X");
            toolTip1.SetToolTip(numericY, "Y");
            toolTip1.SetToolTip(numericZ, "Z");
            toolTip1.SetToolTip(checkBox9, "Freeze zombies' health");
            toolTip1.SetToolTip(label2, "Current weapon ID");
            moneyval.Maximum = int.MaxValue;
            moneyval.Minimum = 0;
            weaponval.Maximum = int.MaxValue;
            weaponval.Minimum = 0;
            moneyval_p2.Maximum = int.MaxValue;
            moneyval_p2.Minimum = 0;
            weaponval_p2.Maximum = int.MaxValue;
            weaponval_p2.Minimum = 0;
            moneyval_p3.Maximum = int.MaxValue;
            moneyval_p3.Minimum = 0;
            weaponval_p3.Maximum = int.MaxValue;
            weaponval_p3.Minimum = 0;
            moneyval_p4.Maximum = int.MaxValue;
            moneyval_p4.Minimum = 0;
            weaponval_p4.Maximum = int.MaxValue;
            weaponval_p4.Minimum = 0;
            roundval.Maximum = int.MaxValue;
            roundval.Minimum = 0;
            zhval.Maximum = int.MaxValue;
            zhval.Minimum = 0;
            numericX.Maximum = decimal.MaxValue;
            numericX.Minimum = decimal.MinValue;
            numericY.Maximum = decimal.MaxValue;
            numericY.Minimum = decimal.MinValue;
            numericZ.Maximum = decimal.MaxValue;
            numericZ.Minimum = decimal.MinValue;
            jumpval.Maximum = decimal.MaxValue;
            jumpval.Minimum = 0;
        }



        private void button13_Click(object sender, EventArgs e)
        {
            undox3 = m.ReadFloat(address.X);
            undoy3 = m.ReadFloat(address.Y);
            undoz3 = m.ReadFloat(address.Z);
            m.WriteFloat(address.X, Convert.ToSingle(numericX.Text));
            m.WriteFloat(address.Y, Convert.ToSingle(numericY.Text));
            m.WriteFloat(address.Z, Convert.ToSingle(numericZ.Text));
            button17.Enabled = true;
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        //int tick = 0;
        private void money_Tick(object sender, EventArgs e)
        {        
            m.WriteInt32(address.soldi, Convert.ToInt32(moneyval.Text));
            /*tick++;
            if (tick == 100)
            {
                address.findadd("money");
                tick = 0;
            }*/
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox6.Checked)
            {
                address.findadd("money_p1");
                money.Start();
            }
            else
            {
                money.Stop();
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
        }


        private void zhealth_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < address.vz.Length / 2; i++)
            {
                m.WriteInt32(address.vz[i], Convert.ToInt32(zhval.Text));
            }
        }

        private void zhealth2_Tick(object sender, EventArgs e)
        {
            for (int i = address.vz.Length / 2; i < address.vz.Length; i++)
            {
                m.WriteInt32(address.vz[i], Convert.ToInt32(zhval.Text));
            }
        }

        private void button36_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            m.WriteFloat(address.jump, Convert.ToSingle(jumpval.Text));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(radioButton1.Checked)
            {
                m.WriteInt32(address.give1, Convert.ToInt32(weaponval.Text));
                m.WriteInt32(address.ammo_init[0], Convert.ToInt32(weaponval.Text));
            }
            else if (radioButton2.Checked)
            {
                m.WriteInt32(address.give2, Convert.ToInt32(weaponval.Text));
                m.WriteInt32(address.ammo_init[2], Convert.ToInt32(weaponval.Text));
            }
            else
            {
                m.WriteInt32(address.give3, Convert.ToInt32(weaponval.Text));
                m.WriteInt32(address.ammo_init[3], Convert.ToInt32(weaponval.Text));
            }
        }

        private void checkBox7_CheckedChanged_1(object sender, EventArgs e)
        {
            if(checkBox7.Checked)
            {
                special.Start();
            }
            else
            {
                special.Stop();
                m.WriteFloat(address.special, 0);
            }
        }

        private void special_Tick(object sender, EventArgs e)
        {
            m.WriteFloat(address.special, 1);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            for(int i=0;i<address.vz.Length;i++)
            {
                m.WriteInt32(address.vz[i], Convert.ToInt32(zhval.Text));
            }
        }

        private void checkBox9_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox9.Checked)
            {
                zhealth.Start();
                zhealth2.Start();
            }
            else
            {
                zhealth.Stop();
                zhealth2.Stop();
            }
        }

        private void checkBox43_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox43.Checked)
            {
                for (int i=0;i<address.posz_X.Length;i++)
                {
                    X_z[i] = m.ReadFloat(address.posz_X[i]);
                    Y_z[i] = m.ReadFloat(address.posz_Y[i]);
                    Z_z[i] = m.ReadFloat(address.posz_Z[i]);
                }
                freeze_z.Start();
                freeze_z2.Start();
                freeze_z3.Start();
                freeze_z4.Start();
                freeze_z5.Start();
                freeze_z6.Start();
                freeze_z7.Start();
            }
            else
            {
                freeze_z.Stop();
                freeze_z2.Stop();
                freeze_z3.Stop();
                freeze_z4.Stop();
                freeze_z5.Stop();
                freeze_z6.Stop();
                freeze_z7.Stop();
            }
        }

        private void freeze_z_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                m.WriteFloat(address.posz_X[i], X_z[i]);
                m.WriteFloat(address.posz_Y[i], Y_z[i]);
                m.WriteFloat(address.posz_Z[i], Z_z[i]);
            }
        }

        private void freeze_z2_Tick(object sender, EventArgs e)
        {
            for (int i = 3; i < 6; i++)
            {
                m.WriteFloat(address.posz_X[i], X_z[i]);
                m.WriteFloat(address.posz_Y[i], Y_z[i]);
                m.WriteFloat(address.posz_Z[i], Z_z[i]);
            }
        }

        private void freeze_z3_Tick(object sender, EventArgs e)
        {
            for (int i = 6; i < 9; i++)
            {
                m.WriteFloat(address.posz_X[i], X_z[i]);
                m.WriteFloat(address.posz_Y[i], Y_z[i]);
                m.WriteFloat(address.posz_Z[i], Z_z[i]);
            }
        }

        private void freeze_z4_Tick(object sender, EventArgs e)
        {
            for (int i = 9; i < 12; i++)
            {
                m.WriteFloat(address.posz_X[i], X_z[i]);
                m.WriteFloat(address.posz_Y[i], Y_z[i]);
                m.WriteFloat(address.posz_Z[i], Z_z[i]);
            }
        }

        private void freeze_z5_Tick(object sender, EventArgs e)
        {
            for (int i = 12; i < 15; i++)
            {
                m.WriteFloat(address.posz_X[i], X_z[i]);
                m.WriteFloat(address.posz_Y[i], Y_z[i]);
                m.WriteFloat(address.posz_Z[i], Z_z[i]);
            }

        }

        private void freeze_z6_Tick(object sender, EventArgs e)
        {
            for (int i = 15; i < 18; i++)
            {
                m.WriteFloat(address.posz_X[i], X_z[i]);
                m.WriteFloat(address.posz_Y[i], Y_z[i]);
                m.WriteFloat(address.posz_Z[i], Z_z[i]);
            }
        }

        private void freeze_z7_Tick(object sender, EventArgs e)
        {
            for (int i = 18; i < 21; i++)
            {
                m.WriteFloat(address.posz_X[i], X_z[i]);
                m.WriteFloat(address.posz_Y[i], Y_z[i]);
                m.WriteFloat(address.posz_Z[i], Z_z[i]);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int k = 0;
            if (checkBox43.Checked)
            {
                checkBox43.Checked = false;
                k = 1;
            }
            for(int i=0;i<address.posz_X.Length;i++)
            {
                m.WriteFloat(address.posz_X[i], X + 50);
                m.WriteFloat(address.posz_Y[i], Y + 50);
                m.WriteFloat(address.posz_Z[i], Z);
            }
            if (k == 1)
            {
                checkBox43.Checked = true;
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            m.WriteFloat(address.timescale, Convert.ToSingle(timescaleval.Text));
        }

        private void button8_Click(object sender, EventArgs e)
        {
            address.findadd("round");
            if(m.ReadInt32(address.round)<Convert.ToInt32(roundval.Text))
            {
                m.WriteInt32(address.round, Convert.ToInt32(roundval.Text));
                toolTip1.SetToolTip(roundval, "");
            }
            else
            {
                toolTip1.SetToolTip(roundval, "You can only increase the round");
            }
        }

        private void addr_Tick(object sender, EventArgs e)
        {
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string dir = String.Empty;
            try
            {
                dir = Path.GetDirectoryName(Application.ExecutablePath) + @"\debug.txt";
                File.AppendAllText(dir, address.baseadd.ToString("X") + Environment.NewLine);
                File.AppendAllText(dir, address.soldi.ToString("X") + " = " + m.ReadInt32(address.soldi) + Environment.NewLine);
                File.AppendAllText(dir, address.norecoil.ToString("X") + " = " + m.ReadXBytes(address.norecoil, 1)[0] + Environment.NewLine);
                File.AppendAllText(dir, address.timescale.ToString("X") + " = " + m.ReadFloat(address.timescale) + Environment.NewLine);
                File.AppendAllText(dir, address.vita.ToString("X")+ " = " + m.ReadInt32(address.vita) + Environment.NewLine);
                File.AppendAllText(dir, address.give1.ToString("X") + " = " + m.ReadInt32(address.give1) + Environment.NewLine);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Error");
            }
        }

        private void checkBox14_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox14.Checked == true)
            {
                vita_p2.Start();
                checkBox13.Checked = false;

            }
            else
            {
                vita_p2.Stop();
                m.WriteInt32(address.p2.vita, 100);
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            m.WriteFloat(address.X, X_p2);
            m.WriteFloat(address.Y, Y_p2);
            m.WriteFloat(address.Z, Z_p2);
        }

        private void button24_Click_1(object sender, EventArgs e)
        {
            m.WriteFloat(address.X, X_p3);
            m.WriteFloat(address.Y, Y_p3);
            m.WriteFloat(address.Z, Z_p3);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            m.WriteFloat(address.X, X_p4);
            m.WriteFloat(address.Y, Y_p4);
            m.WriteFloat(address.Z, Z_p4);
        }

        private void button28_Click(object sender, EventArgs e)
        {
            m.WriteFloat(address.p2.X, X);
            m.WriteFloat(address.p2.Y, Y);
            m.WriteFloat(address.p2.Z, Z);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            m.WriteFloat(address.p2.X, X_p3);
            m.WriteFloat(address.p2.Y, Y_p3);
            m.WriteFloat(address.p2.Z, Z_p3);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            m.WriteFloat(address.p2.X, X_p4);
            m.WriteFloat(address.p2.Y, Y_p4);
            m.WriteFloat(address.p2.Z, Z_p4);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            m.WriteFloat(address.p3.X, X);
            m.WriteFloat(address.p3.Y, Y);
            m.WriteFloat(address.p3.Z, Z);
        }

        private void button30_Click(object sender, EventArgs e)
        {
            m.WriteFloat(address.p3.X, X_p2);
            m.WriteFloat(address.p3.Y, Y_p2);
            m.WriteFloat(address.p3.Z, Z_p2);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            m.WriteFloat(address.p3.X, X_p4);
            m.WriteFloat(address.p3.Y, Y_p4);
            m.WriteFloat(address.p3.Z, Z_p4);
        }

        private void button34_Click(object sender, EventArgs e)
        {
            m.WriteFloat(address.p4.X, X);
            m.WriteFloat(address.p4.Y, Y);
            m.WriteFloat(address.p4.Z, Z);
        }

        private void button33_Click(object sender, EventArgs e)
        {
            m.WriteFloat(address.p4.X, X_p2);
            m.WriteFloat(address.p4.Y, Y_p2);
            m.WriteFloat(address.p4.Z, Z_p2);
        }

        private void button32_Click(object sender, EventArgs e)
        {
            m.WriteFloat(address.p4.X, X_p3);
            m.WriteFloat(address.p4.Y, Y_p3);
            m.WriteFloat(address.p4.Z, Z_p3);
        }

        private void checkBox13_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox13.Checked == true)
            {
                checkBox14.Checked = false;
                m.WriteNOP(address.p2.vita);
            }
            else
            {
                m.WriteInt32(address.p2.vita, 100);
            }
        }

        private void checkBox15_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox15.Checked == true)
            {
                ammo_p2.Start();
            }
            else
            {
                ammo_p2.Stop();
                for (int i = 0; i < address.p2.ammo.Length; i++)
                {
                    m.WriteInt32(address.p2.ammo[i], 10);
                }
            }
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox11.Checked)
            {
                special_p2.Start();
            }
            else
            {
                special_p2.Stop();
                m.WriteFloat(address.p2.special, 0);
            }
        }

        private void special_p2_Tick(object sender, EventArgs e)
        {
            m.WriteFloat(address.p2.special, 1);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            address.findadd("money_p2");
            //address.setadd();
            m.WriteInt32(address.p2.soldi, Convert.ToInt32(moneyval_p2.Text));
            //MessageBox.Show(address.p2.soldi.ToString("X"));
        }

        private void checkBox12_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox12.Checked)
            {
                address.findadd("money_p2");
                address.setadd();
                money_p2.Start();
            }
            else
            {
                money_p2.Stop();
            }
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            if (radioButton6.Checked)
            {
                m.WriteInt32(address.p2.give1, Convert.ToInt32(weaponval_p2.Text));
                m.WriteInt32(address.p2.ammo_init[0], Convert.ToInt32(weaponval_p2.Text));
            }
            else if (radioButton5.Checked)
            {
                m.WriteInt32(address.p2.give2, Convert.ToInt32(weaponval_p2.Text));
                m.WriteInt32(address.p2.ammo_init[2], Convert.ToInt32(weaponval_p2.Text));
            }
            else
            {
                m.WriteInt32(address.p2.give3, Convert.ToInt32(weaponval_p2.Text));
                m.WriteInt32(address.p2.ammo_init[3], Convert.ToInt32(weaponval_p2.Text));
            }
        }

        private void checkBox16_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox16.Checked == true)
            {
                rapid_p2.Start();
            }
            else
            {
                rapid_p2.Stop();
            }
        }

        private void button19_Click_1(object sender, EventArgs e)
        {
            address.findadd("money_p3");
            address.setadd();
            m.WriteInt32(address.p3.soldi, Convert.ToInt32(moneyval_p3.Text));
        }

        private void button22_Click_1(object sender, EventArgs e)
        {
            address.findadd("money_p4");
            address.setadd();
            m.WriteInt32(address.p4.soldi, Convert.ToInt32(moneyval_p4.Text));
        }

        private void checkBox10_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox10.Checked)
            {
                address.findadd("money_p3");
                address.setadd();
                money_p3.Start();
            }
            else
            {
                money_p3.Stop();
            }
        }

        private void money_p3_Tick(object sender, EventArgs e)
        {
            m.WriteInt32(address.p3.soldi, Convert.ToInt32(moneyval_p3.Text));
        }

        private void checkBox22_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox22.Checked)
            {
                address.findadd("money_p4");
                address.setadd();
                money_p4.Start();
            }
            else
            {
                money_p4.Stop();
            }
        }

        private void money_p4_Tick(object sender, EventArgs e)
        {
            m.WriteInt32(address.p4.soldi, Convert.ToInt32(moneyval_p4.Text));
        }

        private void checkBox18_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox18.Checked == true)
            {
                vita_p3.Start();
                checkBox17.Checked = false;

            }
            else
            {
                vita_p3.Stop();
                m.WriteInt32(address.p3.vita, 100);
            }
        }

        private void vita_p3_Tick(object sender, EventArgs e)
        {
            m.WriteInt32(address.p3.vita, 10000);
        }

        private void checkBox17_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox17.Checked == true)
            {
                checkBox18.Checked = false;
                m.WriteNOP(address.p3.vita);
            }
            else
            {
                m.WriteInt32(address.p3.vita, 100);
            }
        }

        private void button18_Click_1(object sender, EventArgs e)
        {
            if (radioButton9.Checked)
            {
                m.WriteInt32(address.p3.give1, Convert.ToInt32(weaponval_p3.Text));
                m.WriteInt32(address.p3.ammo_init[0], Convert.ToInt32(weaponval_p3.Text));
            }
            else if (radioButton8.Checked)
            {
                m.WriteInt32(address.p3.give2, Convert.ToInt32(weaponval_p3.Text));
                m.WriteInt32(address.p3.ammo_init[2], Convert.ToInt32(weaponval_p3.Text));
            }
            else
            {
                m.WriteInt32(address.p3.give3, Convert.ToInt32(weaponval_p3.Text));
                m.WriteInt32(address.p3.ammo_init[3], Convert.ToInt32(weaponval_p3.Text));
            }
        }

        private void checkBox8_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox8.Checked)
            {
                special_p3.Start();
            }
            else
            {
                special_p3.Stop();
                m.WriteFloat(address.p3.special, 0);
            }
        }

        private void special_p3_Tick(object sender, EventArgs e)
        {
            m.WriteFloat(address.p3.special, 1);
        }

        private void checkBox19_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox19.Checked == true)
            {
                ammo_p3.Start();
            }
            else
            {
                ammo_p3.Stop();
                for (int i = 0; i < address.p3.ammo.Length; i++)
                {
                    m.WriteInt32(address.p3.ammo[i], 10);
                }
            }
        }

        private void ammo_p3_Tick(object sender, EventArgs e)
        {
            ammo_p3.Interval = 1000;
            for (int i = 0; i < address.p3.ammo.Length; i++)
            {
                m.WriteInt32(address.p3.ammo[i], 65535);
            }
        }

        private void checkBox20_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox20.Checked == true)
            {
                rapid_p3.Start();
            }
            else
            {
                rapid_p3.Stop();
            }
        }

        private void rapid_p3_Tick(object sender, EventArgs e)
        {
            m.WriteInt32(address.p3.rapid, 0);
        }

        private void button21_Click_1(object sender, EventArgs e)
        {
            if (radioButton12.Checked)
            {
                m.WriteInt32(address.p4.give1, Convert.ToInt32(weaponval_p4.Text));
                m.WriteInt32(address.p4.ammo_init[0], Convert.ToInt32(weaponval_p4.Text));
            }
            else if (radioButton11.Checked)
            {
                m.WriteInt32(address.p4.give2, Convert.ToInt32(weaponval_p4.Text));
                m.WriteInt32(address.p4.ammo_init[2], Convert.ToInt32(weaponval_p4.Text));
            }
            else
            {
                m.WriteInt32(address.p4.give3, Convert.ToInt32(weaponval_p4.Text));
                m.WriteInt32(address.p4.ammo_init[3], Convert.ToInt32(weaponval_p4.Text));
            }
        }

        private void checkBox21_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox21.Checked)
            {
                special_p4.Start();
            }
            else
            {
                special_p4.Stop();
                m.WriteFloat(address.p4.special, 0);
            }
        }

        private void special_p4_Tick(object sender, EventArgs e)
        {
            m.WriteFloat(address.p4.special, 1);
        }

        private void checkBox24_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox24.Checked == true)
            {
                vita_p4.Start();
                checkBox23.Checked = false;

            }
            else
            {
                vita_p4.Stop();
                m.WriteInt32(address.p4.vita, 100);
            }
        }

        private void checkBox23_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox23.Checked == true)
            {
                checkBox24.Checked = false;
                m.WriteNOP(address.p4.vita);
            }
            else
            {
                m.WriteInt32(address.p4.vita, 100);
            }
        }

        private void vita_p4_Tick(object sender, EventArgs e)
        {
            m.WriteInt32(address.p4.vita, 10000);
        }

        private void checkBox25_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox25.Checked == true)
            {
                ammo_p4.Start();
            }
            else
            {
                ammo_p4.Stop();
                for (int i = 0; i < address.p4.ammo.Length; i++)
                {
                    m.WriteInt32(address.p4.ammo[i], 10);
                }
            }
        }

        private void ammo_p4_Tick(object sender, EventArgs e)
        {
            ammo_p4.Interval = 1000;
            for (int i = 0; i < address.p4.ammo.Length; i++)
            {
                m.WriteInt32(address.p4.ammo[i], 65535);
            }
        }

        private void tabPage5_Click_1(object sender, EventArgs e)
        {

        }

        private void checkBox26_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox26.Checked == true)
            {
                rapid_p4.Start();
            }
            else
            {
                rapid_p4.Stop();
            }
        }

        private void rapid_p4_Tick(object sender, EventArgs e)
        {
            m.WriteInt32(address.p4.rapid, 0);
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            int k = 0;
            if (checkBox43.Checked)
            {
                checkBox43.Checked = false;
                k = 1;
            }
            for (int i = 0; i < address.posz_X.Length; i++)
            {
                m.WriteFloat(address.posz_X[i], X_p2 + 50);
                m.WriteFloat(address.posz_Y[i], Y_p2 + 50);
                m.WriteFloat(address.posz_Z[i], Z_p2);
            }
            if (k == 1)
            {
                checkBox43.Checked = true;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            int k = 0;
            if (checkBox43.Checked)
            {
                checkBox43.Checked = false;
                k = 1;
            }
            for (int i = 0; i < address.posz_X.Length; i++)
            {
                m.WriteFloat(address.posz_X[i], X_p3 + 50);
                m.WriteFloat(address.posz_Y[i], Y_p3 + 50);
                m.WriteFloat(address.posz_Z[i], Z_p3);
            }
            if (k == 1)
            {
                checkBox43.Checked = true;
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            int k = 0;
            if (checkBox43.Checked)
            {
                checkBox43.Checked = false;
                k = 1;
            }
            for (int i = 0; i < address.posz_X.Length; i++)
            {
                m.WriteFloat(address.posz_X[i], X_p4 + 50);
                m.WriteFloat(address.posz_Y[i], Y_p4 + 50);
                m.WriteFloat(address.posz_Z[i], Z_p4);
            }
            if (k == 1)
            {
                checkBox43.Checked = true;
            }
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void checkBox27_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox27.Checked)
            {
                window.Start();
            }
            else
            {
                window.Stop();
            }
        }

        private void window_Tick(object sender, EventArgs e)
        {
            window.Interval = 100;
            this.Text = RandomString(20);
        }

        private void checkBox51_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox51.Checked)
            {
                m.WriteByte(address.norecoil, 117);
            }
            else
            {
                m.WriteByte(address.norecoil, 116);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            m.WriteFloat(address.X, undox3);
            m.WriteFloat(address.Y, undoy3);
            m.WriteFloat(address.Z, undoz3);
            button17.Enabled = false;
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void button21_Click(object sender, EventArgs e)
        {

        }

        private void button22_Click(object sender, EventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void checkBox14_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void vita_p2_Tick(object sender, EventArgs e)
        {
            m.WriteInt32(address.p2.vita, 10000);
        }

        private void checkBox15_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ammo_p2_Tick(object sender, EventArgs e)
        {
            ammo_p2.Interval = 1000;
            for (int i = 0; i < address.p2.ammo.Length; i++)
            {
                m.WriteInt32(address.p2.ammo[i], 65535);
            }
        }

        private void checkBox16_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rapid_p2_Tick(object sender, EventArgs e)
        {
            m.WriteInt32(address.p2.rapid, 0);
        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button19_Click(object sender, EventArgs e)
        {

        }

        private void money_p2_Tick(object sender, EventArgs e)
        {
            m.WriteInt32(address.p2.soldi, Convert.ToInt32(moneyval_p2.Text));
        }

        private void button18_Click(object sender, EventArgs e)
        {
        }

        private void checkBox17_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button24_Click(object sender, EventArgs e)
        {

        }



        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string dir = String.Empty;
            string[] lines = null;
            try
            {
                dir = Path.GetDirectoryName(Application.ExecutablePath) + @"\tp.txt";
                lines = File.ReadAllLines(dir);
                int index = Array.IndexOf(lines, comboBox2.SelectedItem.ToString());
                numericX.Text = lines[index + 1];
                numericY.Text = lines[index + 2];
                numericZ.Text = lines[index + 3];
            }
            catch(FileNotFoundException)
            {
                MessageBox.Show("File 'tp.txt' not found");
            }
            toolTip1.SetToolTip(comboBox2, comboBox2.Text);
        }

        private void comboBox2_DropDown(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            string dir = String.Empty;
            string[] lines = null;
            try
            {
                dir = Path.GetDirectoryName(Application.ExecutablePath) + @"\tp.txt";
                lines = File.ReadAllLines(dir);
                for (int i = 0; i < lines.Count(); i++)
                {
                    if (i % 5 == 0)
                        comboBox2.Items.Add(lines[i]);
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("File 'tp.txt' not found");
            }
        }


        private void replacer_Tick(object sender, EventArgs e)
        {

        }



        private void checkBox50_CheckedChanged(object sender, EventArgs e)
        {
        }
        

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void button49_Click(object sender, EventArgs e)
        {

        }

        private void button51_Click(object sender, EventArgs e)
        {
        }


        private void checkBox33_CheckedChanged(object sender, EventArgs e)
        {
        }


        private void checkBox37_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void checkBox36_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void checkBox35_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void checkBox34_CheckedChanged(object sender, EventArgs e)
        {
        }

    }
}
