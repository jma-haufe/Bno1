using System.Xml.Serialization;
using Windows.UI.Xaml;

namespace transmate.DataService
{
    public class CheckListItem
    {
        public string Name { get; set; }
        public string Link { get; set; }

        public Visibility HasLink { get { return string.IsNullOrEmpty(this.Link) ? Visibility.Collapsed : Visibility.Visible; } }

        [XmlIgnore]
        public bool IsChecked { get; set; }
    }
}
