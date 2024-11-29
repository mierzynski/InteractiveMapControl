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
        private int previousGridSpacing = -1; // -1 oznacza, że nie ma poprzedniej wartości przy pierwszym uruchomieniu
        private Bitmap gridBitmap;

        public MapControl()
        {
            InitializeComponent();

            backgroundPictureBox.SizeMode = PictureBoxSizeMode.Normal;
            backgroundPictureBox.BackColor = Color.White;

            backgroundPictureBox.MouseWheel += BackgroundPictureBox_MouseWheel;

            UpdateGrid();

            axisXPanel.Paint += AxisXPanel_Paint;
            axisYPanel.Paint += AxisYPanel_Paint;

            DisplayObjectInfo();


            AddObject("Hala", 500, 200, 20, 20, true, 0);
            AddObject("Obiekt", 140, 60, 40, 40, false, 1, 0);
            AddObject("Obiekt", 140, 60, 160, 80, false, 2, 0);
        }
        private void BackgroundPictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (ModifierKeys.HasFlag(Keys.Control))
            {
                if (e.Delta > 0)
                {
                    gridSpacing = Math.Min(gridSpacing + 5, 100); // Maksymalny odstęp: 100
                }
                else if (e.Delta < 0)
                {
                    gridSpacing = Math.Max(gridSpacing - 5, 5); // Minimalny odstęp: 5
                }
                UpdateGrid();
            }
        }

        private void UpdateObjectSizes()
        {
            float scaleFactor = gridSpacing / 20.0f; // Obliczamy współczynnik skali

            foreach (var boardObject in boardObjects)
            {
                // Skalowanie rozmiaru
                int newWidth = (int)(boardObject.OriginalSize.Width * scaleFactor);
                int newHeight = (int)(boardObject.OriginalSize.Height * scaleFactor);

                // Skalowanie oryginalnej lokalizacji
                int newX = (int)(boardObject.OriginalLocation.X * scaleFactor);
                int newY = (int)(boardObject.OriginalLocation.Y * scaleFactor);

                // Zaokrąglamy do najbliższej linii siatki
                newX = (newX / gridSpacing) * gridSpacing;  // Zaokrąglamy w dół
                newY = (newY / gridSpacing) * gridSpacing;  // Zaokrąglamy w dół

                // Aktualizacja właściwości UI
                boardObject.UIElement.Location = new Point(newX, newY);
                boardObject.UIElement.Size = new Size(newWidth, newHeight);
            }
        }





        //PROPOZYCJA CHATGPT
        //Domyślnie zdarzenie MouseWheel może być przechwycone przez kontener nadrzędny kontrolki.
        //Aby mieć pewność, że zdarzenie trafi do backgroundPictureBox, nadpisz metodę OnMouseWheel
        //w klasie MapControl i wywołaj zdarzenie na backgroundPictureBox

        ////protected override void OnMouseWheel(MouseEventArgs e)
        //{
        //    base.OnMouseWheel(e);

        //    // Przekierowanie zdarzenia MouseWheel do backgroundPictureBox
        //    backgroundPictureBox?.Focus();
        //    backgroundPictureBox?.InvokeOnClick(backgroundPictureBox, e);
        //}

        private void UpdateGrid()
        {
            gridBitmap = GenerateGridBitmap(backgroundPictureBox.Width, backgroundPictureBox.Height, gridSpacing);
            backgroundPictureBox.Image = gridBitmap;

            axisXPanel.Invalidate();
            axisYPanel.Invalidate();

            // Jeśli wartość gridSpacing zmieniła się, dostosowujemy pozycje obiektów
            if (previousGridSpacing != -1)
            {
                int difference = gridSpacing - previousGridSpacing;

                foreach (var boardObject in boardObjects)
                {
                    Point newLocation = boardObject.OriginalLocation;

                    newLocation.X += difference;
                    newLocation.Y += difference;


                    boardObject.OriginalLocation = newLocation;
                    boardObject.UIElement.Location = newLocation;
                }
            }

            previousGridSpacing = gridSpacing;
            UpdateObjectSizes();
        }


        private Bitmap GenerateGridBitmap(int width, int height, int spacing)
        {
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);

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


            BoardObject parentObject = null;
            if (parentId.HasValue)
            {
                parentObject = boardObjects.FirstOrDefault(obj => obj.ID == parentId.Value);
            }

            var boardObject = new BoardObject
            {
                ID = id,
                Name = label,
                Parent = parentObject,
                Category = label,
                Group = "Default",
                UIElement = uiPanel,
                ZIndex = currentZIndex,
                OriginalLocation = new Point(x, y),
                OriginalSize = new Size(width, height),
            };


            if (parentObject != null)
            {
                parentObject.Children.Add(boardObject);
            }

            uiPanel.Tag = boardObject;

            boardObjects.Add(boardObject);

            if (positionBottom)
            {
                uiPanel.SendToBack();
            }
            else
            {
                uiPanel.BringToFront();
            }

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

                listBox.Items.Add($"ID: {obj.ID}, Location: {obj.OriginalLocation}, Parent ID: {parentInfo}, Children IDs: {childrenInfo}");
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
                // Obliczamy nową pozycję obiektu
                int newX = _draggedControl.Left + e.X - _dragStartPoint.X;
                int newY = _draggedControl.Top + e.Y - _dragStartPoint.Y;

                // Zaokrąglamy do najbliższej linii siatki
                int nearestXDown = (newX / gridSpacing) * gridSpacing;  // Zaokrąglamy w dół
                int nearestXUp = ((newX + gridSpacing - 1) / gridSpacing) * gridSpacing;  // Zaokrąglamy w górę
                newX = Math.Abs(newX - nearestXDown) < Math.Abs(newX - nearestXUp) ? nearestXDown : nearestXUp;

                int nearestYDown = (newY / gridSpacing) * gridSpacing;  // Zaokrąglamy w dół
                int nearestYUp = ((newY + gridSpacing - 1) / gridSpacing) * gridSpacing;  // Zaokrąglamy w górę
                newY = Math.Abs(newY - nearestYDown) < Math.Abs(newY - nearestYUp) ? nearestYDown : nearestYUp;

                // Przypisujemy nowe wartości lokalizacji
                BoardObject draggedObject = _draggedControl.Tag as BoardObject;
                if (draggedObject != null)
                {
                    // Aktualizacja lokalizacji obiektu
                    draggedObject.OriginalLocation = new Point(newX, newY); // Aktualizujemy oryginalną lokalizację

                    // Ustawienie nowej pozycji w UI
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
            Pen thickPen = new Pen(Color.Black, 2); // Grube kreski dla etykiet
            Pen thinPen = new Pen(Color.Black, 1); // Cienkie kreski pomocnicze
            Font labelFont = new Font("Arial", 6);

            int panelWidth = isXAxis ? axisXPanel.Width : axisYPanel.Width;
            int panelHeight = isXAxis ? axisXPanel.Height : axisYPanel.Height;

            int spacing = gridSpacing;

            // Rysowanie głównej linii osi X (u dołu) lub Y (po lewej)
            if (isXAxis)
            {
                g.DrawLine(thinPen, 0, panelHeight - 1, panelWidth, panelHeight - 1);
            }
            else
            {
                g.DrawLine(thinPen, 0, 0, 0, panelHeight);
            }

            // Rysowanie linii i etykiet
            for (int i = 0; i <= (isXAxis ? panelWidth : panelHeight) / spacing; i++)
            {
                int position = i * spacing;

                // Wybór stylu kreski
                Pen currentPen = (i % 2 == 0) ? thickPen : thinPen;

                // Rysowanie kreski
                if (isXAxis)
                {
                    g.DrawLine(currentPen, position, panelHeight - 10, position, panelHeight);
                }
                else
                {
                    g.DrawLine(currentPen, 0, position, 10, position);
                }

                // Rysowanie etykiet tylko co drugą kreskę
                if (i % 2 == 0) // Co drugą kreskę, gdy indeks `i` jest parzysty
                {
                    string label = (i / 2).ToString(); // Etykiety numerowane sekwencyjnie, np. 0, 1, 2...

                    if (isXAxis)
                    {
                        // Etykiety dla osi X
                        SizeF labelSize = g.MeasureString(label, labelFont);
                        float labelX = position - (labelSize.Width / 2);
                        float labelY = panelHeight - 20;
                        g.DrawString(label, labelFont, Brushes.Black, labelX, labelY);
                    }
                    else
                    {
                        // Etykiety dla osi Y
                        SizeF labelSize = g.MeasureString(label, labelFont);
                        float labelX = 12; // Stała wartość dla lewego marginesu osi Y
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
