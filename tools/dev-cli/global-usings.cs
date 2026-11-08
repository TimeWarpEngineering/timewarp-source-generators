#region Purpose
// Global usings for dev-cli
#endregion
#region Design
// ICommand, ICommandHandler, and Unit live in TimeWarp.Mediator as of Nuru 4.
// A static using of Unit collides with Task inside the DevCli package sources, so handlers return Unit.Value.
#endregion

global using System;
global using System.IO;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Collections.Generic;
global using System.Diagnostics;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Xml.Linq;
global using System.Text.RegularExpressions;

global using TimeWarp.Mediator;
global using TimeWarp.Nuru;
global using TimeWarp.Amuru;
global using TimeWarp.Terminal;
global using DevCli;
global using Microsoft.Extensions.DependencyInjection;