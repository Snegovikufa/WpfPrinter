using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;

namespace WpfPrintTest
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // A4 Page size
        private const double PageWidth = 1122.77;
        private const double PageHeight = 793.2774803149606;

        public static readonly DependencyProperty ControlsProperty = DependencyProperty.Register(
            "Controls", typeof (ObservableCollection<Control>), typeof (MainWindow), new PropertyMetadata());

        public MainWindow()
        {
            InitializeComponent();
            Controls = new ObservableCollection<Control>();
            ChangeClick(null, null);
        }

        public ObservableCollection<Control> Controls
        {
            get { return (ObservableCollection<Control>) GetValue(ControlsProperty); }
            set { SetValue(ControlsProperty, value); }
        }

        private void ChangeClick(object sender, RoutedEventArgs e)
        {
            Controls.Clear();
            Controls.Add(new TextBox() {Text = "123"});
            Controls.Add(new TextBox() {Text = "456"});
        }

        private void PrintClick(object sender, RoutedEventArgs e)
        {
            FixedDocument doc = new FixedDocument();

            foreach (var control in Controls)
            {
                List<object> list = new List<object> {control};
                CollectionViewSource collectionViewSource = new CollectionViewSource {Source = list};
                var listview = new ListView
                {
                    ItemsSource = collectionViewSource.View,
                    ItemContainerStyle = listView.ItemContainerStyle,
                    BorderThickness = new Thickness(0),
                };
                listview.Measure(new Size());
                listview.Arrange(new Rect(0, 0, PageWidth, PageHeight));

                var fixedPage = new FixedPage
                {
                    Width = PageWidth,
                    Height = PageHeight,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                };

                fixedPage.Children.Add(listview);
                doc.Pages.Add(new PageContent {Child = fixedPage});
            }


            DocumentViewer viewer = new DocumentViewer {Document = doc};

            var w = new Window {Content = viewer};
            w.ShowDialog();

            const string fPath = @"C:\Users\snegovik\Desktop\2.xps";
            using (var xps = new XpsDocument(fPath, FileAccess.ReadWrite))
            {
                var writer = XpsDocument.CreateXpsDocumentWriter(xps);
                writer.Write(doc);

                var v = new DocumentViewer() {Document = xps.GetFixedDocumentSequence()};
                var w2 = new Window() {Content = v};
                w2.ShowDialog();
            }
        }
    }
}