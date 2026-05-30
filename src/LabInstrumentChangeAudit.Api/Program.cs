using System.Text.Json;
using LabInstrumentChangeAudit.Api;

var app = LabInstrumentChangeAuditApplication.BuildApp(args);

if (args.Contains("--prerender"))
{
    await SiteBuilder.WriteAsync();
    return;
}

if (args.Contains("--demo"))
{
    Console.WriteLine(JsonSerializer.Serialize(AnalysisService.Summary(), new JsonSerializerOptions { WriteIndented = true }));
    Console.WriteLine(JsonSerializer.Serialize(SampleData.InstrumentLane, new JsonSerializerOptions { WriteIndented = true }));
    return;
}

app.Run();

public partial class Program;

public static class LabInstrumentChangeAuditApplication
{
    public static WebApplication BuildApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.MapGet("/", () => Results.Content(RenderService.Overview(), "text/html"));
        app.MapGet("/instrument-lane", () => Results.Content(RenderService.InstrumentLane(), "text/html"));
        app.MapGet("/change-log", () => Results.Content(RenderService.ChangeLog(), "text/html"));
        app.MapGet("/control-posture", () => Results.Content(RenderService.ControlPosture(), "text/html"));
        app.MapGet("/verification", () => Results.Content(RenderService.Verification(), "text/html"));
        app.MapGet("/docs", () => Results.Content(RenderService.Docs(), "text/html"));

        app.MapGet("/api/dashboard/summary", () => Results.Json(AnalysisService.Summary()));
        app.MapGet("/api/instrument-lane", () => Results.Json(SampleData.InstrumentLane));
        app.MapGet("/api/change-log", () => Results.Json(SampleData.Payload.Gaps));
        app.MapGet("/api/control-posture", () => Results.Json(SampleData.AuditPackets));
        app.MapGet("/api/verification", () => Results.Json(new[]
        {
            "Synthetic instrument change and audit evidence only; no patient, lab, or proprietary biotech data is published.",
            "Firmware review, calibration freshness, SOP acknowledgment, maintenance continuity, and QA attestation are modeled as operator surfaces.",
            "This repo demonstrates biotech quality workflow depth without claiming CLIA, GxP, FDA, or clinical compliance."
        }));
        app.MapGet("/api/sample", () => Results.Text(RenderService.Sample(), "application/json"));

        return app;
    }
}

public static class SiteBuilder
{
    public static async Task WriteAsync()
    {
        var root = FindRepoRoot();
        var siteDir = Path.Combine(root, "site");
        Directory.CreateDirectory(siteDir);

        var pages = new Dictionary<string, string>
        {
            ["index.html"] = RenderService.Overview(),
            [Path.Combine("instrument-lane", "index.html")] = RenderService.InstrumentLane(),
            [Path.Combine("change-log", "index.html")] = RenderService.ChangeLog(),
            [Path.Combine("control-posture", "index.html")] = RenderService.ControlPosture(),
            [Path.Combine("verification", "index.html")] = RenderService.Verification(),
            [Path.Combine("docs", "index.html")] = RenderService.Docs()
        };

        foreach (var (relative, html) in pages)
        {
            var target = Path.Combine(siteDir, relative);
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            await File.WriteAllTextAsync(target, html);
        }

        var apiDir = Path.Combine(siteDir, "api");
        Directory.CreateDirectory(Path.Combine(apiDir, "dashboard"));
        await File.WriteAllTextAsync(Path.Combine(apiDir, "dashboard", "summary.json"), JsonSerializer.Serialize(AnalysisService.Summary(), new JsonSerializerOptions { WriteIndented = true }));
        await File.WriteAllTextAsync(Path.Combine(apiDir, "instrument-lane.json"), JsonSerializer.Serialize(SampleData.InstrumentLane, new JsonSerializerOptions { WriteIndented = true }));
        await File.WriteAllTextAsync(Path.Combine(apiDir, "change-log.json"), JsonSerializer.Serialize(SampleData.Payload.Gaps, new JsonSerializerOptions { WriteIndented = true }));
        await File.WriteAllTextAsync(Path.Combine(apiDir, "control-posture.json"), JsonSerializer.Serialize(SampleData.AuditPackets, new JsonSerializerOptions { WriteIndented = true }));
        await File.WriteAllTextAsync(Path.Combine(apiDir, "verification.json"), JsonSerializer.Serialize(new[]
        {
            "Synthetic instrument change and audit evidence only; no patient, lab, or proprietary biotech data is published.",
            "Firmware review, calibration freshness, SOP acknowledgment, maintenance continuity, and QA attestation are modeled as operator surfaces.",
            "This repo demonstrates biotech quality workflow depth without claiming CLIA, GxP, FDA, or clinical compliance."
        }, new JsonSerializerOptions { WriteIndented = true }));
        await File.WriteAllTextAsync(Path.Combine(apiDir, "sample.json"), RenderService.Sample());

        const string domain = "instruments.kineticgain.com";
        await File.WriteAllTextAsync(
            Path.Combine(siteDir, "robots.txt"),
            $"User-agent: *{Environment.NewLine}Allow: /{Environment.NewLine}Sitemap: https://{domain}/sitemap.xml{Environment.NewLine}");
        await File.WriteAllTextAsync(Path.Combine(siteDir, "sitemap.xml"), """
<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
  <url><loc>https://instruments.kineticgain.com/</loc></url>
  <url><loc>https://instruments.kineticgain.com/instrument-lane/</loc></url>
  <url><loc>https://instruments.kineticgain.com/change-log/</loc></url>
  <url><loc>https://instruments.kineticgain.com/control-posture/</loc></url>
  <url><loc>https://instruments.kineticgain.com/verification/</loc></url>
  <url><loc>https://instruments.kineticgain.com/docs/</loc></url>
</urlset>
""");
        await File.WriteAllTextAsync(Path.Combine(siteDir, "CNAME"), domain + Environment.NewLine);
    }

    private static string FindRepoRoot()
    {
        var current = AppContext.BaseDirectory;
        for (var i = 0; i < 8; i++)
        {
            if (File.Exists(Path.Combine(current, "lab-instrument-change-audit.sln")))
            {
                return current;
            }

            current = Directory.GetParent(current)?.FullName
                ?? throw new DirectoryNotFoundException("Unable to resolve repo root.");
        }

        throw new DirectoryNotFoundException("Unable to resolve repo root.");
    }
}
