using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using PrototypPlanerare.Data;
using PrototypPlanerare.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PrototypPlanerare.Views
{
    public sealed partial class ItemDetailsPage : Page
    {
        private bool _isEditing = false;


        // ===== ADD THIS: fields that cache the two templates =====
        private DataTemplate? _engReadTemplate;
        private DataTemplate? _engEditTemplate;

        public ObservableCollection<string> OwnerSuggestions { get; } = new();
        public ObservableCollection<ItemComment> EngineeringComments { get; } = new();
        public ObservableCollection<ItemComment> PurchasingComments { get; } = new();
        public ObservableCollection<ItemComment> ProductionComments { get; } = new();
        public ObservableCollection<ItemComment> PlanningComments { get; } = new(); 

        private List<string> _allUsers = new();

        // status choices for the edit ComboBox
        public IReadOnlyList<EngineeringStatus> EngineeringStatusOptions { get; } =
            new[] { EngineeringStatus.NotStarted, EngineeringStatus.InProgress, EngineeringStatus.Blocked, EngineeringStatus.Done };

        // helper: try to find a resource without throwing
        private T? GetResource<T>(string key) where T : class
        {
            if (Resources.ContainsKey(key)) return Resources[key] as T;
            if (Application.Current?.Resources?.ContainsKey(key) == true)
                return Application.Current.Resources[key] as T;
            return null;
        }

        public ItemDetailsPage()
        {
            this.InitializeComponent();

            _engReadTemplate = GetResource<DataTemplate>("EngReadTemplate");
            _engEditTemplate = GetResource<DataTemplate>("EngEditTemplate");
        }

        private static readonly string[] DefaultEngineeringTaskTitles =
{
    "BOM / Beredning",
    "Monteringsinstruktion",
    "Stencil",
    "SMT Beredning",
    "SPI Beredning",
    "AOI Beredning",
    "Selektivlödning",
    "Prototyprapport",
    "Klar för 0-serie"
};

        protected override async void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (e.Parameter is Item item)
            {
                this.DataContext = item;
                this.Tag = item;
                _itemId = item.Id;
                await LoadKnownUsersAsync();
                await LoadCommentsAsync();
                await LoadEngineeringTasksAsync();
            }
            base.OnNavigatedTo(e);
        }

        private void SetEditMode(bool editing)
        {
            _isEditing = editing;

            ReadView.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            EditView.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            EditButton.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            SaveButton.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            CancelButton.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;

            // Read/edit controls
            ToggleMarketEditors(editing);
            TogglePurchasingEditors(editing);
            ToggleProductionEditors(editing);
            TogglePlanningEditors(editing);


            // Engineering list template swap
            if (EngineeringList != null)
            {
                if (editing && _engEditTemplate != null)
                    EngineeringList.ItemTemplate = _engEditTemplate;
                else if (!editing && _engReadTemplate != null)
                    EngineeringList.ItemTemplate = _engReadTemplate;
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {

            if (DataContext is not Item item) return;

            item.CreatedBy = CapitalizeFirst(item.CreatedBy);

            try
            {
                using var db = new AppDbContext();
                db.Attach(item);
                db.Entry(item).State = EntityState.Modified;
                await db.SaveChangesAsync();

                // ✅ persist engineering task statuses
                await SaveEngineeringTasksAsync();

                // Refresh both Item and tasks from DB so UI shows what’s saved
                var fresh = await db.Items.AsNoTracking().FirstOrDefaultAsync(i => i.Id == item.Id);
                if (fresh is not null) this.DataContext = fresh;

                await LoadEngineeringTasksAsync();

                SetEditMode(false);

                var dlg = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Saved",
                    Content = "Changes saved.",
                    PrimaryButtonText = "OK"
                };
                await dlg.ShowAsync();
            }
            catch (Exception ex)
            {
                var dlg = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Save failed",
                    Content = ex.Message,
                    PrimaryButtonText = "OK"
                };
                await dlg.ShowAsync();
            }
        }


        private async void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Revert the main Item
            if (DataContext is Item item)
            {
                using var db = new AppDbContext();
                var fresh = await db.Items.AsNoTracking().FirstOrDefaultAsync(i => i.Id == item.Id);
                if (fresh != null)
                    this.DataContext = fresh;
            }

            // Revert Engineering task changes (discard in-memory edits)
            await LoadEngineeringTasksAsync();

            SetEditMode(false);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Frame?.CanGoBack == true)
                Frame.GoBack();
        }
        public ObservableCollection<ItemComment> Comments { get; } = new();
        public ObservableCollection<ItemComment> OverviewComments { get; } = new();
        public ObservableCollection<ItemComment> MarketComments { get; } = new();
        public ObservableCollection<EngineeringTask> EngineeringTasks { get; } = new();

        private int _itemId;

        private async Task LoadCommentsAsync()
        {
            using var db = new AppDbContext();
            var all = await db.ItemComments
                .Where(c => c.ItemId == _itemId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            Comments.Clear();
            OverviewComments.Clear();
            MarketComments.Clear();
            EngineeringComments.Clear();
            PurchasingComments.Clear();
            ProductionComments.Clear();
            PlanningComments.Clear();

            foreach (var c in all)
            {
                Comments.Add(c);
                var section = string.IsNullOrWhiteSpace(c.Section) ? "Overview" : c.Section;

                if (section.Equals("Market", StringComparison.OrdinalIgnoreCase)) MarketComments.Add(c);
                else if (section.Equals("Engineering", StringComparison.OrdinalIgnoreCase)) EngineeringComments.Add(c);
                else if (section.Equals("Purchasing", StringComparison.OrdinalIgnoreCase)) PurchasingComments.Add(c);
                else if (section.Equals("Production", StringComparison.OrdinalIgnoreCase)) ProductionComments.Add(c);
                else if (section.Equals("Planning", StringComparison.OrdinalIgnoreCase)) PlanningComments.Add(c);
                else OverviewComments.Add(c);
            }
        }

        private async void AddComment_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var text = NewCommentBox.Text?.Trim();
            if (string.IsNullOrEmpty(text)) return;

            // Capitalize first letter of the Windows username (fallback if you don't have a helper)
            var author = Environment.UserName;
            if (!string.IsNullOrWhiteSpace(author))
                author = char.ToUpper(author[0]) + (author.Length > 1 ? author.Substring(1) : string.Empty);

            var comment = new ItemComment
            {
                ItemId = _itemId,
                Author = author,
                CreatedAt = DateTime.Now,
                Text = text,
                Section = "Overview"   // <-- important: separate thread
            };

            using var db = new AppDbContext();
            db.ItemComments.Add(comment);
            await db.SaveChangesAsync();

            NewCommentBox.Text = string.Empty;

            // Show instantly in the Overview list
            OverviewComments.Insert(0, comment);

            // Optional: keep legacy list in sync while we transition
            Comments.Insert(0, comment);
        }

        private async void AddMarketComment_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var text = MarketNewCommentBox.Text?.Trim();
            if (string.IsNullOrEmpty(text)) return;

            // Capitalize first letter of Windows username
            var author = Environment.UserName;
            if (!string.IsNullOrWhiteSpace(author))
                author = char.ToUpper(author[0]) + (author.Length > 1 ? author.Substring(1) : string.Empty);

            var comment = new ItemComment
            {
                ItemId = _itemId,
                Author = author,
                CreatedAt = DateTime.Now,
                Text = text,
                Section = "Market"   // <— key: separate Market thread
            };

            using var db = new AppDbContext();
            db.ItemComments.Add(comment);
            await db.SaveChangesAsync();

            MarketNewCommentBox.Text = string.Empty;

            // Show instantly in Market list
            MarketComments.Insert(0, comment);

            // Optional: if you still keep the legacy list around during transition
            if (Comments != null) Comments.Insert(0, comment);
        }


        private void Edit_Click(object sender, RoutedEventArgs e) => SetEditMode(true);

        private async void DeleteComment_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is not ItemComment comment)
                return;

            var confirm = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Delete comment?",
                Content = "This cannot be undone.",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close
            };
            var result = await confirm.ShowAsync();
            if (result != ContentDialogResult.Primary) return;

            // Remove from database
            using (var db = new AppDbContext())
            {
                var toDelete = await db.ItemComments.FindAsync(comment.Id);
                if (toDelete != null)
                {
                    db.ItemComments.Remove(toDelete);
                    await db.SaveChangesAsync();
                }
            }

            // Normalize section
            var section = (comment.Section ?? "Overview").Trim();
            // Remove from the correct UI collection
            if (section.Equals("Overview", StringComparison.OrdinalIgnoreCase))
            {
                var local = OverviewComments.FirstOrDefault(c => c.Id == comment.Id) ?? comment;
                OverviewComments.Remove(local);
            }
            else if (section.Equals("Market", StringComparison.OrdinalIgnoreCase))
            {
                var local = MarketComments.FirstOrDefault(c => c.Id == comment.Id) ?? comment;
                MarketComments.Remove(local);
            }
            else if (section.Equals("Engineering", StringComparison.OrdinalIgnoreCase))
            {
                var local = EngineeringComments.FirstOrDefault(c => c.Id == comment.Id) ?? comment;
                EngineeringComments.Remove(local);
            }
            else if (section.Equals("Purchasing", StringComparison.OrdinalIgnoreCase))
            {
                var local = PurchasingComments.FirstOrDefault(c => c.Id == comment.Id) ?? comment;
                PurchasingComments.Remove(local);
            }
            else if (section.Equals("Production", StringComparison.OrdinalIgnoreCase))
            {
                var local = ProductionComments.FirstOrDefault(c => c.Id == comment.Id) ?? comment;
                ProductionComments.Remove(local);
            }
            else if (section.Equals("Planning", StringComparison.OrdinalIgnoreCase))
            {
                var local = PlanningComments.FirstOrDefault(c => c.Id == comment.Id) ?? comment;
                PlanningComments.Remove(local);
            }

            // Also remove from the legacy combined list if you still bind anywhere to it
            var legacy = Comments.FirstOrDefault(c => c.Id == comment.Id) ?? comment;
            Comments.Remove(legacy);
        }

        // Populate Owner combobox
        private async Task LoadKnownUsersAsync()
        {
            using var db = new AppDbContext();

            var createdBy = await db.Items
                .Select(i => i.CreatedBy)
                .Where(s => s != null && s != "")
                .Distinct()
                .ToListAsync();

            var commenters = await db.ItemComments
                .Select(c => c.Author)
                .Where(s => s != null && s != "")
                .Distinct()
                .ToListAsync();

            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var s in createdBy) set.Add(s);
            foreach (var s in commenters) set.Add(s);
            set.Add(Environment.UserName);

            _allUsers = set.OrderBy(s => s).ToList();
        }
        private void OwnerBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only react to actual typing; ignore programmatic and selection changes
            if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
                return;

            var query = sender.Text?.Trim() ?? string.Empty;

            IEnumerable<string> results =
                string.IsNullOrEmpty(query)
                ? _allUsers.Take(10)
                : _allUsers.Where(u => u.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                           .Take(10);

            OwnerSuggestions.Clear();
            foreach (var u in results)
                OwnerSuggestions.Add(u);
        }
        private void OwnerBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is string name)
                sender.Text = CapitalizeFirst(name);   // normalize selection
        }

        private void OwnerBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            sender.Text = CapitalizeFirst(sender.Text); // normalize on Enter/submit
        }

        private void OwnerBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is AutoSuggestBox box)
                box.Text = CapitalizeFirst(box.Text);   // normalize when leaving the field
        }

        //Capital letter force
        private static string CapitalizeFirst(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            var t = s.Trim();
            var first = char.ToUpper(t[0], CultureInfo.CurrentCulture);
            return t.Length == 1 ? first.ToString() : first + t.Substring(1);
        }
        private void QtyBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (DataContext is Item item)
            {
                // Force integer + non-negative
                var v = (int)Math.Round(sender.Value);
                if (v < 0) v = 0;

                // Only write if changed to avoid extra binding churn
                if (item.Quantity != v)
                    item.Quantity = v;
            }
        }
        private void ToggleMarketEditors(bool editing)
        {
            // read blocks
            MarketRead_Kickoff.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            MarketRead_QuoteNo.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            MarketRead_Quoted.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            MarketRead_Approved.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            MarketRead_Owner.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;

            // editors
            MarketEdit_Kickoff.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            MarketEdit_QuoteNo.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            MarketEdit_Quoted.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            MarketEdit_Approved.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            MarketEdit_Owner.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;

            // notes area
            if (MarketNotesPanel != null)
                MarketNotesPanel.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            if (MarketNotesHint != null)
                MarketNotesHint.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;

            // hide row dividers while editing
            var sepVis = editing ? Visibility.Collapsed : Visibility.Visible;
            if (MktSep0 != null) MktSep0.Visibility = sepVis;
            if (MktSep1 != null) MktSep1.Visibility = sepVis;
            if (MktSep2 != null) MktSep2.Visibility = sepVis;
            if (MktSep3 != null) MktSep3.Visibility = sepVis;
            if (MktSep4 != null) MktSep4.Visibility = sepVis;

            // allow adding comments only in view mode (same behavior)
            MarketAddRow.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        }

        private async Task LoadEngineeringTasksAsync()
        {
            if (_itemId <= 0) return;

            using var db = new AppDbContext();

            // 1) Load existing tasks
            var list = await db.EngineeringTasks
                .Where(t => t.ItemId == _itemId)
                .OrderBy(t => t.Id)
                .AsNoTracking()
                .ToListAsync();

            // 2) If none exist, create the defaults once
            if (list.Count == 0)
            {
                foreach (var title in DefaultEngineeringTaskTitles)
                {
                    db.EngineeringTasks.Add(new EngineeringTask
                    {
                        ItemId = _itemId,
                        Title = title,
                        Status = EngineeringStatus.NotStarted   // ✅ enum, not string
                    });
                }
                await db.SaveChangesAsync();

                // Reload after insert (so we get IDs and correct order)
                list = await db.EngineeringTasks
                    .Where(t => t.ItemId == _itemId)
                    .OrderBy(t => t.Id)
                    .AsNoTracking()
                    .ToListAsync();
            }

            // 3) Push to the UI collection
            EngineeringTasks.Clear();
            foreach (var t in list)
                EngineeringTasks.Add(t);
        }
        private async Task SaveEngineeringTasksAsync()
        {
            if (EngineeringTasks.Count == 0) return;

            using var db = new AppDbContext();

            var ids = EngineeringTasks.Select(t => t.Id).ToList();
            var dbTasks = await db.EngineeringTasks
                                  .Where(t => ids.Contains(t.Id))
                                  .ToListAsync();

            foreach (var dbTask in dbTasks)
            {
                var uiTask = EngineeringTasks.First(t => t.Id == dbTask.Id);
                dbTask.Status = uiTask.Status; // enum copy
            }

            await db.SaveChangesAsync();
        }
        private async void AddEngineeringComment_Click(object sender, RoutedEventArgs e)
        {
            var text = EngNewCommentBox.Text?.Trim();
            if (string.IsNullOrEmpty(text)) return;

            var author = Environment.UserName;
            if (!string.IsNullOrWhiteSpace(author))
                author = char.ToUpper(author[0]) + (author.Length > 1 ? author.Substring(1) : string.Empty);

            var comment = new ItemComment
            {
                ItemId = _itemId,
                Author = author,
                CreatedAt = DateTime.Now,
                Text = text,
                Section = "Engineering"
            };

            using var db = new AppDbContext();
            db.ItemComments.Add(comment);
            await db.SaveChangesAsync();

            EngNewCommentBox.Text = string.Empty;

            EngineeringComments.Insert(0, comment);
            Comments.Insert(0, comment); // optional legacy sync
        }
        private async void AddPurchasingComment_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var text = PurchNewCommentBox.Text?.Trim();
            if (string.IsNullOrEmpty(text)) return;

            var author = CapitalizeFirst(Environment.UserName);

            var comment = new ItemComment
            {
                ItemId = _itemId,
                Author = author,
                CreatedAt = DateTime.Now,
                Text = text,
                Section = "Purchasing"
            };

            using var db = new AppDbContext();
            db.ItemComments.Add(comment);
            await db.SaveChangesAsync();

            PurchNewCommentBox.Text = string.Empty;

            PurchasingComments.Insert(0, comment);
            // optional: keep the legacy aggregate list in sync
            Comments.Insert(0, comment);
        }
        private void TogglePurchasingEditors(bool editing)
        {
            // read values
            PurchRead_PcbDate.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            PurchRead_MaterialDate.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            PurchRead_PcbDrawing.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            PurchRead_PasteFile.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;

            // editors
            PurchEdit_PcbDate.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            PurchEdit_MaterialDate.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            PurchEdit_PcbDrawing.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            PurchEdit_PasteFile.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;

            // notes: same behavior as other tabs
            PurchasingNotesPanel.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            PurchasingAddRow.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            PurchasingNotesHint.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;

            // dividers hidden in edit mode (consistent with Overview)
            PurchSep0.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            PurchSep1.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            PurchSep2.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            PurchSep3.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void AddProductionComment_Click(object sender, RoutedEventArgs e)
        {
            var text = ProdNewCommentBox.Text?.Trim();
            if (string.IsNullOrEmpty(text)) return;

            var author = Environment.UserName;
            if (!string.IsNullOrWhiteSpace(author))
                author = char.ToUpper(author[0]) + (author.Length > 1 ? author.Substring(1) : string.Empty);

            var comment = new ItemComment
            {
                ItemId = _itemId,
                Author = author,
                CreatedAt = DateTime.Now,
                Text = text,
                Section = "Production"
            };

            using var db = new AppDbContext();
            db.ItemComments.Add(comment);
            await db.SaveChangesAsync();

            ProdNewCommentBox.Text = string.Empty;
            ProductionComments.Insert(0, comment);
            Comments.Insert(0, comment); // optional legacy
        }

        private void ToggleProductionEditors(bool editing)
        {
            // read blocks
            ProdRead_Start.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            ProdRead_ToNumber.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            ProdRead_SmtPrim.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            ProdRead_SmtSec.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            ProdRead_Prep.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            ProdRead_Tht.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            ProdRead_Efter.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            ProdRead_Test.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;

            // editors
            ProdEdit_Start.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            ProdEdit_ToNumber.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            ProdEdit_SmtPrim.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            ProdEdit_SmtSec.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            ProdEdit_Prep.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            ProdEdit_Tht.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            ProdEdit_Efter.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            ProdEdit_Test.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;

            // notes: show hint in edit, thread in read
            ProductionNotesHint.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            ProductionNotesPanel.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;

            // row dividers hidden in edit (to match Overview behavior)
            var sepVis = editing ? Visibility.Collapsed : Visibility.Visible;
            ProdSep0.Visibility = sepVis;
            ProdSep1.Visibility = sepVis;
            ProdSep2.Visibility = sepVis;
            ProdSep3.Visibility = sepVis;
            ProdSep4.Visibility = sepVis;
            ProdSep5.Visibility = sepVis;
            ProdSep6.Visibility = sepVis;
            ProdSep7.Visibility = sepVis;
        }

        private async void AddPlanningComment_Click(object sender, RoutedEventArgs e)
        {
            var text = PlanNewCommentBox.Text?.Trim();
            if (string.IsNullOrEmpty(text)) return;

            var author = Environment.UserName;
            if (!string.IsNullOrWhiteSpace(author))
                author = char.ToUpper(author[0]) + (author.Length > 1 ? author[1..] : "");

            var comment = new ItemComment
            {
                ItemId = _itemId,
                Author = author,
                CreatedAt = DateTime.Now,
                Text = text,
                Section = "Planning"
            };

            using var db = new AppDbContext();
            db.ItemComments.Add(comment);
            await db.SaveChangesAsync();

            PlanNewCommentBox.Text = string.Empty;
            PlanningComments.Insert(0, comment);
            Comments.Insert(0, comment); // legacy combined list, if still used anywhere
        }

        private void TogglePlanningEditors(bool editing)
        {
            // READ blocks
            PlanRead_OrderReceived.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            PlanRead_ConfirmedDate.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            PlanRead_Excess.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            PlanRead_DeliveredQty.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            PlanRead_Invoiced.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            PlanRead_Owner.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;

            // EDIT controls
            PlanEdit_OrderReceived.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            PlanEdit_ConfirmedDate.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            PlanEdit_Excess.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            PlanEdit_DeliveredQty.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            PlanEdit_Invoiced.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            PlanEdit_Owner.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;

            // Notes visibility/hint (consistent with other tabs)
            PlanningNotesPanel.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            PlanningNotesHint.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;

            // Hide dividers in edit mode (consistent look)
            var sepVis = editing ? Visibility.Collapsed : Visibility.Visible;
            PlanSep0.Visibility = sepVis;
            PlanSep1.Visibility = sepVis;
            PlanSep2.Visibility = sepVis;
            PlanSep3.Visibility = sepVis;
            PlanSep4.Visibility = sepVis;
            PlanSep5.Visibility = sepVis;
        }

        private void PlanEdit_DeliveredQty_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (DataContext is Item item)
            {
                var v = (int)Math.Round(sender.Value);
                if (v < 0) v = 0;
                if (item.PlanningDeliveredQty != v)
                    item.PlanningDeliveredQty = v;
            }
        }
    }
}