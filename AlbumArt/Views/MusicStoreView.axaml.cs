using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AlbumArt.Views
{
	public class MusicStoreView : UserControl
	{
		public MusicStoreView()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
