using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MasterLibrary.LowLevel_IO.Drive;
using MasterLibrary.LowLevel_IO.PInvoke;
using MasterLibrary.Datasave.SaveableClasses;
using MasterLibrary.LowLevel_IO.Streams;

namespace TestApplication
{
    public partial class Form1 : Form
    {
        Settings settings = new Settings();
        Int64 pos = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            Button1_Click(null, null);

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(WIN32_DiskDrive.GetDrives());

            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Disk disk = new Disk();
            disk.Open(comboBox1.SelectedItem as WIN32_DiskDrive);

            SectorStream stream = new SectorStream(disk);
            stream.Seek(446);
            Partition[] partitions = new Partition[4];
            partitions[0] = new Partition(stream.Read(16));
            partitions[1] = new Partition(stream.Read(16));
            partitions[2] = new Partition(stream.Read(16));
            partitions[3] = new Partition(stream.Read(16));

            pos = (partitions[0].LBA_Begin + partitions[0].NumberOfSectors) * disk.BytesPerSector;

            
            stream.Close();
            LoadPWS();
        }

        private void LoadPWS()
        {
            Disk disk = new Disk();
            disk.Open(comboBox1.SelectedItem as WIN32_DiskDrive);

            SectorStream stream = new SectorStream(disk);
            stream.Seek(pos);
            settings.Load(stream);
            stream.Close();

            listBox1.Items.Clear();
            listBox1.DataSource = settings.Accounts;
        }

        private void SavePWS()
        {
            Disk disk = new Disk();
            disk.Open(comboBox1.SelectedItem as WIN32_DiskDrive);

            SectorStream stream = new SectorStream(disk);
            stream.Seek(pos);
            settings.Save(stream);
            stream.Flush();
            stream.Close();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            settings.Accounts.Add(new Account());
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Account sa = listBox1.SelectedItem as Account;
            textBox1.Text = sa.Name;
            textBox2.Text = sa.Site;
            textBox3.Text = sa.Email;
            textBox4.Text = sa.Username;
            textBox5.Text = sa.Password;
            richTextBox1.Text = sa.Detail;
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            Account sa = listBox1.SelectedItem as Account;
            sa.Name = textBox1.Text;
            sa.Site = textBox2.Text;
            sa.Email = textBox3.Text;
            sa.Username = textBox4.Text;
            sa.Password = textBox5.Text;
            sa.Detail = richTextBox1.Text;

            SavePWS();
        }
    }

    public class Settings : SaveableSettings
    {
        public BindingList<Account> Accounts { get; set; } = new BindingList<Account>(); 

    }


    public class Account
    {
        public string Name { get; set; } = "New Item";
        public string Site { get; set; } = "New Item";
        public string Email { get; set; } = "New Item";
        public string Username { get; set; } = "New Item";
        public string Password { get; set; } = "New Item";
        public string Detail { get; set; } = "";
        public override string ToString()
        {
            return Name;
        }
    }





    public class Partition
    {
        public byte BootFlag { get; set; }
        public UInt32 CHS_Begin { get; set; }
        public UInt32 CHS_End { get; set; }
        public byte TypeCode { get; set; }
        public UInt32 LBA_Begin { get; set; }
        public UInt32 NumberOfSectors { get; set; }

        public Partition(byte[] data)
        {
            BootFlag = data[0];
            CHS_Begin = BitConverter.ToUInt32(data, 0) & 0x00FFFFFF;
            TypeCode = data[4];
            CHS_End = BitConverter.ToUInt32(data, 4) & 0x00FFFFFF;
            LBA_Begin = BitConverter.ToUInt32(data, 8);
            NumberOfSectors = BitConverter.ToUInt32(data, 12);
        }
    }

    
}
