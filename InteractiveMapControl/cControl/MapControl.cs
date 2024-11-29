using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using InteractiveMapControl.cControl.Models;
using System.Linq;

/*
1. Obiekty, które będą tworzone: hala, pomieszczene, regał, przedmiot
2. Możliwość grupowania obiektów
3. Możliwość skalowania poprzez przybliżanie
4. Możliwość kliknięcia na obiekt regał w celu otworzenia okna gdzie będzie wizualizacja regału od przodu
 */

namespace InteractiveMapControl.cControl
{
    public partial class MapControl : UserControl
    {
        private Point _dragStartPoint;
        private Control _draggedControl;
        private List<BoardObject> boardObjects = new List<BoardObject>();
        private Panel _selectedPanel;

        private int gridSpacing = 20;
        private Bitmap gridBitmap;

        public MapControl()
        {
            InitializeComponent();

            backgroundPictureBox.SizeMode = PictureBoxSizeMode.Normal;
            backgroundPictureBox.BackColor = Color.White;

            UpdateGrid();

            axisXPanel.Paint += AxisXPanel_Paint;
            axisYPanel.Paint += AxisYPanel_Paint;

            DisplayObjectInfo();

            // Dodaj testowe obiekty
            AddObject("Hala", 500, 200, 20, 20, true, 0);
            AddObject("Obiekt", 140, 60, 40, 40, false, 1, 0);
            AddObject("Obiekt", 140, 60, 160, 80, false, 2, 0);


        }

        private void UpdateGrid()
        {
            // Tworzenie bitmapy siatki na podstawie aktualnych ustawień
            gridBitmap = GenerateGridBitmap(backgroundPictureBox.Width, backgroundPictureBox.Height, gridSpacing);

            // Ustawienie bitmapy jako tła PictureBox
            backgroundPictureBox.Image = gridBitmap;
        }

        private Bitmap GenerateGridBitmap(int width, int height, int spacing)
        {
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White); // Tło siatki

                // Pióro dla linii siatki
                using (Pen gridPen = new Pen(Color.LightGray, 1))
                {
                    // Rysowanie linii pionowych
                    for (int x = 0; x < width; x += spacing)
                    {
                        g.DrawLine(gridPen, x, 0, x, height);
                    }

                    // Rysowanie linii poziomych
                    for (int y = 0; y < height; y += spacing)
                    {
                        g.DrawLine(gridPen, 0, y, width, y);
                    }
                }
            }
            return bitmap;
        }

        public void AddObject(string label, int width, int height, int x, int y, bool positionBottom, int id, int? parentId = null)
        {
            int currentZIndex = backgroundPictureBox.Controls.Count;

            var uiPanel = new Panel
            {
                Width = width,
                Height = height,
                BackColor = label.Equals("hala", StringComparison.OrdinalIgnoreCase) ? Color.Transparent : Color.LightBlue,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(x, y)
            };

            var labelControl = new Label
            {
                Text = $"{label}, ID: {id}",
                AutoSize = true,
                Location = new Point(5, 5)
            };

            uiPanel.Controls.Add(labelControl);

            uiPanel.MouseDown += Rectangle_MouseDown;
            uiPanel.MouseMove += Rectangle_MouseMove;
            uiPanel.MouseUp += Rectangle_MouseUp;
            uiPanel.DoubleClick += Rectangle_DoubleClick;

            backgroundPictureBox.Controls.Add(uiPanel);


            // Jeśli parentId jest przekazany, wyszukaj rodzica po ID
            BoardObject parentObject = null;
            if (parentId.HasValue)
            {
                parentObject = boardObjects.FirstOrDefault(obj => obj.ID == parentId.Value);
            }

            // Tworzenie nowego obiektu BoardObject
            var boardObject = new BoardObject
            {
                ID = id, // Używamy przekazanego ID
                Name = label,
                Location = new Point(x, y),
                Parent = parentObject, // Przypisujemy rodzica, jeśli znaleziony
                Category = label,
                Group = "Default",
                UIElement = uiPanel,
                ZIndex = currentZIndex
            };

            // Jeśli rodzic istnieje, dodaj obiekt do listy dzieci rodzica
            if (parentObject != null)
            {
                parentObject.Children.Add(boardObject);
            }

            uiPanel.Tag = boardObject;

            // Dodaj obiekt do listy boardObjects
            boardObjects.Add(boardObject);

            // Ustawienie kolejności wyświetlania
            if (positionBottom)
            {
                uiPanel.SendToBack();
            }
            else
            {
                uiPanel.BringToFront();
            }

            // Aktualizacja ZIndex dla wszystkich obiektów
            UpdateZIndices();

            // TESTOWY PANEL DO ŚLEDZENIE INFORMACJI O OBIEKTACH
            DisplayObjectInfo();
        }


        // TESTOWY PANEL DO ŚLEDZENIE INFORMACJI O OBIEKTACH
        private void DisplayObjectInfo()
        {
            listBox.Items.Clear();

            foreach (var obj in boardObjects)
            {
                string parentInfo = obj.Parent != null ? obj.Parent.ID.ToString() : "Brak rodzica";
                string childrenInfo = obj.Children.Any() ? string.Join(", ", obj.Children.Select(c => c.ID)) : "Brak dzieci";

                listBox.Items.Add($"ID: {obj.ID}, Location: {obj.Location}, Parent ID: {parentInfo}, Children IDs: {childrenInfo}");
            }
        }

        private void Rectangle_MouseDown(object sender, MouseEventArgs e)
        {
            _draggedControl = sender as Control;
            _dragStartPoint = e.Location;
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {

            if (_draggedControl != null && e.Button == MouseButtons.Left)
            {
                // Obliczanie nowej pozycji
                int newX = _draggedControl.Left + e.X - _dragStartPoint.X;
                int newY = _draggedControl.Top + e.Y - _dragStartPoint.Y;

                // Pobranie obiektu BoardObject z Tag
                BoardObject draggedObject = _draggedControl.Tag as BoardObject;
                if (draggedObject != null)
                {
                    // Zaktualizowanie lokalizacji
                    draggedObject.Location = new Point(newX, newY);

                    // Rysowanie obiektu w UI
                    _draggedControl.Left = newX;
                    _draggedControl.Top = newY;
                }
            }
        }

        private void Rectangle_MouseUp(object sender, MouseEventArgs e)
        {
            _draggedControl = null;

            // TESTOWY PANEL DO ŚLEDZENIE INFORMACJI O OBIEKTACH
            DisplayObjectInfo();
        }


        private void UpdateZIndices()
        {
            for (int i = 0; i < backgroundPictureBox.Controls.Count; i++)
            {
                var control = backgroundPictureBox.Controls[i];
                var boardObject = boardObjects.FirstOrDefault(obj => obj.UIElement == control);

                if (boardObject != null)
                {
                    boardObject.ZIndex = i;
                }
            }
        }

        private void Rectangle_DoubleClick(object sender, EventArgs e)
        {
            var clickedPanel = sender as Panel;

            if (clickedPanel != null)
            {
                if (_selectedPanel != null)
                {
                    _selectedPanel.BackColor = Color.LightBlue;
                }


                clickedPanel.BackColor = Color.LightGreen;
                _selectedPanel = clickedPanel;

                MessageBox.Show($"Kliknięto obiekt: {clickedPanel.Controls[0].Text}");

                _selectedPanel.BackColor = Color.LightBlue;
            }
        }

        private void DrawAxis(Graphics g, bool isXAxis)
        {
            Pen thickPen = new Pen(Color.Black, 2);
            Pen thinPen = new Pen(Color.Black, 1);
            Font labelFont = new Font("Arial", 6);

            int panelWidth = isXAxis ? axisXPanel.Width : axisYPanel.Width;
            int panelHeight = isXAxis ? axisXPanel.Height : axisYPanel.Height;

            int spacing = 20;

            // Rysowanie głównej linii osi X (u dołu) lub Y (po lewej)
            if (isXAxis)
            {
                g.DrawLine(thinPen, 0, panelHeight - 1, panelWidth, panelHeight - 1);
            }
            else
            {
                g.DrawLine(thinPen, 0, 0, 0, panelHeight);
            }

            // Rysowanie głównych kresek i etykiet
            for (int i = 0; i <= (isXAxis ? panelWidth : panelHeight) / spacing; i++)
            {
                int position = i * spacing;

                if (isXAxis)
                {
                    // Rysowanie głównej kreski na osi X
                    g.DrawLine(thickPen, position, panelHeight - 10, position, panelHeight);

                    // Rysowanie cienkich kresek na osi X
                    if (i > 0)
                    {
                        int thinPosition = position - spacing / 2;
                        g.DrawLine(thinPen, thinPosition, panelHeight - 5, thinPosition, panelHeight);
                    }

                    // Etykiety na osi X
                    if (i > 0)
                    {
                        string label = i.ToString();
                        SizeF labelSize = g.MeasureString(label, labelFont);
                        float labelX = position - (labelSize.Width / 2);
                        float labelY = panelHeight - 20;
                        g.DrawString(label, labelFont, Brushes.Black, labelX, labelY);
                    }
                }
                else
                {
                    // Rysowanie głównej kreski na osi Y
                    g.DrawLine(thickPen, 0, position, 10, position);

                    // Rysowanie cienkich kresek na osi Y
                    if (i > 0)
                    {
                        int thinPosition = position - spacing / 2;
                        g.DrawLine(thinPen, 0, thinPosition, 5, thinPosition);
                    }

                    // Etykiety na osi Y, zmniejszona odległość od osi
                    if (i > 0)
                    {
                        string label = i.ToString();
                        SizeF labelSize = g.MeasureString(label, labelFont);
                        float labelX = 12; // Mniejsza wartość dla osi Y, aby etykiety były bliżej kreski
                        float labelY = position - (labelSize.Height / 2);
                        g.DrawString(label, labelFont, Brushes.Black, labelX, labelY);
                    }
                }
            }
        }

        private void AxisXPanel_Paint(object sender, PaintEventArgs e)
        {
            DrawAxis(e.Graphics, isXAxis: true);
        }

        private void AxisYPanel_Paint(object sender, PaintEventArgs e)
        {
            DrawAxis(e.Graphics, isXAxis: false);
        }
    }
}
