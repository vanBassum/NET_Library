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
        public event EventHandler<PointD> MouseDownScaled;
        //public event EventHandler<PointD> MouseUpScaled;
        public Map Map { get; set; }

        public PointD Center { get; set; } = new PointD(0,0);
        public int Zoom { get; set; } = 0;
        public event EventHandler<Graphics> OnDrawObjectsScaled;
        PictureBox picBox_Map = new PictureBox();
        PictureBox picBox_Draw = new PictureBox();

        private PointD centerStartDrag;
        private Point mouseStartDrag;

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

            picBox_Draw.MouseMove += picBox_MouseMove;
            picBox_Draw.MouseDown += picBox_MouseDown;
        }


        private void picBox_MouseDown(object sender, MouseEventArgs e)
        {
            centerStartDrag = Center;
            mouseStartDrag = e.Location;

            V2D pt = ((V2D)Center / Map.TileSets[Zoom].Muliplier - ((V2D)this.Size / 2) + (V2D)e.Location) * Map.TileSets[Zoom].Muliplier;
            MouseDownScaled?.Invoke(this, pt);
        }


        private void picBox_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.Button.HasFlag(MouseButtons.Left))
            {
                Center = (V2D)centerStartDrag + ((V2D)mouseStartDrag - (V2D)e.Location) * Map.TileSets[Zoom].Muliplier;
                picBox_Draw.Refresh();
                picBox_Map.Refresh();
            }
        }

        private void PicBox_Draw_Paint(object sender, PaintEventArgs e)
        {

            e.Graphics.DrawLine(Pens.Red, 0, 0, this.Width, this.Height);
            e.Graphics.DrawLine(Pens.Red, 0, this.Height, this.Width, 0);

            if (Map != null)
            {
                if (Map.TileSets != null)
                {
                    if (Map.TileSets.Count > Zoom)
                    {

                        V2D pt = ((V2D)this.Size / 2) - (V2D)Center / Map.TileSets[Zoom].Muliplier;
                        e.Graphics.TranslateTransform((float)pt.X, (float)pt.Y);
                        e.Graphics.ScaleTransform((float)(1.0/Map.TileSets[Zoom].Muliplier), (float)(1.0/Map.TileSets[Zoom].Muliplier));
                        OnDrawObjectsScaled?.Invoke(this, e.Graphics);
                    }
                }
            }

            

            
            
            
            
        }

        private void PicBox_Map_Paint(object sender, PaintEventArgs e)
        {
            
            if (Map != null)
            {
                if (Map.TileSets != null)
                {
                    if (Map.TileSets.Count > Zoom)
                    {
                        RectangleD viewPort = new RectangleD();
                        viewPort.Size = (V2D)this.Size;
                        viewPort.Position = (V2D)Center / Map.TileSets[Zoom].Muliplier - ((V2D)viewPort.Size / 2);
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
