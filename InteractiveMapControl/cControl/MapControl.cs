﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace InteractiveMapControl.cControl
{
    public partial class MapControl : UserControl
    {
        private Point _dragStartPoint;
        private Control _draggedControl;
        private List<Panel> mapObjects = new List<Panel>(); // Lista przechowująca obiekty
        private Panel _selectedPanel;
        private int gridSpacing = 20;

        public MapControl()
        {
            InitializeComponent();

            boardPanel.AutoScroll = true;
            boardPanel.BackColor = Color.LightGray;

            axisXPanel.Paint += AxisXPanel_Paint;
            axisYPanel.Paint += AxisYPanel_Paint;

            boardPanel.Paint += BoardPanel_Paint;

            SetGridSpacing(20);
        }

        private void BoardPanel_Paint(object sender, PaintEventArgs e)
        {
            // Pobranie grafiki do rysowania
            Graphics g = e.Graphics;

            // Rozmiar boardPanel
            int width = boardPanel.Width;
            int height = boardPanel.Height;

            // Kolor i styl linii siatki
            Pen gridPen = new Pen(Color.DarkGray, 1);

            // Rysowanie linii pionowych
            for (int x = 0; x < width; x += gridSpacing)
            {
                g.DrawLine(gridPen, x, 0, x, height);
            }

            // Rysowanie linii poziomych
            for (int y = 0; y < height; y += gridSpacing)
            {
                g.DrawLine(gridPen, 0, y, width, y);
            }
        }

        public void SetGridSpacing(int spacing)
        {
            gridSpacing = spacing > 0 ? spacing : 20; // Minimalny odstęp to 20
            boardPanel.Invalidate();
        }


        public void AddObject(string label, int width, int height, int x, int y, bool positionBottom)
        {
            var rect = new Panel
            {
                Width = width,
                Height = height,
                BackColor = label.Equals("hala", StringComparison.OrdinalIgnoreCase) ? Color.Transparent : Color.LightBlue,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(x, y),
                Tag = label
            };

            // Pobranie bieżącego indeksu obiektu do testów
            int index = boardPanel.Controls.Count;

            var labelControl = new Label
            {
                Text = $"{label} (Index: {index})",
                AutoSize = true,
                Location = new Point(5, 5)
            };

            rect.Controls.Add(labelControl);

            // Obsługa przeciągania i klikania
            rect.MouseDown += Rectangle_MouseDown;
            rect.MouseMove += Rectangle_MouseMove;
            rect.MouseUp += Rectangle_MouseUp;
            rect.DoubleClick += Rectangle_DoubleClick;

            // Dodajemy obiekt do listy
            mapObjects.Add(rect);
            boardPanel.Controls.Add(rect);
            
            if (positionBottom)
            {
                rect.SendToBack();
            }
            else
            {
                rect.BringToFront();
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
                // Obliczanie nowej pozycji obiektu
                _draggedControl.Left += e.X - _dragStartPoint.X;
                _draggedControl.Top += e.Y - _dragStartPoint.Y;
            }
        }

        private void Rectangle_MouseUp(object sender, MouseEventArgs e)
        {
            _draggedControl = null;
        }

        private void Rectangle_DoubleClick(object sender, EventArgs e)
        {
            var clickedPanel = sender as Panel;

            if (clickedPanel != null)
            {
                // Jeśli mamy już zaznaczony obiekt, przywróć jego oryginalny kolor
                if (_selectedPanel != null)
                {
                    _selectedPanel.BackColor = Color.LightBlue; // Domyślny kolor
                }

                // Podświetl kliknięty obiekt
                clickedPanel.BackColor = Color.LightGreen;
                _selectedPanel = clickedPanel; // Zapamiętaj zaznaczony obiekt

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

        private void AddObjectTest_Click(object sender, EventArgs e)
        {
            AddObject("Test Object", 140, 60, 40, 40, false);
        }

        private void buttonAddHall_Click_1(object sender, EventArgs e)
        {
            AddObject("Hala", 200, 100, 20, 20, true);
        }
    }
}