using Cervantes.Web.Helpers;

namespace Cervantes.Tests;

public class ReportHtmlTests
{
    [Fact]
    public void EnsureDocument_WrapsAFragment()
    {
        var html = ReportHtml.EnsureDocument("<style>p{}</style><p>Informe</p>");

        Assert.StartsWith("<!DOCTYPE html>", html);
        Assert.Contains("<meta charset=\"utf-8\">", html);
        Assert.Contains("<body><style>p{}</style><p>Informe</p></body>", html);
    }

    [Fact]
    public void EnsureDocument_KeepsAFullDocument()
    {
        const string full = "<!DOCTYPE HTML><html><head><style>p{}</style></head><body><p>x</p></body></html>";
        Assert.Equal(full, ReportHtml.EnsureDocument(full));
    }

    [Fact]
    public void EnsureDocument_BodyWithAttributes()
    {
        const string full = "<html><body class=\"x\"><p>x</p></body></html>";
        Assert.Equal(full, ReportHtml.EnsureDocument(full));
    }

    [Fact]
    public void EnsureDocument_DoesNotMatchBodyLikeTags()
    {
        Assert.StartsWith("<!DOCTYPE html>", ReportHtml.EnsureDocument("<bodyx>x</bodyx>"));
    }
}
