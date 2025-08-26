using Domain.Models.ResponseModel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

public class StatementDocument : IDocument
{
    private readonly AccountStatementResponseModel model;

    public StatementDocument(AccountStatementResponseModel model)
    {
        this.model = model;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        var culture = new CultureInfo("en-PK"); // English (Pakistan)  
        //culture.NumberFormat.CurrencySymbol = "PKR"; //
        container.Page(page =>
        {
            page.Margin(30);

            byte[] logoBytes = {
    0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52,
    0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x10, 0x08, 0x02, 0x00, 0x00, 0x00, 0x90, 0x91, 0x68,
    0x36, 0x00, 0x00, 0x00, 0x01, 0x73, 0x52, 0x47, 0x42, 0x00, 0xAE, 0xCE, 0x1C, 0xE9, 0x00, 0x00,
    0x00, 0x04, 0x67, 0x41, 0x4D, 0x41, 0x00, 0x00, 0xB1, 0x8F, 0x0B, 0xFC, 0x61, 0x05, 0x00, 0x00,
    0x00, 0x09, 0x70, 0x48, 0x59, 0x73, 0x00, 0x00, 0x0E, 0xC3, 0x00, 0x00, 0x0E, 0xC3, 0x01, 0xC7,
    0x6F, 0xA8, 0x64, 0x00, 0x00, 0x00, 0x1A, 0x49, 0x44, 0x41, 0x54, 0x38, 0x4F, 0x63, 0x64, 0x60,
    0x60, 0xF8, 0xCF, 0x80, 0x04, 0x98, 0x18, 0x06, 0x1F, 0x00, 0x00, 0x4C, 0x80, 0x01, 0x00, 0x2E,
    0xE2, 0x09, 0x31, 0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82
};

            page.Header().Row(row =>
            {
                // Left: Logo
                row.ConstantColumn(80).Height(50).Image(logoBytes);

                // Right: Company Name
                row.RelativeColumn(3).Stack(stack =>
                {
                    stack.Item().Text("My Company Pvt Ltd")
                        .FontSize(20)
                        .SemiBold()
                        .FontColor(Colors.Blue.Medium);
                });
            });

            //page.Header()
            //    .Text(model.StatementTitle)
            //    .FontSize(18)
            //    .SemiBold().AlignCenter();

            page.Content().Column(col =>
            {
                col.Spacing(5); // <-- adds uniform spacing between all col.Item()
                col.Item().Text(model.StatementTitle).FontSize(16).Bold().Underline();
                col.Item().Text($"Customer: {model.EntityName}");
                col.Item().Text($"From: {model.FromDate:yyyy-MM-dd}  To: {model.ToDate:yyyy-MM-dd}");
                col.Item().Text($"Opening Balance: {model.OpeningBalance.ToString("C0", culture)}");
                col.Item().Text($"Closing Balance: {model.ClosingBalance.ToString("N0")}").FontColor(model.ClosingBalance < 0 ? Colors.Green.Medium : Colors.Red.Medium);

                // Add some space
                col.Spacing(10); // adds 10px gap between all column items
                col.Item().LineHorizontal(1);
                col.Spacing(10); // adds 10px gap between all column items
                // Add some space

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(); // Date
                        cols.RelativeColumn();    // Transaction Type
                        cols.RelativeColumn(); // Quantity 
                        cols.RelativeColumn(); // Price
                        cols.RelativeColumn(); // Debit
                        cols.RelativeColumn(); // Credit
                        cols.RelativeColumn(); // Balance
                    });

                    // Table Header
                    table.Header(header =>
                    {
                        header.Cell().Text("Date").Bold();
                        header.Cell().Text("Transaction").Bold();
                        header.Cell().Text("Quantity").Bold();
                        header.Cell().Text("Price").Bold();
                        header.Cell().Text("Debit").Bold();
                        header.Cell().Text("Credit").Bold();
                        header.Cell().Text("Balance").Bold();
                    });

                    // Transactions
                    foreach (var tx in model.Transactions)
                    {
                        col.Spacing(5);
                        table.Cell().Text(tx.TransactionDate.ToString("yyyy-MM-dd"));
                        table.Cell().Text($"{tx.TransactionType}");
                        table.Cell().Text($"{tx.Quantity}");
                        table.Cell().Text($"{tx.Price}");
                        table.Cell().Text(tx.TransactionDirection == "Debit" ? tx.SignedAmount.ToString("N0") : "");
                        table.Cell().Text(tx.TransactionDirection == "Credit" ? tx.SignedAmount.ToString("N0") : "");
                        //table.Cell().Text(tx.RunningBalance.ToString("C"));
                        var balance = tx.RunningBalance;
                        table.Cell().Text(balance.ToString("N0", culture));
                            //.FontColor(balance < 0 ? Colors.Green.Medium : Colors.Black);
                    }
                });
            });

            page.Footer()
                .AlignRight()
                .Text($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm}");
        });
    }
}