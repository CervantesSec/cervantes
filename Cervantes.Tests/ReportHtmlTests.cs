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
    [Fact]
    public void MoveHeadStylesToBody_MovesTheStyle()
    {
        Assert.Equal("<html><head><meta charset=\"utf-8\"></head><body><style>pre{}</style><p>x</p></body></html>",
            ReportHtml.MoveHeadStylesToBody("<html><head><meta charset=\"utf-8\"><style>pre{}</style></head><body><p>x</p></body></html>"));
    }

    [Fact]
    public void MoveHeadStylesToBody_KeepsOrder()
    {
        Assert.Equal("<html><head></head><body><style>a{}</style><style>b{}</style><p>x</p></body></html>",
            ReportHtml.MoveHeadStylesToBody("<html><head><style>a{}</style><style>b{}</style></head><body><p>x</p></body></html>"));
    }

    [Fact]
    public void MoveHeadStylesToBody_BodyWithAttributes()
    {
        Assert.Equal("<html><head></head><body class=\"r\"><style>a{}</style><p>x</p></body></html>",
            ReportHtml.MoveHeadStylesToBody("<html><head><style>a{}</style></head><body class=\"r\"><p>x</p></body></html>"));
    }

    [Fact]
    public void MoveHeadStylesToBody_FragmentUnchanged()
    {
        const string fragment = "<header>h</header><style>a{}</style><p>x</p>";
        Assert.Equal(fragment, ReportHtml.MoveHeadStylesToBody(fragment));
    }

    [Fact]
    public void MoveHeadStylesToBody_NoHeadStyleUnchanged()
    {
        const string html = "<html><head><title>t</title></head><body><style>a{}</style><p>x</p></body></html>";
        Assert.Equal(html, ReportHtml.MoveHeadStylesToBody(html));
    }
}
