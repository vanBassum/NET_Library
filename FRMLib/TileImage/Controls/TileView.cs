using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FRMLib.Scope;

namespace FRMLib.TileImage
{
    public partial class TileView : UserControl
    {
        public Map Map { get; set; }

        public PointD Center { get; set; } = new PointD(0,0);
        public int Zoom { get; set; } = 0;

        PictureBox picBox_Map = new PictureBox();
        PictureBox picBox_Draw = new PictureBox();

        public TileView()
        {
            InitializeComponent();
            this.Controls.Add(picBox_Map);
            picBox_Map.Controls.Add(picBox_Draw);

            picBox_Map.Dock = DockStyle.Fill;
            picBox_Map.BackColor = Color.Transparent;
            picBox_Map.BringToFront();
            picBox_Map.Paint += PicBox_Map_Paint;

            picBox_Draw.Dock = DockStyle.Fill;
            picBox_Draw.BackColor = Color.Transparent;
            picBox_Draw.BringToFront();
            picBox_Draw.Paint += PicBox_Draw_Paint;
        }

        private void PicBox_Draw_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.Red, 0, 0, this.Width, this.Height );
            e.Graphics.DrawLine(Pens.Red, 0, this.Height, this.Width, 0);
        }

        private void PicBox_Map_Paint(object sender, PaintEventArgs e)
        {
            
            if (Map != null)
            {
                if (Map.TileSets != null)
                {
                    RectangleD viewPort = new RectangleD();
                    viewPort.Size = (V2D)this.Size;
                    viewPort.Position = (V2D)Center / Map.TileSets[Zoom].Muliplier - ((V2D)viewPort.Size / 2);

                    if (Map.TileSets.Count > Zoom)
                    {
                        foreach (Tile tile in Map.TileSets[Zoom].Tiles)
                        {
                            if (RectangleD.Collides(viewPort, tile))
                            {
                                //Draw tile.
                                e.Graphics.DrawImage(tile.Image, (Point)(((V2D)tile.Position - (V2D)Center / Map.TileSets[Zoom].Muliplier + ((V2D)viewPort.Size / 2))));
                            }
                        }
                    }
                }
            }
        }
    }
}
