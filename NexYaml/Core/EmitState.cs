using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Core;

/// <summary>
/// Represents the current emission context during YAML serialization.
/// Used to determine correct formatting, indentation, and structural expectations
/// when emitting nested YAML structures.
/// </summary>
public enum EmitState
{
    /// <summary>
    /// Default state, no active emission context.
    /// </summary>
    None,

    /// <summary>
    /// Inside a block-style sequence (e.g., "- item").
    /// Expects an element to be emitted as a sequence entry.
    /// </summary>
    BlockSequenceEntry,

    /// <summary>
    /// Inside a block-style mapping (e.g., "key: value").
    /// Expects a key to be emitted.
    /// </summary>
    BlockMappingKey,

    /// <summary>
    /// Inside a block-style mapping.
    /// Expects a value after a key.
    /// </summary>
    BlockMappingValue,

    /// <summary>
    /// Inside a flow-style sequence (e.g., "[item1, item2]").
    /// Expects a sequence entry with comma-separated syntax.
    /// </summary>
    FlowSequenceEntry,

    /// <summary>
    /// Inside a flow-style mapping (e.g., "{ key: value }").
    /// Expects a key to be emitted.
    /// </summary>
    FlowMappingKey,

    /// <summary>
    /// Inside a flow-style mapping (e.g., "{ key: value }").
    /// Expects a key to be emitted.
    /// Used for non First Elements
    /// </summary>
    FlowMappingSecondaryKey,



    /// <summary>
    /// Inside a flow-style mapping.
    /// Expects a value after a key.
    /// </summary>
    FlowMappingValue,
}