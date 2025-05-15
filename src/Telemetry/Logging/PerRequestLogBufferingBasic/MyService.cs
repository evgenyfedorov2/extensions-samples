// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.Extensions.Diagnostics.Buffering;

namespace LogBuffering;

public class MyService
{
    private readonly GlobalLogBuffer _buffer;

    public MyService(GlobalLogBuffer buffer)
    {
        _buffer = buffer;
    }

    public void HandleException(Exception ex)
    {
        _buffer.Flush();

        // After flushing, log buffering will be temporarily suspended (= all logs will be emitted immediately)
        // for the duration specified by AutoFlushDuration.
    }
}
