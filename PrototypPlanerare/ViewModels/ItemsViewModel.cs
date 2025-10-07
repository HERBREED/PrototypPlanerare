using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using PrototypPlanerare.Data;
using PrototypPlanerare.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PrototypPlanerare.ViewModels
{
    public partial class ItemsViewModel : ObservableObject
    {
        public ObservableCollection<Item> Items { get; } = new();
        private List<Item> _all = new();

        private string? _searchText;
        private int _sortIndex = 0; // 0=newest, 1=oldest, 2=customer, 3=product, 4=status
        private bool _showArchived = false;

        public string? SearchText
        {
            get => _searchText;
            set { if (SetProperty(ref _searchText, value)) ApplyFilter(); }
        }

        public int SortIndex
        {
            get => _sortIndex;
            set { if (SetProperty(ref _sortIndex, value)) ApplyFilter(); }
        }

        public bool ShowArchived
        {
            get => _showArchived;
            set { if (SetProperty(ref _showArchived, value)) ApplyFilter(); }
        }

        public async Task LoadAsync()
        {
            await using var db = new AppDbContext();
            // Pull everything; filter in-memory so toggles/search are instant.
            _all = await db.Items
                .AsNoTracking()
                .OrderByDescending(i => i.Date)
                .ToListAsync();

            ApplyFilter();
        }

        public void ApplyFilter()
        {
            IEnumerable<Item> filtered = _all;

            // Show only archived OR only active
            filtered = ShowArchived
                ? filtered.Where(i => i.IsArchived)
                : filtered.Where(i => !i.IsArchived);

            // Text search
            var q = (SearchText ?? string.Empty).Trim();
            if (q.Length > 0)
            {
                filtered = filtered.Where(i =>
                    (!string.IsNullOrEmpty(i.Type) && i.Type.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(i.Customer) && i.Customer.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(i.Product) && i.Product.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(i.Status) && i.Status.Contains(q, StringComparison.OrdinalIgnoreCase))
                );
            }

            // Sort
            filtered = _sortIndex switch
            {
                0 => filtered.OrderByDescending(i => i.Date ?? DateTime.MinValue),
                1 => filtered.OrderBy(i => i.Date ?? DateTime.MinValue),
                2 => filtered.OrderBy(i => i.Customer),
                3 => filtered.OrderBy(i => i.Product),
                4 => filtered.OrderBy(i => i.Status),
                _ => filtered
            };

            Items.Clear();
            foreach (var it in filtered) Items.Add(it);
        }
    }
}
