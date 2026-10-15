# Cybersecurity Awareness Bot — Part 2 (WPF GUI)

A WPF desktop chatbot that teaches users about online safety.
Builds on Part 1 (voice greeting, ASCII art, name greeting, validation) and adds keyword recognition,
random responses, conversation flow, memory, sentiment detection and error handling.

## Requirements
- Windows 10/11
- .NET 8 SDK (or Visual Studio 2022 with the ".NET desktop development" workload)

## Run
1. Copy your Part 1 recording to `Assets/greeting.wav` (the file is copied to the output folder automatically).
2. `dotnet run` — or open the `.csproj` in Visual Studio and press F5.
3. Enter your name, then chat or use the Quick Topics buttons.

## Try these
| You type | What happens |
|---|---|
| `Tell me about password safety` | Keyword recognition → random tip |
| `Give me a phishing tip` | Random response (never repeats back-to-back) |
| `Another tip` / `Tell me more` / `I'm confused` | Follow-up on the current topic |
| `I'm interested in privacy` | Memory: stored and recalled later |
| `I'm worried about online scams` | Sentiment: empathy + a tip straight away |
| `What do you remember?` | Memory recall |
| `asdfgh` or empty input | Friendly default response, no crash |
| `exit` | Goodbye and close |

## Structure
| File | Purpose |
|---|---|
| `MainWindow.xaml(.cs)` | GUI layout, chat bubbles, typing effect |
| `ChatEngine.cs` | Intent matching (delegates), flow, memory, random picks |
| `TopicLibrary.cs` / `Topic.cs` | Dictionary + Lists of keywords and responses |
| `SentimentDetector.cs` | Sentiment keywords and replies |
| `UserMemory.cs` | Remembered user details (auto-properties) |
| `SoundManager.cs` | Plays the WAV greeting |
| `AsciiLogo.cs` | ASCII logo from Part 1 |

## Releases (Part 2 needs at least 2)
```
git tag -a v1.0.0 -m "Part 1: console chatbot"      # tag your Part 1 commit
git tag -a v2.0.0 -m "Part 2: WPF GUI chatbot"
git push --tags
```
Then create a GitHub Release from each tag with notes.

## CI
GitHub Actions builds the project on every push (`.github/workflows/dotnet.yml`).
Add a screenshot of the green tick here.

## Video
YouTube (unlisted): _add link_
