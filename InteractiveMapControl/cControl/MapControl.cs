using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using InteractiveMapControl.cControl.Models;
using System.Linq;

/*
2. Możliwość grupowania obiektów
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
        private bool isResizing = false;
        private Point resizeStartPoint;
        private Size originalSize;


        private int gridSpacing = 20;
        private Bitmap gridBitmap;

        public MapControl()
        {
            InitializeComponent();

            backgroundPictureBox.Size = new Size(4035, 4035); // 100m x 100m przy 20px = 0.5m
            backgroundPictureBox.SizeMode = PictureBoxSizeMode.Normal;
            backgroundPictureBox.BackColor = Color.White;

            backgroundPictureBox.Click += BackgroundPictureBox_Click;

            backgroundPictureBox.MouseWheel += BackgroundPictureBox_MouseWheel;
            panelScroll.Scroll += BackgroundPictureBox_Scroll;


            UpdateGrid();
            backgroundPictureBox.Paint += AddAxes_Paint;

            DisplayObjectInfo();



            AddObject("Hala", 500, 200, 25, 10, true, 0);
            //AddObject("Obiekt", 140, 60, 40, 40, false, 1, 0);
            //AddObject("Obiekt", 140, 60, 160, 80, false, 2, 0);
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private void BackgroundPictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            int scrollStep = 0;
            int scrollOffsetYMin = 0;
            int scrollOffsetYMax = 0;

            scrollOffsetYMax = (gridSpacing - 15) / 5 * 1000 + 2752;
            scrollStep = Math.Abs(e.Delta);
            int scrollChange = e.Delta > 0 ? -scrollStep : scrollStep;
            int newScrollOffsetY = scrollOffsetY + scrollChange;

            newScrollOffsetY = Clamp(newScrollOffsetY, scrollOffsetYMin, scrollOffsetYMax);

            var scrollArgs = new ScrollEventArgs(
                e.Delta > 0 ? ScrollEventType.SmallDecrement : ScrollEventType.SmallIncrement,
                scrollOffsetY,
                newScrollOffsetY,
                ScrollOrientation.VerticalScroll
            );

            scrollOffsetY = newScrollOffsetY;

            BackgroundPictureBox_Scroll(sender, scrollArgs);

            if (ModifierKeys.HasFlag(Keys.Control))
            {
                if (e.Delta > 0)
                {
                    gridSpacing = Math.Min(gridSpacing + 5, 50);
                }
                else if (e.Delta < 0)
                {
                    gridSpacing = Math.Max(gridSpacing - 5, 15);
                }
                UpdateGrid();
            }

            DisplayObjectInfo();
        }


        private void UpdateObjectSizes()
        {
            float scaleFactor = gridSpacing / 20.0f; // Obliczamy współczynnik skali

            foreach (var boardObject in boardObjects)
            {
                int newWidth = (int)(boardObject.OriginalSize.Width * scaleFactor);
                int newHeight = (int)(boardObject.OriginalSize.Height * scaleFactor);

                int newX = (int)(boardObject.OriginalLocation.X * scaleFactor);
                int newY = (int)(boardObject.OriginalLocation.Y * scaleFactor);

                newX = ((newX - 25 + gridSpacing / 2) / gridSpacing) * gridSpacing + 25;
                newY = ((newY - 10 + gridSpacing / 2) / gridSpacing) * gridSpacing + 10;

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
            int gridUnits = 200; // 100m / 0.5m = 200 jednostek
            int newSize = Math.Min((gridUnits * gridSpacing) + 35, 10035);

            backgroundPictureBox.Size = new Size(newSize, newSize);


            gridBitmap = GenerateGridBitmap(backgroundPictureBox.Width, backgroundPictureBox.Height, gridSpacing);
            backgroundPictureBox.Image = gridBitmap;

            backgroundPictureBox.Invalidate();
            UpdateObjectSizes();
            if (yAxisPanel != null)
            {
                yAxisPanel.Height = backgroundPictureBox.Height;
            }
            if (xAxisPanel != null)
            {
                xAxisPanel.Width = backgroundPictureBox.Width;
            }
        }

        private Panel yAxisPanel;
        private int scrollOffsetX = 0;
        private Panel xAxisPanel;
        private int scrollOffsetY = 0;

        private void AddAxes_Paint(object sender, PaintEventArgs e)
        {
            if (yAxisPanel == null)
            {
                yAxisPanel = new Panel();
                yAxisPanel.Size = new Size(25, 15 + gridSpacing * 200);
                backgroundPictureBox.Controls.Add(yAxisPanel);
                yAxisPanel.Paint += YAxisPanel_Paint;
            }
            yAxisPanel.Location = new Point(scrollOffsetX, 0);

            if (xAxisPanel == null)
            {
                xAxisPanel = new Panel();
                xAxisPanel.Size = new Size(backgroundPictureBox.Width, 25);
                backgroundPictureBox.Controls.Add(xAxisPanel);
                xAxisPanel.Paint += XAxisPanel_Paint;
            }
            int xAxisYPosition = yAxisPanel.Height;
            xAxisPanel.Location = new Point(0, (panelScroll.Height - (xAxisPanel.Height + 15)+ scrollOffsetY));

            yAxisPanel.BringToFront();
            xAxisPanel.BringToFront();
        }
        private void BackgroundPictureBox_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                scrollOffsetX = e.NewValue;
            }
            else if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                scrollOffsetY = e.NewValue;
            }
            DisplayObjectInfo();
            Invalidate();
        }

        private void YAxisPanel_Paint(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            if (panel == null) return;

            Graphics g = e.Graphics;

            int panelHeight = panel.Height;
            int panelWidth = panel.Width;

            using (Pen verticalLinePen = new Pen(Color.Black, 2))
            using (Pen horizontalLinePen = new Pen(Color.Gray, 1))
            using (Font font = new Font("Arial", 6))
            using (Brush textBrush = new SolidBrush(Color.Black))
            {
                int verticalLineX = panelWidth;

                int maxLabels = 100;
                int maxVerticalHeight = 10 + gridSpacing * (maxLabels * 2);

                g.DrawLine(verticalLinePen, verticalLineX, 10, verticalLineX, maxVerticalHeight);

                int spacing = gridSpacing;
                int labelCounter = 0;

                for (int y = 10; y <= maxVerticalHeight; y += spacing)
                {
                    int horizontalLineWidth = panelWidth / 3;


                    g.DrawLine(horizontalLinePen, verticalLineX - horizontalLineWidth, y, verticalLineX, y);


                    if ((labelCounter % 2) == 0)
                    {
                        int labelValue = labelCounter / 2;
                        string labelText = labelValue.ToString();

                        SizeF textSize = g.MeasureString(labelText, font);

                        g.DrawString(
                            labelText,
                            font,
                            textBrush,
                            verticalLineX - horizontalLineWidth - textSize.Width - 2,
                            y - textSize.Height / 2);
                    }

                    labelCounter++;
                }
            }
        }

        private void XAxisPanel_Paint(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            if (panel == null) return;

            Graphics g = e.Graphics;

            int panelWidth = panel.Width;
            int panelHeight = panel.Height;

            using (Pen horizontalLinePen = new Pen(Color.Black, 2))
            using (Pen verticalLinePen = new Pen(Color.Gray, 1))
            using (Font font = new Font("Arial", 6))
            using (Brush textBrush = new SolidBrush(Color.Black))
            {
                int horizontalLineY = 0;

                int maxLabels = 100;
                int maxHorizontalWidth = 25 + gridSpacing * (maxLabels * 2);

                g.DrawLine(horizontalLinePen, 25, horizontalLineY, maxHorizontalWidth, horizontalLineY);

                int spacing = gridSpacing;
                int labelCounter = 0;

                for (int x = 25; x <= maxHorizontalWidth; x += spacing)
                {
                    int verticalLineHeight = panelHeight / 3;

                    g.DrawLine(verticalLinePen, x, horizontalLineY, x, horizontalLineY + verticalLineHeight);


                    if ((labelCounter % 2) == 0)
                    {
                        int labelValue = labelCounter / 2;
                        string labelText = labelValue.ToString();

                        SizeF textSize = g.MeasureString(labelText, font);

                        g.DrawString(
                            labelText,
                            font,
                            textBrush,
                            x - textSize.Width / 2,
                            horizontalLineY + verticalLineHeight + 2);
                    }

                    if(labelCounter == 100)
                    {
                        //MessageBox.Show(x.ToString());
                    }

                    labelCounter++;
                }
            }
        }


        private Bitmap GenerateGridBitmap(int width, int height, int spacing)
        {
            // Sprawdzam, czy szerokość i wysokość nie przekraczają maksymalnych limitów
            width = Math.Min(width, 10000);
            height = Math.Min(height, 10000);

            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);

                // Pióro dla linii siatki
                using (Pen gridPen = new Pen(Color.LightGray, 1))
                {
                    int offset = 25;

                    // Rysowanie linii pionowych
                    for (int x = offset; x < width; x += spacing)
                    {
                        g.DrawLine(gridPen, x, 10, x, height - 20);
                    }

                    // Rysowanie linii poziomych
                    for (int y = 10; y < height - offset; y += spacing)
                    {
                        g.DrawLine(gridPen, offset, y, width, y);
                    }
                }
            }
            return bitmap;
        }

        public void AddObject(string label, int width, int height, int x, int y, bool positionBottom, int id, int? parentId = null)
        {
            if (x < 25) x = 25;
            if (y < 10) y = 10;

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
            uiPanel.MouseEnter += Rectangle_MouseEnter;
            uiPanel.MouseLeave += Rectangle_MouseLeave;

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
                DefaultBackColor = uiPanel.BackColor
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
            var horizontalVisible = panelScroll.HorizontalScroll.Visible;
            var verticalVisible = panelScroll.VerticalScroll.Visible;
            listBox.Items.Clear();

            foreach (var obj in boardObjects)
            {
                string parentInfo = obj.Parent != null ? obj.Parent.ID.ToString() : "Brak rodzica";
                string childrenInfo = obj.Children.Any() ? string.Join(", ", obj.Children.Select(c => c.ID)) : "Brak dzieci";

                listBox.Items.Add($"UIElement.Location: {obj.UIElement.Location}");
                listBox.Items.Add($"OriginalLocation: {obj.OriginalLocation}");
            }
            
            listBox.Items.Add($"scaleFactor: {gridSpacing / 20.0f}");
            //listBox.Items.Add($"PictureBox size: {backgroundPictureBox.Size}");
            //listBox.Items.Add($"Bitmap size: {gridBitmap.Size}");
            //listBox.Items.Add($"gridSpacing: {gridSpacing}");
            //if (yAxisPanel != null)
            //{
            //    listBox.Items.Add($"yAxisPanel: {yAxisPanel.Location}");
            //    listBox.Items.Add($"xAxisPanel: {xAxisPanel.Location}");
            //    listBox.Items.Add($"scrollOffsetY: {scrollOffsetY}");
            //}

        }
        private void Rectangle_MouseDown(object sender, MouseEventArgs e)
        {
            if (_selectedPanel != null && _selectedPanel == sender as Panel && e.Button == MouseButtons.Left)
            {
                // Sprawdzamy, czy użytkownik kliknął w róg (dolny prawy)
                var panel = _selectedPanel;
                if (panel != null)
                {
                    var bottomRightCorner = new Rectangle(panel.Width - 10, panel.Height - 10, 10, 10);
                    if (bottomRightCorner.Contains(e.Location))
                    {
                        isResizing = true;
                        resizeStartPoint = e.Location;
                        originalSize = panel.Size;
                    }
                }
            }
            else
            {
                _draggedControl = sender as Control;
                _dragStartPoint = e.Location;
            }
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (isResizing && _selectedPanel != null)
            {
                var panel = _selectedPanel;
                if (panel != null)
                {
                    int newWidth = originalSize.Width + (e.X - resizeStartPoint.X);
                    int newHeight = originalSize.Height + (e.Y - resizeStartPoint.Y);

                    newWidth = Math.Max(gridSpacing, newWidth);
                    newHeight = Math.Max(gridSpacing, newHeight);

                    newWidth = (newWidth / gridSpacing) * gridSpacing;
                    newHeight = (newHeight / gridSpacing) * gridSpacing;

                    panel.Size = new Size(newWidth, newHeight);

                    var boardObject = panel.Tag as BoardObject;
                    if (boardObject != null)
                    {
                        boardObject.OriginalSize = new Size(newWidth, newHeight);
                    }
                }
            }
            else if (_draggedControl != null && e.Button == MouseButtons.Left)
            {
                int newX = _draggedControl.Left + e.X - _dragStartPoint.X;
                int newY = _draggedControl.Top + e.Y - _dragStartPoint.Y;

                newX = ((newX - 25 + gridSpacing / 2) / gridSpacing) * gridSpacing + 25;
                newY = ((newY - 10 + gridSpacing / 2) / gridSpacing) * gridSpacing + 10;

                int maxX = backgroundPictureBox.Width - _draggedControl.Width;
                int maxY = backgroundPictureBox.Height - _draggedControl.Height;

                newX = Math.Max(25, Math.Min(newX, maxX));
                newY = Math.Max(10, Math.Min(newY, maxY));

                _draggedControl.Left = newX;
                _draggedControl.Top = newY;

                var draggedObject = _draggedControl.Tag as BoardObject;
                if (draggedObject != null)
                {
                    draggedObject.OriginalLocation = new Point(newX, newY);
                }
            }
        }

        private void Rectangle_MouseUp(object sender, MouseEventArgs e)
        {
            if (isResizing)
            {
                isResizing = false;
            }
            else
            {
                _draggedControl = null;
            }

            DisplayObjectInfo();
        }

        private void Rectangle_MouseEnter(object sender, EventArgs e)
        {
            var panel = sender as Panel;
            if (panel != null && _selectedPanel == panel)
            {
                Cursor = Cursors.SizeNWSE;
            }
        }

        private void Rectangle_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
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
                var clickedObject = clickedPanel.Tag as BoardObject;

                if (_selectedPanel == clickedPanel)
                {
                    if (clickedObject != null)
                    {
                        clickedPanel.BackColor = clickedObject.DefaultBackColor;
                    }
                    _selectedPanel = null;
                }
                else
                {
                    if (_selectedPanel != null)
                    {
                        var previousObject = _selectedPanel.Tag as BoardObject;
                        if (previousObject != null)
                        {
                            _selectedPanel.BackColor = previousObject.DefaultBackColor;
                        }
                    }

                    clickedPanel.BackColor = Color.LightGreen;
                    _selectedPanel = clickedPanel;
                }
            }
        }

        private void BackgroundPictureBox_Click(object sender, EventArgs e)
        {
            if (_selectedPanel != null)
            {
                var selectedObject = _selectedPanel.Tag as BoardObject;

                if (selectedObject != null)
                {
                    _selectedPanel.BackColor = selectedObject.DefaultBackColor;
                }
                _selectedPanel = null;
            }

            DisplayObjectInfo();
        }


    }
}
