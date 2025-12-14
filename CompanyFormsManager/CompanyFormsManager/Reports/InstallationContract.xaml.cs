using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace CompanyFormsManager.Reports
{
    /// <summary>
    /// Interaction logic for InstallationContract.xaml
    /// </summary>
    public partial class InstallationContract : Window
    {
        public class Config
        {
            public string BankAccount { get; set; }
        }

        public InstallationContract()
        {
            InitializeComponent();
            var config = LoadConfig();

            BankAccountRun.Text = config.BankAccount;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            PrintFlowDocumentDirectly();
        }

        private void PrintFlowDocumentDirectly()
        {
            FlowDocument originalDocument = MyDocument;

            if (originalDocument == null)
            {
                MessageBox.Show("Błąd: Dokument nie istnieje!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string xaml = XamlWriter.Save(originalDocument);
            StringReader stringReader = new StringReader(xaml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            FlowDocument clonedDocument = (FlowDocument)XamlReader.Load(xmlReader);

            ReplaceTextBoxesWithText(clonedDocument);

            PrintDialog printDialog = new PrintDialog();

            clonedDocument.PageHeight = printDialog.PrintableAreaHeight;
            clonedDocument.PageWidth = printDialog.PrintableAreaWidth;
            clonedDocument.PagePadding = new Thickness(50);
            clonedDocument.ColumnGap = 0;
            clonedDocument.ColumnWidth = printDialog.PrintableAreaWidth;

            printDialog.PrintDocument(((IDocumentPaginatorSource)clonedDocument).DocumentPaginator, "Drukowanie umowy");
        }

        private void ReplaceTextBoxesWithText(FlowDocument document)
        {
            var sectionsToProcess = document.Blocks.OfType<Section>().ToList();

            foreach (var section in sectionsToProcess)
            {
                var blocksToProcess = section.Blocks.OfType<Paragraph>().ToList();

                foreach (var block in blocksToProcess)
                {
                    var inlinesToProcess = block.Inlines.ToList();
                    block.Inlines.Clear();

                    foreach (var inline in inlinesToProcess)
                    {
                        if (inline is InlineUIContainer container && container.Child is TextBox textBox)
                        {
                            if (textBox.Name == "SpecificationTextBox" || textBox.Name == "AdditionalNotesTextBox")
                            {
                                block.Inlines.Add(inline);
                            }
                            else
                            {
                                var run = new Run(textBox.Text)
                                {
                                    FontWeight = textBox.FontWeight,
                                    FontSize = block.FontSize,
                                    Foreground = textBox.Foreground
                                };
                                block.Inlines.Add(run);
                            }
                        }
                        else
                        {
                            block.Inlines.Add(inline);
                        }
                    }
                }
            }
        }
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
                e.Handled = true; // Zatrzymuje dalsze przekazywanie zdarzenia
            }
        }

        private Config LoadConfig()
        {
            // Ścieżka do pliku konfiguracyjnego
            string filePath = "config.json";

            if (File.Exists(filePath))
            {
                // Wczytanie zawartości pliku JSON
                string json = File.ReadAllText(filePath);

                // Deserializacja JSON do obiektu Config
                return JsonConvert.DeserializeObject<Config>(json);
            }

            // Jeśli plik nie istnieje, zwrócimy pustą konfigurację
            return new Config();
        }
    }
}
