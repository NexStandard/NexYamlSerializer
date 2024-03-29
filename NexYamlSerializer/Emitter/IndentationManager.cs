﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlSerializer.Emitter;
internal class IndentationManager
{
    public int CurrentIndentLevel { get; private set; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IncreaseIndent()
    {
        CurrentIndentLevel++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DecreaseIndent()
    {
        if (CurrentIndentLevel > 0)
            CurrentIndentLevel--;
    }
}
