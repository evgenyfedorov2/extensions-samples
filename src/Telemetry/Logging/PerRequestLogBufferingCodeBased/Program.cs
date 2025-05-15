// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;
using LogBuffering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Buffering;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var hostBuilder = Host.CreateApplicationBuilder();

hostBuilder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;
    options.TimestampFormat = "hh:mm:ss";
    options.UseUtcTimestamp = true;
});

// Add the Global buffer to the logging pipeline.
hostBuilder.Logging.AddGlobalBuffer(options =>
{
    options.MaxBufferSizeInBytes = 104857600; // 100 MB
    options.MaxLogRecordSizeInBytes = 51200; // 50 KB
    options.AutoFlushDuration = TimeSpan.FromSeconds(30);
    options.Rules.Add(new LogBufferingFilterRule(
        categoryName: "BufferingDemo",
        logLevel: LogLevel.Information));
    options.Rules.Add(new LogBufferingFilterRule(eventId: 1001));
});

using var app = hostBuilder.Build();

var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger("BufferingDemo");
var buffer = app.Services.GetRequiredService<GlobalLogBuffer>();

for (int i = 1; i < 21; i++)
{
    try
    {
        logger.InformationMessage();

        if(i % 10 == 0)
        {
           throw new Exception("Simulated exception");
        }
    }
    catch (Exception ex)
    {
        logger.ErrorMessage(ex.Message);
        buffer.Flush();
    }

    await Task.Delay(1000).ConfigureAwait(false);
}
