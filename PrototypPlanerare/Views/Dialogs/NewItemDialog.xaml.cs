using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PrototypPlanerare.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace PrototypPlanerare.Views.Dialogs
{
    public sealed partial class NewItemDialog : ContentDialog, INotifyPropertyChanged
    {
        // ===== Fields bound in XAML =====
        private string _type = "ECO";
        private string _status = "NotStarted";
        private string _owner = string.Empty;

        public string Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged();
                    // update conditional sections
                    OnPropertyChanged(nameof(EcoAoVisibility));
                    OnPropertyChanged(nameof(CurrentRevVisibility));
                }
            }
        }

        public string Status
        {
            get => _status;
            set { if (_status != value) { _status = value; OnPropertyChanged(); } }
        }

        public string Owner
        {
            get => _owner;
            set
            {
                var v = CapitalizeFirst(value);   // ensure first letter is uppercase
                if (_owner != v)
                {
                    _owner = v;
                    OnPropertyChanged();
                }
            }
        }

        public string EcoNumber { get; set; } = string.Empty;
        public string? AoNumber { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public string Product { get; set; } = string.Empty;
        public string? CurrentRevision { get; set; } = string.Empty;
        public string? NewRevision { get; set; } = string.Empty;
        public double Quantity { get; set; } = 0; // NumberBox uses double

        // ===== Conditional visibility based on Type =====
        public Visibility EcoAoVisibility =>
            Type == "ECO" ? Visibility.Visible : Visibility.Collapsed;

        // Show Current Rev only for ECO
        public Visibility CurrentRevVisibility =>
            Type == "ECO" ? Visibility.Visible : Visibility.Collapsed;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public NewItemDialog()
        {
            InitializeComponent();
            DataContext = this;
            Owner = Environment.UserName;   // triggers CapitalizeFirst(...)
        }

        private static string CapitalizeFirst(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            var t = s.Trim();
            var c = char.ToUpper(t[0]);
            return t.Length == 1 ? c.ToString() : c + t.Substring(1);
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Required across all types
            if (string.IsNullOrWhiteSpace(Customer) || string.IsNullOrWhiteSpace(Product))
            {
                ErrorBar.Title = "Missing required fields";
                ErrorBar.Message = "Customer and Product are required.";
                ErrorBar.IsOpen = true;
                args.Cancel = true;
                return;
            }

            // Type-specific requirements
            if (Type == "ECO")
            {
                if (string.IsNullOrWhiteSpace(EcoNumber))
                {
                    ErrorBar.Title = "Missing ECO";
                    ErrorBar.Message = "ECO is required for Type = ECO.";
                    ErrorBar.IsOpen = true;
                    args.Cancel = true;
                    return;
                }
            }
            else if (Type == "PROTOTYPE")
            {
                if (string.IsNullOrWhiteSpace(NewRevision))
                {
                    ErrorBar.Title = "Missing New Rev";
                    ErrorBar.Message = "New Rev is required for Type = PROTOTYPE.";
                    ErrorBar.IsOpen = true;
                    args.Cancel = true;
                    return;
                }
            }

            Owner = CapitalizeFirst(Owner);
        }

        public Item BuildItem() => new Item
        {
            Date = DateTime.Today,
            EcoNumber = (Type == "ECO" ? EcoNumber?.Trim() : string.Empty),
            AoNumber = (Type == "ECO" ? AoNumber?.Trim() : string.Empty),
            Customer = Customer?.Trim(),
            Product = Product?.Trim(),
            CurrentRevision = (Type == "ECO" ? CurrentRevision?.Trim() : string.Empty),
            NewRevision = NewRevision?.Trim(),
            Quantity = (int)Math.Round(Quantity),
            Type = Type,
            Status = Status,
            CreatedBy = Owner            // already capitalized by the setter
        };
    }
}
