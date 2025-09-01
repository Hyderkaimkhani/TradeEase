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

            byte[] logoBytes = model.CompanyLogo != null ? model.CompanyLogo : [];

            page.Header().Row(row =>
            {
                // Left: Logo
                //row.ConstantColumn(80).Height(50).Image(logoBytes);
                //row.ConstantColumn(120).Image(logoBytes).FitHeight();
                row.ConstantColumn(120).Height(60).Image(logoBytes).FitArea();

                row.ConstantColumn(10); // spacer column
                                        // Right: Company Name

                row.RelativeColumn().AlignMiddle().AlignCenter().Stack(stack =>
                {
                    stack.Item().Text(model.CompanyName)
                        .FontSize(20)
                        .SemiBold();
                        //.FontColor(Colors.Blue.Medium);
                });

                row.ConstantColumn(120);
            });

            //page.Header()
            //    .Text(model.StatementTitle)
            //    .FontSize(18)
            //    .SemiBold().AlignCenter();

            page.Content().Column(col =>
            {
                col.Spacing(5); // <-- adds uniform spacing between all col.Item()
                col.Item().Text(model.StatementTitle).FontSize(14).Bold().Underline();
                col.Item().Text($"Customer: {model.EntityName}");
                col.Item().Text($"From: {model.FromDate:yyyy-MM-dd}  To: {model.ToDate:yyyy-MM-dd}");
                col.Item().Text($"Opening Balance: {model.OpeningBalance.ToString("N0")}");
                col.Item().Text($"Closing Balance: {model.ClosingBalance.ToString("N0")}").FontColor(model.ClosingBalance < 0 ? Colors.Red.Medium : Colors.Green.Medium);

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
                        table.Cell().Text(tx.TransactionDate.ToString("dd-MM-yyyy"));
                        table.Cell().Text($"{tx.TransactionType}");
                        table.Cell().Text($"{tx.Quantity}");
                        table.Cell().Text($"{tx.Price?.ToString("N0")}");
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