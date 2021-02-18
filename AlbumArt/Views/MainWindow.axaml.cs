using System.Threading.Tasks;
using AlbumArt.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;

namespace AlbumArt.Views
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // When the window is activated, registers a handler for the ShowOpenFileDialog interaction.
            this.WhenActivated(d => d(ViewModel.ShowDialog.RegisterHandler(ShowDialog)));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private async Task ShowDialog(InteractionContext<MusicStoreViewModel, string?> interaction)
        {
            var dialog = new DialogWindow();
            dialog.DataContext = interaction.Input;
            
            var result = await dialog.ShowDialog<string?>(this);
            interaction.SetOutput(result);
        }
    }
}