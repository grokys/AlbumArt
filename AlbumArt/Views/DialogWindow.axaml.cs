using AlbumArt.ViewModels;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;

namespace AlbumArt.Views
{
    public class DialogWindow : ReactiveWindow<MusicStoreViewModel>
    {
        public DialogWindow()
        {
            InitializeComponent();
            
            this.WhenActivated(d => d(ViewModel.Ok.Subscribe(_ => Close(ViewModel.Text))));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}