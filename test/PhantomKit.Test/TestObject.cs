using System;
using System.Text.Json.Serialization;

namespace PhantomKit.Test;

/// <summary>
///     Nested class used in one test
/// </summary>
[Serializable]
public class TestObject
{
    [JsonInclude] public string ValueA { get; init; } = string.Empty;
    [JsonInclude] public int ValueB { get; init; }
}