using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel; // Dodaj referencję do ClosedXML

namespace InteractiveMapControl.ObjectData
{
    internal class XlsObjectLoader
    {
        // Klasa reprezentująca dane z pliku XLSX
        public class CsvObject
        {
            public int ObjectID { get; set; }
            public int? ParentID { get; set; }
            public string Name { get; set; }
            public int Level { get; set; }
        }

        // Metoda do wczytywania danych z pliku XLSX
        public List<CsvObject> LoadObjectsFromXlsx(string filePath)
        {
            try
            {
                var objects = new List<CsvObject>();

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
                        objects.Add(new CsvObject
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
                return new List<CsvObject>();
            }
        }

        // Metoda do tworzenia obiektów na podstawie wczytanych danych
        public void CreateObjectsFromXlsxData(
            string filePath,
            Action<string, double, double, double, double, bool, int, int?> addObjectCallback)
        {
            var csvObjects = LoadObjectsFromXlsx(filePath);

            foreach (var csvObj in csvObjects)
            {
                double width = 3;
                double height = 3;
                double labelX = 1;
                double labelY = 1;
                bool positionBottom = csvObj.Level > 1;

                addObjectCallback(
                    csvObj.Name,
                    width,
                    height,
                    labelX,
                    labelY,
                    positionBottom,
                    csvObj.ObjectID,
                    csvObj.ParentID
                );
            }
        }
    }
}
