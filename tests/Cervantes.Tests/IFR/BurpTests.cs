using System.Text;
using System.Xml;
using Cervantes.IFR.Parsers.Burp;

namespace Cervantes.Tests.IFR;

// Real Burp Suite issues export (DefectDojo sample, see Fixtures/Parsers/NOTICE.md).
public class BurpTests
{
    private static XmlNode Inline(string issueBody)
    {
        var doc = new XmlDocument();
        doc.LoadXml("<issues><issue>" + issueBody + "</issue></issues>");
        return doc.SelectSingleNode("//issue")!;
    }

    private static string B64(string text) => Convert.ToBase64String(Encoding.UTF8.GetBytes(text));

    [Fact]
    public void PocOf_DecodesBase64Request()
    {
        var issue = ParserFixtures.Xml("Burp", "burp-cwe.xml").SelectSingleNode("//issue")!;

        var poc = BurpParser.PocOf(issue);

        Assert.StartsWith("Request:\nGET /test/v4/public/users/fake HTTP/1.1\nHost: exemple.com", poc);
    }

    [Fact]
    public void PocOf_TruncatesLongResponse()
    {
        var issue = Inline("<requestresponse><request base64=\"true\">" + B64("GET / HTTP/1.1") + "</request>" +
                           "<response base64=\"true\">" + B64(new string('x', 5000)) + "</response></requestresponse>");

        var poc = BurpParser.PocOf(issue);

        var response = poc[(poc.IndexOf("Response:\n", StringComparison.Ordinal) + "Response:\n".Length)..];
        Assert.Equal(new string('x', 4000) + "\n[…]", response);
    }

    [Fact]
    public void PocOf_PlainTextRequest()
    {
        var issue = Inline("<requestresponse><request base64=\"false\">POST /login HTTP/1.1</request></requestresponse>");

        Assert.Equal("Request:\nPOST /login HTTP/1.1", BurpParser.PocOf(issue));
    }

    [Fact]
    public void PocOf_BadBase64KeepsTheText()
    {
        var issue = Inline("<requestresponse><request base64=\"true\">not base64!</request></requestresponse>");

        Assert.Equal("Request:\nnot base64!", BurpParser.PocOf(issue));
    }

    [Fact]
    public void PocOf_EmptyWhenNoRequest()
    {
        Assert.Equal("", BurpParser.PocOf(Inline("<name>n</name>")));
    }
}
