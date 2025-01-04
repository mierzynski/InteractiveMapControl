using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InteractiveMapControl.cControl.Models
{
    public class BoardObject
    {
        public int ID { get; set; }                 
        public string Name { get; set; }            
        public BoardObject Parent { get; set; }     // Obiekt rodzic (null, jeśli brak)
        public List<BoardObject> Children { get; set; } = new List<BoardObject>();
        public string Category { get; set; }        
        public string Group { get; set; }
        public Control UIElement { get; set; }
        public int ZIndex { get; set; }
        public double LocationX { get; set; }
        public double LocationY { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public Color DefaultBackColor { get; set; }
    }
}
