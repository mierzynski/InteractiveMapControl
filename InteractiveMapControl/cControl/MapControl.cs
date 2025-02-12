using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using InteractiveMapControl.cControl.Models;
using System.Linq;
using InteractiveMapControl.ObjectData;
using System.IO;
//using System.Reflection.Emit;


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
        private int previousGridSpacing = 20;
        private Bitmap gridBitmap;

        private Timer _blinkTimer = new Timer();
        private bool _blinkState = true; // Czy trójkąt jest aktualnie widoczny?


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

            InitializeGroupVisibilityComboBox();

            DisplayObjectInfo();

            _blinkTimer.Interval = 500;
            _blinkTimer.Tick += BlinkTimer_Tick;


            // Ścieżka do pliku CSV
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ObjectData", "Obiekty.xlsx");
            var csvLoader = new XlsObjectLoader();
            csvLoader.CreateObjectsFromXlsxData(filePath, AddObject);

            //AddObject("Hala", 2, 2, 1, 1, true, 0);
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

            //Zauważyłem zależność pomiędzy zmianą scrollOffsetYMax - zawsze jest mniejszy o 483 niezależnie od wartości gridSpacing
            scrollOffsetYMax = backgroundPictureBox.Height - 483;
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
                if (gridSpacing != previousGridSpacing)
                {
                    UpdateGrid();
                }
            }

            DisplayObjectInfo();
        }


        private void UpdateObjectSizes()
        {
            int differenceGridSpacing = 0;
            float scaleFactor = gridSpacing / 20.0f;

            if (gridSpacing != previousGridSpacing)
            {
                differenceGridSpacing = gridSpacing - previousGridSpacing;
                previousGridSpacing = gridSpacing;
                foreach (var boardObject in boardObjects)
                {
                    int newWidth = ConvertSizeToPixels(boardObject.Width, boardObject.Height, gridSpacing).X;
                    int newHeight = ConvertSizeToPixels(boardObject.Width, boardObject.Height, gridSpacing).Y;

                    Point labelPositionInPixels = ConvertLabelPositionToPixels(boardObject.LocationX, boardObject.LocationY, gridSpacing);

                    boardObject.UIElement.Location = new Point(labelPositionInPixels.X, labelPositionInPixels.Y);
                    boardObject.UIElement.Size = new Size(newWidth, newHeight);
                }
            }

        }

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

                    //if(labelCounter == 100)
                    //{
                    //    //MessageBox.Show(x.ToString());
                    //}

                    labelCounter++;
                }
            }
        }
        public Point ConvertLabelPositionToPixels(double labelX, double labelY, int gridSpacing)
        {
            int pixelX = 25 + (int)((labelX * 2) * gridSpacing);
            int pixelY = 10 + (int)((labelY * 2) * gridSpacing);
            return new Point(pixelX, pixelY);
        }

        public PointF ConvertPixelsToLabelPosition(int pixelX, int pixelY, int gridSpacing)
        {
            double labelX = (pixelX + scrollOffsetX - 25) / (double)(2 * gridSpacing);
            double labelY = (pixelY + scrollOffsetY - 10) / (double)(2 * gridSpacing);
            return new PointF((float)labelX, (float)labelY);
        }

        public Point ConvertSizeToPixels(double width, double height, int gridSpacing)
        {
            int pixelWidth = (int)(width * 2 * gridSpacing);
            int pixelHeight = (int)(height * 2 * gridSpacing);

            return new Point(pixelWidth, pixelHeight);
        }

        public (double width, double height) ConvertSizeFromPixels(int pixelWidth, int pixelHeight, int gridSpacing)
        {
            double width = pixelWidth / (2.0 * gridSpacing);
            double height = pixelHeight / (2.0 * gridSpacing);

            return (width, height);
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

        public void AddObject(
            string label,
            double width,
            double height,
            double labelX,
            double labelY,
            int level,
            int id,
            int? parentId = null
            )
        {
            Point pixelPosition = ConvertLabelPositionToPixels(labelX, labelY, gridSpacing);

            int x = pixelPosition.X;
            int y = pixelPosition.Y;
            int widthPX = ConvertSizeToPixels(width, height, gridSpacing).X;
            int heightPX = ConvertSizeToPixels(width, height, gridSpacing).Y;

            if (x < 25) x = 25;
            if (y < 10) y = 10;

            var uiPanel = new Panel
            {
                Width = widthPX,
                Height = heightPX,
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
                parentObject = boardObjects.FirstOrDefault(obj => obj.ObjectID == parentId.Value);
            }

            var boardObject = new BoardObject
            {
                ObjectID = id,
                Name = label,
                Parent = parentObject,
                Category = label,
                Group = "Default",
                UIElement = uiPanel,
                Level = level,
                LocationX = labelX,
                LocationY = labelY,
                Width = width,
                Height = height,
                DefaultBackColor = uiPanel.BackColor
            };

            if (parentObject != null)
            {
                parentObject.Children.Add(boardObject);
            }

            uiPanel.Tag = boardObject;

            boardObjects.Add(boardObject);

            UpdateZIndices();
            DisplayObjectInfo();
        }

        //private void UpdateZIndices()
        //{
        //    var sortedObjects = boardObjects.OrderBy(obj => obj.Level).ToList();

        //    foreach (var obj in sortedObjects)
        //    {
        //        if (obj.UIElement is Control control)
        //        {
        //            control.BringToFront();
        //        }
        //    }
        //}
        private void UpdateZIndices()
        {
            var sortedObjects = new List<BoardObject>();

            foreach (var obj in boardObjects.Where(o => o.Parent == null).OrderBy(o => o.Level))
            {
                AddObjectWithChildren(sortedObjects, obj);
            }

            foreach (var obj in sortedObjects)
            {
                if (obj.UIElement is Control control)
                {
                    control.BringToFront();
                }
            }
        }

        private void AddObjectWithChildren(List<BoardObject> sortedList, BoardObject parent)
        {
            sortedList.Add(parent);

            var children = boardObjects.Where(o => o.Parent == parent).OrderBy(o => o.Level).ToList();

            foreach (var child in children)
            {
                AddObjectWithChildren(sortedList, child);
            }
        }


        private void DisplayObjectInfo()
        {
            listBox.Items.Clear();

            if (_selectedPanel != null)
            {
                var selectedObject = _selectedPanel.Tag as BoardObject;
                if (selectedObject != null)
                {
                    string parentID = selectedObject.Parent?.ObjectID.ToString() ?? "Brak";
                    string parentName = selectedObject.Parent?.Name ?? "Brak";

                    listBox.Items.Add($"Nazwa obiektu: {selectedObject.Name}");
                    listBox.Items.Add($"ID obiektu: {selectedObject.ObjectID}");
                    listBox.Items.Add("-----------------------------");
                    listBox.Items.Add($"Pozycja (X, Y): ({selectedObject.LocationX}, {selectedObject.LocationY})");
                    listBox.Items.Add($"Poziom (Level): {selectedObject.Level}");
                    if (selectedObject.UIElement.Parent != null)
                    {
                        int zIndex = selectedObject.UIElement.Parent.Controls.GetChildIndex(selectedObject.UIElement);
                        listBox.Items.Add($"Z-index: {zIndex}");
                    }
                    else
                    {
                        listBox.Items.Add("Z-index: Brak nadrzędnego kontenera");
                    }
                    listBox.Items.Add("-----------------------------");
                    listBox.Items.Add($"Szerokość: {selectedObject.Width}m");
                    listBox.Items.Add($"Wysokość: {selectedObject.Height}m");
                    listBox.Items.Add("-----------------------------");
                    listBox.Items.Add($"Rodzic: {parentName}");
                    listBox.Items.Add("-----------------------------");
                    if (selectedObject.Children != null && selectedObject.Children.Any())
                    {
                        listBox.Items.Add("Dzieci obiektu:");
                        foreach (var child in selectedObject.Children)
                        {
                            listBox.Items.Add($"Nazwa: {child.Name}");
                        }
                        listBox.Items.Add("-----------------------------");
                    }
                    else
                    {
                        listBox.Items.Add("Brak dzieci.");
                        listBox.Items.Add("-----------------------------");
                    }
                }
            }
            else
            {
                listBox.Items.Add($"Rozmiar planszy: {backgroundPictureBox.Size}");
                listBox.Items.Add($"Odstęp siatki (gridSpacing): {gridSpacing}");

                if (xAxisPanel != null)
                {
                    listBox.Items.Add($"Panel osi X: {xAxisPanel.Location}");
                    listBox.Items.Add($"Przesunięcie scrolla Y: {scrollOffsetY}");
                }
            }
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

                    var boardObject = panel.Tag as BoardObject;
                    if (boardObject != null)
                    {
                        int minWidth = gridSpacing;
                        int minHeight = gridSpacing;

                        if (boardObject.Children.Any())
                        {
                            int maxChildRight = boardObject.Children.Max(child => child.UIElement.Right);
                            int maxChildBottom = boardObject.Children.Max(child => child.UIElement.Bottom);

                            minWidth = maxChildRight - panel.Left;
                            minHeight = maxChildBottom - panel.Top;
                        }

                        if (boardObject.Parent != null && boardObject.Parent.UIElement != null)
                        {
                            var parentBounds = boardObject.Parent.UIElement.Bounds;
                            int maxWidth = parentBounds.Width - (panel.Left - parentBounds.Left);
                            int maxHeight = parentBounds.Height - (panel.Top - parentBounds.Top);

                            newWidth = Math.Min(newWidth, maxWidth);
                            newHeight = Math.Min(newHeight, maxHeight);
                        }

                        newWidth = Math.Max(newWidth, minWidth);
                        newHeight = Math.Max(newHeight, minHeight);

                        panel.Size = new Size(newWidth, newHeight);

                        boardObject.Width = ConvertSizeFromPixels(newWidth, newHeight, gridSpacing).width;
                        boardObject.Height = ConvertSizeFromPixels(newWidth, newHeight, gridSpacing).height;

                        // Odśwież panel, aby trójkąt został narysowany w nowym rozmiarze
                        panel.Invalidate();
                    }
                }
            }
            else if (_draggedControl != null && e.Button == MouseButtons.Left)
            {
                int newX = _draggedControl.Left + e.X - _dragStartPoint.X;
                int newY = _draggedControl.Top + e.Y - _dragStartPoint.Y;

                newX = ((newX - 25 + gridSpacing / 2) / gridSpacing) * gridSpacing + 25;
                newY = ((newY - 10 + gridSpacing / 2) / gridSpacing) * gridSpacing + 10;

                var draggedObject = _draggedControl.Tag as BoardObject;
                if (draggedObject != null)
                {
                    if (draggedObject.Parent != null && draggedObject.Parent.UIElement != null)
                    {
                        var parentBounds = draggedObject.Parent.UIElement.Bounds;
                        int parentLeft = parentBounds.Left;
                        int parentTop = parentBounds.Top;
                        int parentRight = parentBounds.Right;
                        int parentBottom = parentBounds.Bottom;

                        int maxX = parentRight - _draggedControl.Width;
                        int maxY = parentBottom - _draggedControl.Height;

                        newX = Math.Max(parentLeft, Math.Min(newX, maxX));
                        newY = Math.Max(parentTop, Math.Min(newY, maxY));
                    }
                    else
                    {
                        int maxX = backgroundPictureBox.Width - _draggedControl.Width;
                        int maxY = backgroundPictureBox.Height - _draggedControl.Height;

                        newX = Math.Max(25, Math.Min(newX, maxX));
                        newY = Math.Max(10, Math.Min(newY, maxY));
                    }

                    int deltaX = newX - _draggedControl.Left;
                    int deltaY = newY - _draggedControl.Top;

                    _draggedControl.Left = newX;
                    _draggedControl.Top = newY;

                    PointF labelPosition = ConvertPixelsToLabelPosition(newX, newY, gridSpacing);
                    draggedObject.LocationX = labelPosition.X;
                    draggedObject.LocationY = labelPosition.Y;
                    draggedObject.UIElement.Location = new Point(newX, newY);

                    MoveChildrenRecursively(draggedObject, deltaX, deltaY);

                    // Odśwież panel, aby trójkąt został narysowany w nowej pozycji
                    _draggedControl.Invalidate();
                }
            }
        }
        private void MoveChildrenRecursively(BoardObject parentObject, int deltaX, int deltaY)
        {
            if (parentObject.Children != null && parentObject.Children.Any())
            {
                foreach (var child in parentObject.Children)
                {
                    if (child.UIElement != null)
                    {
                        child.UIElement.Left += deltaX;
                        child.UIElement.Top += deltaY;

                        PointF childLabelPosition = ConvertPixelsToLabelPosition(child.UIElement.Left, child.UIElement.Top, gridSpacing);
                        child.LocationX = childLabelPosition.X;
                        child.LocationY = childLabelPosition.Y;

                        MoveChildrenRecursively(child, deltaX, deltaY);
                    }
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
        private void BlinkTimer_Tick(object sender, EventArgs e)
        {
            if (_selectedPanel != null)
            {
                var selectedObject = _selectedPanel.Tag as BoardObject;
                if (selectedObject != null)
                {
                    _blinkState = !_blinkState;
                    selectedObject.ShowResizeHandle = _blinkState;
                    _selectedPanel.Invalidate();
                }
            }
            else
            {
                _blinkTimer.Stop();
            }
        }

        private void InitializePanel(Panel panel)
        {
            panel.Paint -= Panel_Paint;
            panel.Paint += Panel_Paint;
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            if (panel == null) return;

            var boardObject = panel.Tag as BoardObject;
            if (boardObject == null || !boardObject.ShowResizeHandle) return;

            using (var brush = new SolidBrush(Color.Red))
            {
                int triangleSize = 15;
                Point[] trianglePoints =
                {
            new Point(panel.Width - triangleSize, panel.Height),
            new Point(panel.Width, panel.Height - triangleSize),
            new Point(panel.Width, panel.Height)
        };

                e.Graphics.FillPolygon(brush, trianglePoints);
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
                    clickedObject.ShowResizeHandle = false;
                    _selectedPanel = null;
                    _blinkTimer.Stop();
                }
                else
                {
                    if (_selectedPanel != null)
                    {
                        var previousObject = _selectedPanel.Tag as BoardObject;
                        if (previousObject != null)
                        {
                            _selectedPanel.BackColor = previousObject.DefaultBackColor;
                            previousObject.ShowResizeHandle = false;
                        }
                    }

                    clickedPanel.BackColor = Color.LightGreen;
                    _selectedPanel = clickedPanel;

                    clickedObject.ShowResizeHandle = true;
                    InitializePanel(clickedPanel);

                    _blinkState = true;
                    _blinkTimer.Start();
                }

                clickedPanel.Invalidate();
                DisplayObjectInfo();
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
                    selectedObject.ShowResizeHandle = false;
                }
                _selectedPanel = null;
            }

            DisplayObjectInfo();
        }

        private void groupVisibilityByLevelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedOption = groupVisibilityByLevelComboBox.SelectedItem.ToString();

            if (selectedOption == "All")
            {
                // Jeśli wybrano "All", pokaż wszystkie obiekty
                UpdateVisibilityByLevels(null);
            }
            else if (int.TryParse(selectedOption, out int selectedLevel))
            {
                // Jeśli wybrano konkretny Level, pokaż tylko te obiekty
                UpdateVisibilityByLevels(new List<int> { selectedLevel });
            }
        }

        private void UpdateVisibilityByLevels(List<int> selectedLevels)
        {
            foreach (var obj in boardObjects)
            {
                if (obj.UIElement is Control control)
                {
                    control.Visible = (selectedLevels == null || selectedLevels.Contains(obj.Level));
                }
            }
        }
        private void InitializeGroupVisibilityComboBox()
        {
            groupVisibilityByLevelComboBox.Items.Add("All");
            groupVisibilityByLevelComboBox.Items.Add("1");
            groupVisibilityByLevelComboBox.Items.Add("2");
            groupVisibilityByLevelComboBox.Items.Add("3");
            groupVisibilityByLevelComboBox.Items.Add("4");

            groupVisibilityByLevelComboBox.SelectedIndex = 0; // Domyślnie "All"
        }

    }
}
