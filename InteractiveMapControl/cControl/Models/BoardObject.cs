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
        public int ID { get; set; }                  // Unikalny identyfikator
        public string Name { get; set; }            // Nazwa obiektu
        public Point Location { get; set; }         // Lokalizacja względem rodzica
        public BoardObject Parent { get; set; }     // Obiekt rodzic (null, jeśli brak)
        public List<BoardObject> Children { get; set; } = new List<BoardObject>(); // Lista dzieci
        public string Category { get; set; }        // Kategoria (np. hala, pomieszczenie)
        public string Group { get; set; }           // Grupa, do której należy obiekt
        public Panel UIElement { get; set; }        // Powiązany element graficzny (Panel)
        public int ZIndex { get; set; }             // Pozycja na osi Z
    }
}
