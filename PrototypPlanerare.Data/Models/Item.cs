using System;
using System.Collections.Generic;

namespace PrototypPlanerare.Models
{
    public class Item
    {
        public int Id { get; set; }

        // Meta
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public List<ItemComment> Comments { get; set; } = new();

        // --- Overview ---
        public DateTime? Date { get; set; }                // "Datum" from your sheet - Överblick
        public string? EcoNumber { get; set; }             // ECO Nummer - Överblick
        public string? AoNumber { get; set; }              // ÄO Nummer - Överblick
        public string? Customer { get; set; }              // Kund - Överblick
        public string? Product { get; set; }               // Produkt - Överblick
        public string? CurrentRevision { get; set; }       // Nuvarande Revision - Överblick
        public string? NewRevision { get; set; }           // Ny Revision - Överblick
        public int? Quantity { get; set; }                 // Antal - Överblick
        public string? Type { get; set; }                  // Typ (ECO/Prototype/Other) - Överblick
        public string Status { get; set; } = "NotStarted"; // Overall status (we'll show a chip later) - Överblick
        public string? Notes { get; set; }                 // General notes/info - Överblick
        public string? CreatedBy { get; set; }

        // --- Purchasing (Inköp) ---
        public DateTime? PurchasingPcbEta { get; set; }            // När PCB bekräftat att komma
        public DateTime? PurchasingMaterialsEta { get; set; }      // När material bekräftat att komma
        public bool PurchasingPcbDrawingReceived { get; set; }     // PCB-ritning: mottagen?
        public bool PurchasingPasteFileReceived { get; set; }      // Pastafil: mottagen?

        // --- Market (Marknad) ---
        public DateTime? MarketKickoffDate { get; set; }   // Uppstartsmöte (Datum)
        public string? MarketQuoteNumber { get; set; }   // Offertnummer
        public bool? MarketQuoted { get; set; }   // Offererad (Ja/Nej)
        public bool? MarketQuoteApproved { get; set; } // Offertstatus (Godkänd/Ej godkänd)
        public string? MarketOwner { get; set; }   // Owner for Market
        public string? MarketNotes { get; set; }   // Notes (Marknad)

        // --- Production ---
        public DateTime? ProductionStartDate { get; set; }
        public string? ProductionToNumber { get; set; }
        public bool ProductionHasSmtPrimary { get; set; }
        public bool ProductionHasSmtSecondary { get; set; }
        public bool ProductionHasPrep { get; set; }
        public bool ProductionHasTht { get; set; }
        public bool ProductionHasEftermontering { get; set; }
        public bool ProductionHasTest { get; set; }

        // --- Planning ---
        public bool? PlanningOrderReceived { get; set; }          // Order från kund: mottagen/ej mottagen
        public DateTime? PlanningConfirmedDeliveryDate { get; set; }  // Bekräftat leveransdatum
        public bool? PlanningExcessMaterial { get; set; }         // Excess material: ja/nej
        public bool? PlanningInvoiced { get; set; }               // Fakturerad: ja/nej
        public string? PlanningOwner { get; set; }                  // Ägare
        public int? PlanningDeliveredQty { get; set; } = 0;


        // Responsibilities
        public string? OwnerMarket { get; set; }           // Ansvarig Marknad
        public string? OwnerEngineering { get; set; }      // Ansvarig Teknik
        public string? OwnerPurchasing { get; set; }       // Ansvarig Inköp
        public string? OwnerPlanning { get; set; }         // Ansvarig Planering

        // Archiving
        public bool IsArchived { get; set; }          // default false
        public DateTime? ArchivedAt { get; set; }     // when it was archived

        // Navigation: checklist steps
        public ICollection<ItemStep> Steps { get; set; } = new List<ItemStep>();
    }
}
