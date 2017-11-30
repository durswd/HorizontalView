using Xamarin.Forms;
using System.Collections.ObjectModel;


namespace HorizontalView
{
    public partial class HorizontalViewPage : ContentPage
    {
        public HorizontalViewPage()
        {
            InitializeComponent();

            var data = new Data();

            for (int i = 0; i < 1000; i++)
            {
                var c = new Content();
                c.Value = i;
                data.Values.Add(c);
            }

            this.BindingContext = data;


            horizontalView.ShowElement(0);
        }

        void Handle_Scrolled(object sender, Xamarin.Forms.ScrolledEventArgs e)
        {
            var s = sender as ScrollView;
            var x = s.ScrollX;

            horizontalView.ShowElement((int)x);
        }

    }

    class Content
    {
        public int Value { get; set; }
    }

    class Data
    {
        public ObservableCollection<Content> Values { get; set; }

        public Data()
        {
            Values = new ObservableCollection<Content>();
        }
    }
}
