using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityAwarenessBot;

/// <summary>All topic data lives here (dictionary of topics, lists of responses).</summary>
public static class TopicLibrary
{
    public static Dictionary<string, Topic> Build()
    {
        var topics = new List<Topic>
        {
            new()
            {
                Name = "password safety",
                Keywords = new[] { "password", "passphrase", "passcode", "credential" },
                Tips = new()
                {
                    "Use a strong, unique password for each account. Avoid personal details like birthdays or pet names.",
                    "Longer is stronger. A passphrase of four or more random words is hard to crack and easy to remember.",
                    "Use a password manager so you don't have to remember or reuse passwords.",
                    "Never share your password, not even with someone claiming to be from your bank or IT support."
                },
                Details = new()
                {
                    "Attackers reuse leaked passwords on other sites. That's why a unique password per account matters: one breach won't unlock everything.",
                    "A strong password mixes length, randomness and variety. Aim for 12+ characters and avoid common words or patterns like 'Password123'.",
                    "Change a password immediately if you suspect a breach, and turn on two-factor authentication for extra protection."
                },
                Personal = "make sure your most important accounts (email and banking) have your strongest passwords."
            },
            new()
            {
                Name = "phishing",
                Keywords = new[] { "phishing", "phish", "suspicious email", "fake email", "suspicious link", "suspicious message" },
                Tips = new()
                {
                    "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
                    "Hover over links before clicking. If the address looks odd, don't open it.",
                    "Watch for urgency such as 'act now or your account will be closed'. It's a classic phishing trick.",
                    "Check the sender's address carefully. Phishers often swap or add a single letter.",
                    "Never click links in suspicious emails or messages. Go to the official website directly instead."
                },
                Details = new()
                {
                    "Phishing messages try to trick you into giving away passwords, card numbers or OTPs. They often have spelling errors, generic greetings and unexpected attachments.",
                    "If you're unsure, contact the organisation using a phone number or website you already trust, not the one in the message.",
                    "Report phishing emails to your email provider or IT team, then delete them."
                },
                Personal = "treat every unexpected link or attachment with suspicion, even from known contacts."
            },
            new()
            {
                Name = "scams",
                Keywords = new[] { "scam", "fraud", "fraudster", "too good to be true", "sim swap" },
                Tips = new()
                {
                    "If an offer sounds too good to be true, it probably is. Be wary of prizes, giveaways and 'guaranteed' returns.",
                    "Banks will never ask for your PIN, password or OTP over the phone or by SMS.",
                    "Scammers create pressure. Slow down, and verify any request for money through a trusted channel.",
                    "Be careful with unexpected payment requests, especially via EFT, gift cards or crypto."
                },
                Details = new()
                {
                    "Common scams include fake job offers, romance scams, fake invoices and impersonation of banks or SARS. They all rely on trust and urgency.",
                    "If you think you've been scammed, contact your bank immediately, change your passwords and report it to the SAPS or the relevant authority.",
                    "A sudden loss of cellphone signal can signal a SIM swap. Contact your network provider straight away."
                },
                Personal = "tell friends and family about common scams as well. Awareness spreads protection."
            },
            new()
            {
                Name = "privacy",
                Keywords = new[] { "privacy", "personal information", "personal data", "data protection", "popia", "tracking" },
                Tips = new()
                {
                    "Review the privacy settings on your social media accounts and limit who can see your posts.",
                    "Share as little personal information online as you can. Oversharing helps scammers guess passwords and answer security questions.",
                    "Check which apps have access to your camera, microphone and location, and remove what they don't need.",
                    "Think before posting. Photos can reveal your home, school or workplace."
                },
                Details = new()
                {
                    "Your personal data has real value to criminals. Limiting what you share reduces the risk of identity theft.",
                    "In South Africa, POPIA gives you rights over how organisations use your personal information.",
                    "Use private browsing, tracker blockers and a VPN on public networks for an extra layer of privacy."
                },
                Personal = "you might want to review the security settings on your accounts."
            },
            new()
            {
                Name = "safe browsing",
                Keywords = new[] { "safe browsing", "browsing", "browser", "https", "public wifi", "wifi", "wi-fi", "website" },
                Tips = new()
                {
                    "Always check that a website uses HTTPS (the padlock) before entering information.",
                    "Avoid logging into banking or shopping accounts on public Wi-Fi, or use a VPN.",
                    "Keep your browser updated and only install extensions from trusted sources.",
                    "Don't download software from unofficial websites. Use the official source."
                },
                Details = new()
                {
                    "HTTPS encrypts data between you and the site, but it doesn't guarantee the site is honest. Scam sites can use HTTPS too, so check the address carefully.",
                    "Type important web addresses yourself or use bookmarks instead of clicking links from emails or ads.",
                    "Pop-ups claiming your device is infected are almost always fake. Close the tab and don't call the number shown."
                },
                Personal = "regularly clear saved logins and review the extensions installed in your browser."
            },
            new()
            {
                Name = "malware",
                Keywords = new[] { "malware", "virus", "ransomware", "spyware", "trojan" },
                Tips = new()
                {
                    "Keep your operating system and apps updated. Updates patch the holes malware uses.",
                    "Only install apps from official stores, and don't open attachments you weren't expecting.",
                    "Use reputable antivirus software and run regular scans.",
                    "Back up important files regularly. It's your best defence against ransomware."
                },
                Details = new()
                {
                    "Malware is software designed to harm or spy on you. Types include viruses, trojans, spyware and ransomware, which locks your files until you pay.",
                    "Keep at least one backup offline or in the cloud, disconnected from your main device.",
                    "If you suspect an infection, disconnect from the internet, run a full scan and change your passwords from a clean device."
                },
                Personal = "set your devices to update automatically so you're always protected."
            },
            new()
            {
                Name = "two-factor authentication",
                Keywords = new[] { "2fa", "two-factor", "two factor", "mfa", "multi-factor", "otp", "authenticator" },
                Tips = new()
                {
                    "Turn on two-factor authentication (2FA) wherever possible. A stolen password alone then isn't enough.",
                    "Prefer an authenticator app over SMS codes for stronger protection.",
                    "Never share your one-time PIN (OTP) with anyone, even if they say they're from your bank."
                },
                Details = new()
                {
                    "2FA asks for something you know (password) plus something you have (your phone or an app). Attackers need both.",
                    "Start with your email, banking and social media accounts. They're the most valuable to attackers.",
                    "Save your backup codes somewhere safe, so you can still get in if you lose your phone."
                },
                Personal = "enable 2FA on your email first, since it can reset all your other accounts."
            },
            new()
            {
                Name = "cybersecurity",
                Keywords = new[] { "cybersecurity", "cyber security", "online safety", "stay safe", "security" },
                Tips = new()
                {
                    "Keep your software updated to protect against security threats.",
                    "Use strong, unique passwords and turn on two-factor authentication.",
                    "Think before you click. Most attacks start with a message that tricks you.",
                    "Back up your important data regularly."
                },
                Details = new()
                {
                    "Cybersecurity is about protecting your devices, accounts and personal data from attackers. Good habits matter more than fancy tools.",
                    "The basics: update everything, use unique passwords, enable 2FA, and stay sceptical of unexpected messages.",
                    "South Africa has seen a rise in cyberattacks on individuals and businesses, so awareness is the first line of defence."
                },
                Personal = "build one new safe habit each week. Small steps add up."
            }
        };

        return topics.ToDictionary(t => t.Name, t => t);
    }
}
