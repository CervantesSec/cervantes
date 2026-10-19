using HtmlToOpenXml.IO;
using iText.StyledXmlParser.Resolver.Resource;
using PreMailer.Net.Downloaders;

namespace Cervantes.Web.Helpers;

/// <summary>
/// iText resource loader for report export: the PDF renders only what is inside the HTML. iText decodes inline
/// data: images itself, without calling this; every other URL (remote or local file) is refused.
/// </summary>
public sealed class OfflineResourceRetriever : IResourceRetriever
{
    public Stream? GetInputStreamByUrl(Uri url) => null;

    public byte[]? GetByteArrayByUrl(Uri url) => null;
}

/// <summary>
/// HtmlToOpenXml image loader for report export: the DOCX renders only what is inside the HTML. HtmlToOpenXml decodes
/// inline data: images itself, without calling this; every other URL (remote, local file or relative) is refused.
/// </summary>
public sealed class OfflineWebRequest : IWebRequest
{
    public bool SupportsProtocol(string protocol) => false;

    public Task<Resource?> FetchAsync(Uri requestUri, CancellationToken cancellationToken) => Task.FromResult<Resource?>(null);
}

/// <summary>
/// PreMailer stylesheet loader for report generation: components are inlined only with the CSS they carry themselves
/// (their own CSS and inline style blocks). Every linked stylesheet, remote or local file, is refused.
/// </summary>
public sealed class OfflineCssDownloader : IWebDownloader
{
    public string DownloadString(Uri uri) => "";
}
