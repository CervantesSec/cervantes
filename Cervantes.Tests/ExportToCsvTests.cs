using Cervantes.IFR.Export;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace Cervantes.Tests;

// DeleteDelay is static, so these tests must not run in parallel with anything else that changes it.
[Collection(nameof(ExportToCsvTests))]
public class ExportToCsvTests : IDisposable
{
    private readonly string root = Path.Combine(Path.GetTempPath(), "cervantes-tests-export-" + Guid.NewGuid());
    private readonly TimeSpan originalDelay = ExportToCsv.DeleteDelay;
    private readonly ExportToCsv exporter;

    public ExportToCsvTests()
    {
        Directory.CreateDirectory(Path.Combine(root, "Attachments", "Export"));
        exporter = new ExportToCsv(new TestWebHostEnvironment { WebRootPath = root });
        ExportToCsv.DeleteDelay = TimeSpan.FromMilliseconds(500);
    }

    public void Dispose()
    {
        ExportToCsv.DeleteDelay = originalDelay;
        Directory.Delete(root, true);
    }

    [Fact]
    public async Task DeleteFile_KeepsTheExportUntilTheDelayHasPassed()
    {
        var relative = "Attachments/Export/Vulns-" + Guid.NewGuid() + ".csv";
        var fullPath = Path.Combine(root, relative);
        await File.WriteAllTextAsync(fullPath, "a;b");

        exporter.DeleteFile(relative);

        // The browser fetches the file only after the page has asked for it to be removed.
        Assert.True(File.Exists(fullPath));
        await WaitUntil(() => !File.Exists(fullPath), TimeSpan.FromSeconds(10));
        Assert.False(File.Exists(fullPath));
    }

    [Fact]
    public async Task DeleteFileLater_IgnoresAMissingFolder()
    {
        // A missing folder makes File.Delete throw DirectoryNotFoundException; the delayed delete must swallow it.
        await exporter.DeleteFileLater("Attachments/Export/missing-dir/x.csv");
    }

    private static async Task WaitUntil(Func<bool> condition, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (!condition() && DateTime.UtcNow < deadline)
        {
            await Task.Delay(100);
        }
    }

    private sealed class TestWebHostEnvironment : IWebHostEnvironment
    {
        public string WebRootPath { get; set; } = "";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string ApplicationName { get; set; } = "Cervantes.Tests";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = "";
        public string EnvironmentName { get; set; } = "Production";
    }
}
