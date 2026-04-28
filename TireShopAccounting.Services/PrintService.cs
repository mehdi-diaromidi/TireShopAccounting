using System;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using TireShopAccounting.Core.Models;

namespace TireShopAccounting.Services
{
    public class PrintService
    {
        public void PrintInvoice(Invoice invoice)
        {
            var printDialog = new PrintDialog();
            
            if (printDialog.ShowDialog() == true)
            {
                var document = CreateInvoiceDocument(invoice);
                printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, "فاکتور فروش");
            }
        }

        public void PreviewInvoice(Invoice invoice, Window owner)
        {
            var document = CreateInvoiceDocument(invoice);
            
            var previewWindow = new Window
            {
                Title = "پیش‌نمایش فاکتور",
                Width = 800,
                Height = 1000,
                Owner = owner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                FlowDirection = FlowDirection.RightToLeft
            };

            var viewer = new DocumentViewer
            {
                Document = document
            };

            previewWindow.Content = viewer;
            previewWindow.ShowDialog();
        }

        private FlowDocument CreateInvoiceDocument(Invoice invoice)
        {
            var document = new FlowDocument
            {
                FontFamily = new FontFamily("Vazir"),
                FontSize = 14,
                FlowDirection = FlowDirection.RightToLeft,
                PagePadding = new Thickness(50),
                ColumnWidth = double.PositiveInfinity
            };

            // هدر فاکتور
            var header = new Paragraph
            {
                TextAlignment = TextAlignment.Center,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 20)
            };
            header.Inlines.Add(new Run("فاکتور فروش لاستیک"));
            document.Blocks.Add(header);

            // خط جداکننده
            document.Blocks.Add(new Paragraph(new Run(new string('─', 80)))
            {
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            });

            // اطلاعات فاکتور
            var infoSection = new Paragraph
            {
                Margin = new Thickness(0, 0, 0, 20)
            };
            infoSection.Inlines.Add(new Run($"شماره فاکتور: {invoice.InvoiceNumber}") { FontWeight = FontWeights.Bold });
            infoSection.Inlines.Add(new LineBreak());
            infoSection.Inlines.Add(new Run($"تاریخ: {invoice.PersianDate}"));
            infoSection.Inlines.Add(new LineBreak());
            infoSection.Inlines.Add(new Run($"مشتری: {invoice.Customer?.Name ?? ""}"));
            infoSection.Inlines.Add(new LineBreak());
            infoSection.Inlines.Add(new Run($"شماره تماس: {invoice.Customer?.Phone ?? ""}"));
            document.Blocks.Add(infoSection);

            // خط جداکننده
            document.Blocks.Add(new Paragraph(new Run(new string('─', 80)))
            {
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 10, 0, 10)
            });

            // جدول اقلام
            var table = new Table
            {
                CellSpacing = 0,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };

            // تعریف ستون‌ها
            table.Columns.Add(new TableColumn { Width = new GridLength(50) }); // ردیف
            table.Columns.Add(new TableColumn { Width = new GridLength(200) }); // نام کالا
            table.Columns.Add(new TableColumn { Width = new GridLength(100) }); // تعداد
            table.Columns.Add(new TableColumn { Width = new GridLength(100) }); // قیمت واحد
            table.Columns.Add(new TableColumn { Width = new GridLength(120) }); // قیمت کل

            var rowGroup = new TableRowGroup();
            table.RowGroups.Add(rowGroup);

            // سطر هدر
            var headerRow = new TableRow { Background = new SolidColorBrush(Color.FromRgb(198, 173, 139)) };
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("ردیف")) { TextAlignment = TextAlignment.Center, FontWeight = FontWeights.Bold }));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("نام کالا")) { TextAlignment = TextAlignment.Center, FontWeight = FontWeights.Bold }));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("تعداد")) { TextAlignment = TextAlignment.Center, FontWeight = FontWeights.Bold }));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("قیمت واحد (تومان)")) { TextAlignment = TextAlignment.Center, FontWeight = FontWeights.Bold }));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("قیمت کل (تومان)")) { TextAlignment = TextAlignment.Center, FontWeight = FontWeights.Bold }));
            rowGroup.Rows.Add(headerRow);

            // ردیف‌های اقلام
            int rowNumber = 1;
            foreach (var item in invoice.Items)
            {
                var row = new TableRow();
                row.Cells.Add(new TableCell(new Paragraph(new Run(rowNumber.ToString())) { TextAlignment = TextAlignment.Center }));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.Product?.DisplayName ?? "")) { TextAlignment = TextAlignment.Right }));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.Quantity.ToString())) { TextAlignment = TextAlignment.Center }));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.Price.ToString("N0"))) { TextAlignment = TextAlignment.Center }));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.TotalPrice.ToString("N0"))) { TextAlignment = TextAlignment.Center }));
                rowGroup.Rows.Add(row);
                rowNumber++;
            }

            document.Blocks.Add(table);

            // جمع کل
            var totalSection = new Paragraph
            {
                Margin = new Thickness(0, 20, 0, 0),
                FontSize = 16
            };
            totalSection.Inlines.Add(new Run($"جمع کل: {invoice.TotalAmount:N0} تومان") { FontWeight = FontWeights.Bold });
            totalSection.Inlines.Add(new LineBreak());
            if (invoice.Discount > 0)
            {
                totalSection.Inlines.Add(new Run($"تخفیف: {invoice.Discount:N0} تومان"));
                totalSection.Inlines.Add(new LineBreak());
                totalSection.Inlines.Add(new Run($"مبلغ قابل پرداخت: {invoice.FinalAmount:N0} تومان") { FontWeight = FontWeights.Bold });
            }
            document.Blocks.Add(totalSection);

            // فوتر
            document.Blocks.Add(new Paragraph(new Run(new string('─', 80)))
            {
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 30, 0, 10)
            });

            var footer = new Paragraph
            {
                Margin = new Thickness(0, 50, 0, 0)
            };
            footer.Inlines.Add(new Run("امضا و مهر فروشنده: ________________"));
            footer.Inlines.Add(new LineBreak());
            footer.Inlines.Add(new LineBreak());
            footer.Inlines.Add(new Run("با تشکر از خرید شما"));
            document.Blocks.Add(footer);

            return document;
        }
    }
}
