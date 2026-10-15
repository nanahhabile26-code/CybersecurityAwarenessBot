using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CybersecurityAwarenessBot;

/// <summary>Stores what the bot "remembers" about the user (automatic properties).</summary>
public class UserMemory
{
    public string Name { get; set; } = "";
    public string FavouriteTopic { get; set; } = "";
    public string LastSentiment { get; set; } = "";
    public List<string> TopicsDiscussed { get; } = new();
}
