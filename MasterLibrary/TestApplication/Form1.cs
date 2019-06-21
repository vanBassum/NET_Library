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
using MasterLibrary.LowLevel_IO.Streams;

namespace TestApplication
{
    public partial class Form1 : Form
    {
        Disk disk = new Disk();
        SectorStream stream;
        

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
            WIN32_DiskDrive selectedDrive = comboBox1.SelectedItem as WIN32_DiskDrive;
            disk.Open(selectedDrive);
            stream = new SectorStream(disk);


            stream.Seek(446);
            Partition[] partitions = new Partition[4];
            partitions[0] = new Partition(stream.Read(16));
            partitions[1] = new Partition(stream.Read(16));
            partitions[2] = new Partition(stream.Read(16));
            partitions[3] = new Partition(stream.Read(16));

            stream.Seek((partitions[0].LBA_Begin + partitions[0].NumberOfSectors) * disk.BytesPerSector);


            stream.Write(new byte[] { 0xAA, 0x55, 0xAA, 0x55, 0xAA, 0x55, 0xAA, 0x55, });

            stream.Flush();

            stream.Seek((partitions[0].LBA_Begin + partitions[0].NumberOfSectors) * disk.BytesPerSector);


            byte[] data = stream.Read(1024);






            stream.Close();
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2} ", b);
            return hex.ToString();
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
