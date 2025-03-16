using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;

namespace CompanyFormsManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnInputChanged(object sender, RoutedEventArgs e)
        {
            UpdateTemplatePreview();
        }

        private void UpdateTemplatePreview()
        {
            /*
            ContractNumberRun.Text = ContractNumberInput.Text;
            DealDateRun.Text = DealDateInput.Text;
            CustomerNameRun.Text = CustomerNameInput.Text;
            CustomerAddressRun.Text = CustomerAddressInput.Text;
            CustomerNIPRun.Text = CustomerNIPInput.Text;
            CustomerPhoneNumberRun.Text = CustomerPhoneNumberInput.Text;
            CustomerEmailRun.Text = CustomerEmailInput.Text;
            */
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

            PrintDialog printDialog = new PrintDialog();

            clonedDocument.PageHeight = printDialog.PrintableAreaHeight;
            clonedDocument.PageWidth = printDialog.PrintableAreaWidth;
            clonedDocument.PagePadding = new Thickness(50);
            clonedDocument.ColumnGap = 0;
            clonedDocument.ColumnWidth = printDialog.PrintableAreaWidth;

            printDialog.PrintDocument(((IDocumentPaginatorSource)clonedDocument).DocumentPaginator, "Drukowanie umowy");
        }
    }
}