using STDLib.Saveable;
using System.Collections.Generic;
using System.ComponentModel;

namespace FRMLib.TileImage
{
    public class TileSet
    {
        public double Muliplier { get; set; }
        public List<Tile> Tiles { get; set; } = new List<Tile>();
    
    }

    public class Map : Saveable
    {
        public BindingList<TileSet> TileSets { get; set; } = new BindingList<TileSet>();
    }



}
