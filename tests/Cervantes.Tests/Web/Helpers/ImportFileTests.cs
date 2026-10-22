using System.Text.Json;
using System.Xml;
using Cervantes.Web.Helpers;
using YamlDotNet.Core;

namespace Cervantes.Tests.Web.Helpers;

public class ImportFileTests
{
    [Fact]
    public void IsUnreadable_ParserErrors()
    {
        Assert.True(ImportFile.IsUnreadable(new JsonException("bad")));
        Assert.True(ImportFile.IsUnreadable(new XmlException("bad")));
        Assert.True(ImportFile.IsUnreadable(new YamlException("bad")));
        Assert.True(ImportFile.IsUnreadable(new InvalidDataException("bad")));
    }

    [Fact]
    public void IsUnreadable_ServerErrorsAreNot()
    {
        Assert.False(ImportFile.IsUnreadable(new InvalidOperationException("db")));
        Assert.False(ImportFile.IsUnreadable(new NullReferenceException()));
    }
}
