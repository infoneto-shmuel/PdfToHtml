PdfToHtml is a simple tool to convert pdf to html or text in C# based on itext7 package

usage:
```cs
      string path = "file.pdf";
      PdfDetails pdf = PdfDetailsFactory.Create(path);
      string text = pdf.ToString();
      string html = new PdfHtmlWriter().ConvertPdf(pdf);
```

PdfToHtml is licensed as <a href="/License.md">AGPL</a>
