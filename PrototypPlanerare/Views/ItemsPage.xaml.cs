using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using PrototypPlanerare.Data;
using PrototypPlanerare.Models;
using PrototypPlanerare.ViewModels;
using PrototypPlanerare.Views.Dialogs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PrototypPlanerare.Views
{
    public sealed partial class ItemsPage : Page
    {
        public ItemsViewModel ViewModel { get; } = new ItemsViewModel();

        public ItemsPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
            this.Loaded += ItemsPage_Loaded;
        }

        private async void ItemsPage_Loaded(object sender, RoutedEventArgs e)
        {
            await SafeLoadAsync();
        }

        private async Task SafeLoadAsync()
        {
            if (ViewModel.Items.Count == 0)
                await ViewModel.LoadAsync();
        }

        // ========= Existing handlers you had =========

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is not Item item) return;
            Frame?.Navigate(typeof(ItemDetailsPage), item);
        }

        private async void NewItem_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new NewItemDialog { XamlRoot = this.XamlRoot };
            var result = await dlg.ShowAsync();
            if (result != ContentDialogResult.Primary) return;

            var item = dlg.BuildItem();

            using var db = new AppDbContext();
            db.Items.Add(item);
            await db.SaveChangesAsync();

            await ViewModel.LoadAsync();
        }

        // ========= Context menu + filter support =========

        // XAML calls this on search box, sort combo, and “Show archived” toggle.
        // Overloads cover different event arg types.
        private async void FilterChanged(object sender, TextChangedEventArgs e) => await RefreshAsync();
        private async void FilterChanged(object sender, SelectionChangedEventArgs e) => await RefreshAsync();
        private async void FilterChanged(object sender, RoutedEventArgs e) => await RefreshAsync();

        private async Task RefreshAsync()
        {
            // Simple + reliable: reload & re-apply VM filtering/sorting.
            await ViewModel.LoadAsync();
        }

        // Open from context menu (same as clicking the row)
        private void OpenItem_Click(object sender, RoutedEventArgs e)
        {
            if (GetItemFromSender(sender) is Item item)
                Frame?.Navigate(typeof(ItemDetailsPage), item);
        }

        private async void ArchiveItem_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is not PrototypPlanerare.Models.Item item) return;

            using var db = new AppDbContext();
            var dbItem = await db.Items.FindAsync(item.Id);
            if (dbItem is null) return;

            dbItem.IsArchived = true;
            await db.SaveChangesAsync();

            await ViewModel.LoadAsync(); // refresh list from DB (respects ShowArchived)
        }

        private async void UnarchiveItem_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is not PrototypPlanerare.Models.Item item) return;

            using var db = new AppDbContext();
            var dbItem = await db.Items.FindAsync(item.Id);
            if (dbItem is null) return;

            dbItem.IsArchived = false;
            await db.SaveChangesAsync();

            await ViewModel.LoadAsync();
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is not PrototypPlanerare.Models.Item item) return;

            var confirm = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Delete item?",
                Content = "This cannot be undone.",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close
            };
            if (await confirm.ShowAsync() != ContentDialogResult.Primary) return;

            using var db = new AppDbContext();
            var dbItem = await db.Items.FindAsync(item.Id);
            if (dbItem is null) return;

            db.Items.Remove(dbItem);
            await db.SaveChangesAsync();

            await ViewModel.LoadAsync();
        }

        // ========= helpers =========

        private static Item? GetItemFromSender(object sender)
        {
            if (sender is MenuFlyoutItem mfi && mfi.CommandParameter is Item cp)
                return cp;

            return (sender as FrameworkElement)?.DataContext as Item;
        }

        private async Task<bool> ConfirmAsync(string title, string message)
        {
            var dlg = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = title,
                Content = message,
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close
            };
            return (await dlg.ShowAsync()) == ContentDialogResult.Primary;
        }
    }
}
