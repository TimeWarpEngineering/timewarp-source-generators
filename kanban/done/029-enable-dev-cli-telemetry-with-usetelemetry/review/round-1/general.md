# Round 1 — general
**Date:** 2026-11-09
**Scope reviewed:** branch `task/029-enable-dev-cli-telemetry-with-usetelemetry` (928bc7f) vs `master` merge-base (836f59b); all four `tools/dev-cli/` product files.

## Summary

The change opts the dev CLI into Nuru telemetry with `.UseTelemetry(options => options.ServiceName = "timewarp-source-generators-dev")` (string literal, as required), adds `IncomingTraceContext` and wraps `app.RunAsync(args)` in `using IDisposable? incomingTrace = IncomingTraceContext.Restore();`, turns off `GenerateDocumentationFile` for the dev CLI only so the AOT `self-install` publish stops failing MSB3030, and adds `tools/dev-cli/telemetry.md`. Risk is low: the change is confined to the dev tool, export stays gated on `OTEL_EXPORTER_OTLP_ENDPOINT`, and an absent or invalid `TRACEPARENT` returns `null` without side effects. No issues found.

## Verification

- `incoming-trace-context.cs` is byte-identical to timewarp-nuru `tools/dev-cli/endpoints/incoming-trace-context.cs` (`diff` exit 0), as the task requires. `dev.cs` wiring matches the reference (`timewarp-nuru` `tools/dev-cli/dev.cs:62,73-74`).
- Required usings (`System.Diagnostics`, `System.Runtime.CompilerServices`, `DevCli`) are already global in `tools/dev-cli/global-usings.cs`; the new file is picked up by `<Compile Include="endpoints/**/*.cs" />`.
- No `ActivitySource` or `Meter` exists in any `*.cs` in the repo (grep), so not registering one is correct.
- `GenerateDocumentationFile=false` is scoped to `tools/dev-cli/Directory.Build.props`; the repo-wide `true` in `Directory.Build.props:31` still applies to product projects.
- `dotnet build tools/dev-cli/dev.cs` succeeded; `./bin/dev --capabilities` exited 0.
- Independent OTLP smoke (local HTTP listener on 127.0.0.1:14399, `OTEL_EXPORTER_OTLP_PROTOCOL=http/protobuf`, `TRACEPARENT=00-02902902902902902902902902902902-b029b029b029b029-01`, `./bin/dev check-version`, a DevCli route): `/v1/traces` and `/v1/metrics` both received payloads containing `timewarp-source-generators-dev` and `TimeWarp.Nuru.Behavior`; the trace payload contained the caller trace id and parent span id bytes. `check-version` exited 1 (version already published), matching the implementer's report; the span still exported.
- `telemetry.md` names `OTEL_EXPORTER_OTLP_ENDPOINT`, `OTEL_SERVICE_NAME`, `TRACEPARENT`, `TRACESTATE`, and `OTEL_EXPORTER_OTLP_PROTOCOL`.

## Goal restatement check

The `## Goal (implementer restatement)` matches the original Description and Requirements (literal service name, DevCli routes covered, endpoint gating, TRACEPARENT restore, no repo ActivitySource, capabilities/build/AOT, telemetry.md). No misread.

## Issues

None.
