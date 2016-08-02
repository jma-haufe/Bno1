using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Bno1.Controls
{
    public sealed partial class AccordionItem : UserControl
    {
        public UIElement SubContent
        {
            get { return (UIElement)GetValue(SubContentProperty); }
            set { SetValue(SubContentProperty, value); }
        }
        public static readonly DependencyProperty SubContentProperty = DependencyProperty.Register(nameof(Content), typeof(UIElement), typeof(AccordionItem), new PropertyMetadata(null));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(AccordionItem), new PropertyMetadata(string.Empty));

        public AccordionItem()
        {
            this.InitializeComponent();
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {

        }
    }
}
