#!/usr/bin/env -S dotnet --
// ═══════════════════════════════════════════════════════════════════════════════
// DEV CLI - timewarp-source-generators DEVELOPMENT TOOL
// ═══════════════════════════════════════════════════════════════════════════════
//
// Usage:
//   As runfile:  dotnet run tools/dev-cli/dev.cs -- <command>
//   As AOT:      ./bin/dev <command>
//
// Run `./bin/dev --help` for available commands.
//
// Telemetry (tools/dev-cli/telemetry.md):
//   UseTelemetry service name is the literal "timewarp-source-generators-dev".
//   Export follows OTEL_EXPORTER_OTLP_ENDPOINT. Unset exports nothing.
//   TRACEPARENT / TRACESTATE join a parent process's trace.
//
// To bootstrap:
//   dotnet run tools/dev-cli/dev.cs -- self-install
//   direnv allow
//   dev --help
// ═══════════════════════════════════════════════════════════════════════════════

#region Purpose
// Entry point for the dev CLI
#endregion
#region Design
// Thin wrapper around TimeWarp.Nuru to execute development commands.
// UseTelemetry is on so a CI job or a ganda walk that shells out to dev
// exports one command span per route, including the DevCli routes. The OTLP
// endpoint stays unset here: a non-literal OtlpEndpoint is ignored, and an
// unset OTEL_EXPORTER_OTLP_ENDPOINT exports nothing. ServiceName is the
// literal timewarp-source-generators-dev. OTEL_SERVICE_NAME overrides it.
// This repo has no ActivitySource of its own, so none is registered.
// IncomingTraceContext restores TRACEPARENT before RunAsync because
// TelemetryBehavior parents its span to Activity.Current.
// NuGetPackageService stays out of ConfigureServices. Its public
// HttpMessageHandler constructor is the greediest one, so source-generated
// DI reports NURU051 unless that handler is registered. The workflow handler
// constructs the parameterless service, which keeps the process-wide cache.
// check-version and release use NuGetVersionService. RepoCheckVersionService
// is not registered: its only constructor needs INuGetPackageService.
#endregion

NuruApp app = NuruApp.CreateBuilder()
  .WithName("dev")
  .WithDescription("Development CLI for timewarp-source-generators")
  .UseTelemetry(options => options.ServiceName = "timewarp-source-generators-dev")
  .ConfigureServices(services =>
  {
    services.AddSingleton<IRepoCleanService, RepoCleanService>();
    services.AddSingleton<NuGetVersionService>();
    services.AddSingleton<IRepoConfigService, RepoConfigService>();
    services.AddSingleton<IPackableProjectService, PackableProjectService>();
  })
  .DiscoverEndpoints()
  .Build();

using IDisposable? incomingTrace = IncomingTraceContext.Restore();
return await app.RunAsync(args);