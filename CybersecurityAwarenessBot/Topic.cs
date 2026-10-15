using System;
using System.Collections.Generic;

namespace CybersecurityAwarenessBot;

/// <summary>One cybersecurity topic: its trigger keywords and its pool of responses.</summary>
// random short tips
// used for "tell me more"
// used with memory recall
public class Topic
{
    public string Name { get; init; } = "";
    public string[] Keywords { get; init; } = Array.Empty<string>();
    public List<string> Tips { get; init; } = new();      
    public List<string> Details { get; init; } = new();   
    public string Personal { get; init; } = "";           
}
