using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;
using InteractiveMapControl.cControl.Models;
using Newtonsoft.Json;

namespace InteractiveMapControl.ObjectData
{
    internal class XlsObjectLoader
    {
        // Klasa reprezentująca dane z pliku XLSX
        public class XlsObject
        {
            public int ObjectID { get; set; }
            public int? ParentID { get; set; }
            public string Name { get; set; }
            public int Level { get; set; }
        }

        // Metoda do wczytywania danych z pliku XLSX
        public List<XlsObject> LoadObjectsFromXlsx(string filePath)
        {
            try
            {
                var objects = new List<XlsObject>();

                // Otwórz plik XLSX
                using (var workbook = new XLWorkbook(filePath))
                {
                    // Załóżmy, że dane znajdują się w pierwszym arkuszu
                    var worksheet = workbook.Worksheet(1); // Pierwszy arkusz

                    // Iteracja po wierszach arkusza
                    foreach (var row in worksheet.RowsUsed().Skip(1)) // Pomijamy pierwszy wiersz (nagłówki)
                    {
                        // Odczytanie danych z komórek
                        var objectId = row.Cell(1).GetValue<int>();

                        // Sprawdzenie, czy ParentID to "NULL" i przypisanie wartości null w takim przypadku
                        var parentIdValue = row.Cell(2).GetValue<string>();
                        int? parentId = null;
                        if (parentIdValue != null && parentIdValue != "NULL")
                        {
                            parentId = row.Cell(2).GetValue<int?>();
                        }

                        var name = row.Cell(3).GetValue<string>();
                        var level = row.Cell(4).GetValue<int>();

                        // Dodanie obiektu do listy
                        objects.Add(new XlsObject
                        {
                            ObjectID = objectId,
                            ParentID = parentId,
                            Name = name,
                            Level = level
                        });
                    }
                }

                return objects;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas wczytywania pliku XLSX: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<XlsObject>();
            }
        }
        public List<BoardObject> LoadBoardObjectsFromJson()
        {
            try
            {
                string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                string filePath = Path.Combine(projectDirectory, "boardObjects.json");

                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<List<BoardObject>>(json);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas wczytywania pliku JSON: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return new List<BoardObject>();
        }
        //    public void CreateObjectsFromXlsxData(
        //string filePath,
        //Action<string, double, double, double, double, int, int, int?> addObjectCallback)
        //    {
        //        var xlsObjects = LoadObjectsFromXlsx(filePath);
        //        double labelX = 0;
        //        double labelY = 0;

        //        foreach (var xlsObj in xlsObjects)
        //        {
        //            double size;
        //            switch (xlsObj.Level)
        //            {
        //                case 1:
        //                    size = 10;
        //                    break;
        //                case 2:
        //                    size = 8;
        //                    break;
        //                case 3:
        //                    size = 6;
        //                    break;
        //                default:
        //                    size = 4; // Domyślny rozmiar dla poziomów powyżej 3
        //                    break;
        //            }

        //            double width = size;
        //            double height = size;



        //            int Level = xlsObj.Level;

        //            addObjectCallback(
        //                xlsObj.Name,
        //                width,
        //                height,
        //                labelX,
        //                labelY,
        //                Level,
        //                xlsObj.ObjectID,
        //                xlsObj.ParentID
        //            );

        //            labelX += 0.5;
        //            labelY += 0.5;
        //        }
        //    }
        public void CreateObjectsFromXlsxData(
        string filePath,
        Action<string, double, double, double, double, int, int, int?> addObjectCallback)
        {
            var xlsObjects = LoadObjectsFromXlsx(filePath);
            var existingObjects = LoadBoardObjectsFromJson(); // Wczytaj obiekty z JSON

            double labelX = 0;
            double labelY = 0;

            foreach (var xlsObj in xlsObjects)
            {
                // Sprawdź, czy obiekt już istnieje w JSON
                var existingObject = existingObjects.FirstOrDefault(obj => obj.ObjectID == xlsObj.ObjectID);

                double width, height;
                int level = xlsObj.Level;

                if (existingObject != null)
                {
                    // Jeśli istnieje, używamy jego wartości
                    width = existingObject.Width;
                    height = existingObject.Height;
                    labelX = existingObject.LocationX;
                    labelY = existingObject.LocationY;
                }
                else
                {
                    // Jeśli nie istnieje, stosujemy domyślne wartości na podstawie Level
                    double size;
                    switch (level)
                    {
                        case 1:
                            size = 10;
                            break;
                        case 2:
                            size = 8;
                            break;
                        case 3:
                            size = 6;
                            break;
                        default:
                            size = 4;
                            break;
                    }

                    width = size;
                    height = size;
                }

                // Wywołanie callbacka do dodania obiektu na planszę
                addObjectCallback(
                    xlsObj.Name,
                    width,
                    height,
                    labelX,
                    labelY,
                    level,
                    xlsObj.ObjectID,
                    xlsObj.ParentID
                );

                labelX += 0.5;
                labelY += 0.5;
            }
        }





    }
}
