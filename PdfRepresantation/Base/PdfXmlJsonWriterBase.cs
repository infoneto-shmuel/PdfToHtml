using System;
using PdfRepresantation.Model;
using PdfRepresantation.Model.Pdf;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PdfRepresantation.Interfaces;
using PdfRepresantation.Model.Config;
using PdfRepresantation.Serialization;

namespace PdfRepresantation.Base
{
    public abstract class PdfXmlJsonWriterBase : PdfWriterBase, IPdfWriterEx
    {
        public XmlJsonWriterConfig Config { get; }

        protected PdfXmlJsonWriterBase(XmlJsonWriterConfig config)
        {
            Config = config ?? new XmlJsonWriterConfig();
        }

        protected PdfModel GetValue(PdfDetails pdfDetails)
        {
            var pdfModel = new PdfModel();
            pdfModel.Fonts = pdfDetails.Fonts;
            pdfModel.Pages = new List<PageModel>();
            foreach (var pdfDetail in pdfDetails.Pages)
            {
                pdfModel.Pages.Add(GetPageValue(pdfDetail));
            }
            return pdfModel;
        }

        protected internal PageModel GetPageValue(PageDetails pageDetails)
        {
            var pageModel = new PageModel();
            pageModel.Fonts = pageDetails.Fonts;
            pageModel.Images = pageDetails.Images;
            pageModel.Shapes = pageDetails.Shapes;
            pageModel.PageNumber = pageDetails.PageNumber;
            pageModel.RightToLeft = pageDetails.RightToLeft;
            pageModel.Width = pageDetails.Width;
            pageModel.Height = pageDetails.Height;
            float lastTop = float.MinValue;
            ExtractRows(pageDetails, lastTop, pageModel);
            SeparateTables(pageModel);

            return pageModel;
        }

        private static void SeparateTables(PageModel pageModel)
        {
            var table = pageModel.TablesList[0];
            var tableModels = new List<TableModel>();
            var tableModel = new TableModel();
            tableModels.Add(tableModel);
            if (table.Count > 0)
            {
                var rowModel = new RowModel();
                rowModel.TextLineDetails = table[0];
                tableModel.RowModels.Add(rowModel);
                for (int i = 1; i < table.Count; i++)
                {
                    if (tableModel.RowModels.LastOrDefault()?.TextLineDetails.Count != table[i].Count)
                    {
                        tableModel = new TableModel();
                        tableModels.Add(tableModel);
                    }

                    var model = new RowModel();
                    model.TextLineDetails = table[i];
                    tableModel.RowModels.Add(model);
                }
            }

            pageModel.Tables = tableModels.ToArray();
        }

        private void ExtractRows(PageDetails pageDetails, float lastTop, PageModel pageModel)
        {
            List<List<TextLineDetails>> detailsList = null;
            List<TextLineDetails> row = null;
            foreach (var line in pageDetails.Lines.OrderBy(l => l.Top).ThenBy(l => l.Left))
            {
                if (IsNewRow(line, lastTop))
                {
                    if (detailsList == null)
                    {
                        detailsList = new List<List<TextLineDetails>>();
                        pageModel.TablesList.Add(detailsList);
                    }

                    lastTop = line.Top;
                    row = new List<TextLineDetails>();
                    detailsList.Add(row);
                    row.Add(line);
                }
                else
                {
                    row.Add(line);
                }
            }
        }

        private bool IsNewRow(TextLineDetails line, float lastTop)
        {
            return Math.Abs(line.Top - lastTop) > Config.HeightTolerance;
        }

        public string ConvertPdf(PdfDetails pdf, bool shouldSerializeAll)
        {
            var serializeAll = PdfModel.ShouldSerializeAll;
            try
            {
                PdfModel.ShouldSerializeAll=shouldSerializeAll;
                return ConvertPdf(pdf);
            }
            finally
            {
                PdfModel.ShouldSerializeAll=serializeAll;
            }
        }

        public string ConvertPage(PageDetails page, bool shouldSerializeAll)
        {
            var serializeAll = PdfModel.ShouldSerializeAll;
            try
            {
                PdfModel.ShouldSerializeAll = shouldSerializeAll;
                return ConvertPage(page);
            }
            finally
            {
                PdfModel.ShouldSerializeAll = serializeAll;
            }
        }

        public void SaveAs(PdfDetails pdf, string path, bool shouldSerializeAll = false)
        {
            var content = ConvertPdf(pdf, shouldSerializeAll);
            File.WriteAllText(path, content);
        }

        public TableModel[] ToTables(PdfDetails pdf)
        {
            var tableModels = new List<TableModel>();
            foreach (var page in pdf.Pages)
            {
                tableModels.AddRange(GetPageValue(page).Tables);
            }

            return tableModels.ToArray();
        }
    }
}