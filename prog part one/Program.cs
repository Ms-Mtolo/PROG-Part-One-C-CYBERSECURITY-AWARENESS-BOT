using System;
using System.Media;
using System.IO;
using System.Threading;

namespace CybersecurityAwarenessBot
{
    class Program
    {
        // Store user's name for personalization
        static string userName = "";

        // Track conversation state
        static bool inDeepConversation = false;
        static string currentTopic = "";

        // Flag to track if program should exit
        static bool shouldExit = false;

        static void Main(string[] args)
        {
            Console.Title = "Cybersecurity Awareness Bot";
            Console.ForegroundColor = ConsoleColor.Cyan;

            // Handle Ctrl+C gracefully
            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("\n\nBot: Detected exit request. Goodbye!");
                Environment.Exit(0);
            };

            try
            {
                //  Play voice greeting
                PlayVoiceGreeting();

                // Display ASCII art logo
                DisplayASCIILogo();

                //  Get user name and display welcome
                GetUserName();
                DisplayWelcomeMessage();

                // Start conversational chat
                StartConversationalChat();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        
        /// Voice Greeting Integration
       
        static void PlayVoiceGreeting()
        {
            try
            {
                string audioFilePath = "Greetings.wav";

                if (File.Exists(audioFilePath))
                {
                    SoundPlayer player = new SoundPlayer(audioFilePath);
                    player.Play();
                    Thread.Sleep(500);
                    Console.WriteLine("[Voice Greeting] Playing welcome message...\n");
                }
                else
                {
                    Console.WriteLine("[Voice Greeting] Hello! Welcome to the Cybersecurity Awareness Bot.");
                    Console.WriteLine("I'm here to help you stay safe online.\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Note: Voice greeting unavailable ({ex.Message})\n");
            }
        }

        
        ///  ASCII Art Display
       
        static void DisplayASCIILogo()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"
    ╔══════════════════════════════════════════════════════════════╗
    ║                    CYBERSECURITY AWARENESS BOT               ║
    ╠══════════════════════════════════════════════════════════════╣
    ║                                                              ║
    ║     ██████╗ ███████╗ ██████╗██╗   ██╗██████╗ ███████╗       ║
    ║    ██╔════╝ ██╔════╝██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝       ║
    ║    ██║  ███╗█████╗  ██║      ╚████╔╝ ██████╔╝███████╗       ║
    ║    ██║   ██║██╔══╝  ██║       ╚██╔╝  ██╔══██╗╚════██║       ║
    ║    ╚██████╔╝███████╗╚██████╗   ██║   ██║  ██║███████║       ║
    ║     ╚═════╝ ╚══════╝ ╚═════╝   ╚═╝   ╚═╝  ╚═╝╚══════╝       ║
    ║                                                              ║
    ║                   STAY SAFE ONLINE!                          ║
    ║                                                              ║
    ╚══════════════════════════════════════════════════════════════╝
            ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
        }

       
        ///  Get user name with validation
        
        static void GetUserName()
        {
            Console.Write("Please enter your name: ");
            string? input = Console.ReadLine();
            userName = string.IsNullOrWhiteSpace(input) ? "" : input ?? "";

            while (string.IsNullOrWhiteSpace(userName))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("I didn't catch that. Please enter a valid name: ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Your name: ");
                input = Console.ReadLine();
                userName = string.IsNullOrWhiteSpace(input) ? "" : input ?? "";
            }

            userName = char.ToUpper(userName[0]) + userName.Substring(1).ToLower();
        }

       
        /// Display personalized welcome
        
        static void DisplayWelcomeMessage()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n╔" + new string('═', 60) + "╗");
            Console.WriteLine($"║  Welcome, {userName}! Let's learn about cybersecurity together.  ║");
            Console.WriteLine("║  I'm your personal security assistant!                         ║");
            Console.WriteLine("╚" + new string('═', 60) + "╝");
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine($"\nHello {userName}! I'm here to help you stay safe online.");
        }

       
        /// Main conversational chat loop - asks how user is doing first
        
        static void StartConversationalChat()
        {
            // FIRST: Ask how the user is doing
            AskHowAreYouDoing();

            // Main conversation loop
            while (!shouldExit)
            {
                if (!inDeepConversation)
                {
                    // Ask what they want to learn about
                    AskWhatUserWantsToLearn();
                }

                string? userInput = Console.ReadLine();

                // Input validation
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Bot: I didn't quite understand that. Could you rephrase?");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    continue;
                }

                string cleanedInput = userInput.Trim().ToLower();

                // Check for exit
                if (cleanedInput == "exit" || cleanedInput == "quit" || cleanedInput == "bye")
                {
                    // Ask for satisfaction before exiting
                    if (CheckUserSatisfaction())
                    {
                        shouldExit = true;
                        break;
                    }
                    else
                    {
                        continue; // User wants to continue learning
                    }
                }

                // Handle conversation based on state
                if (inDeepConversation)
                {
                    HandleDeepConversation(cleanedInput);
                }
                else
                {
                    HandleTopicSelection(cleanedInput);
                }
            }

            SayGoodbye();
        }

       
        /// Ask user if they are satisfied with the information
       
        static bool CheckUserSatisfaction()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            
            Console.WriteLine("                 SATISFACTION CHECK                  "        );
            
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine($"\n{userName}, before you go, I'd like to ask:");
            Console.WriteLine("Are you satisfied with the cybersecurity information I provided?");
            Console.WriteLine("\n[1] Yes, I'm very satisfied! ✅");
            Console.WriteLine("[2] No, I'd like to learn more about another topic ");
            Console.Write("\nYour choice (1-2): ");

            string? choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n Wonderful! I'm so glad I could help you, {userName}!");
                Console.WriteLine("Remember to practice these cybersecurity tips every day.");
                Console.WriteLine("Stay safe and secure online!");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Thread.Sleep(1500);
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n Great, {userName}! Let's explore something new!");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Thread.Sleep(1000);
                inDeepConversation = false;
                return false;
            }
        }

        
        /// Ask user how they are doing - conversational opening
      
        static void AskHowAreYouDoing()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nBefore we begin, {userName}, I'd like to check in with you.");
            Console.WriteLine("How are you doing today?");
            Console.Write("\nYour response: ");

            string? feelingResponse = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(feelingResponse))
            {
                Console.WriteLine("\nBot: That's okay! I'm here whenever you're ready to talk.");
                return;
            }

            string feeling = feelingResponse.Trim().ToLower();

            // Respond empathetically based on their response
            if (feeling.Contains("great") || feeling.Contains("good") || feeling.Contains("wonderful") ||
                feeling.Contains("excellent") || feeling.Contains("awesome") || feeling.Contains("happy"))
            {
                Console.WriteLine($"\nBot: That's fantastic to hear, {userName}! A positive mindset is perfect for learning.");
                Console.WriteLine("I'm excited to share some valuable cybersecurity knowledge with you today.");
            }
            else if (feeling.Contains("okay") || feeling.Contains("fine") || feeling.Contains("alright") ||
                     feeling.Contains("not bad") || feeling.Contains("so so"))
            {
                Console.WriteLine($"\nBot: I'm glad you're doing okay, {userName}. Learning something new might make your day even better!");
                Console.WriteLine("Let me share some tips that can help you feel more secure online.");
            }
            else if (feeling.Contains("bad") || feeling.Contains("terrible") || feeling.Contains("awful") ||
                     feeling.Contains("rough") || feeling.Contains("difficult") || feeling.Contains("hard"))
            {
                Console.WriteLine($"\nBot: I'm sorry to hear that you're having a rough day, {userName}.");
                Console.WriteLine("Sometimes learning something new can help take your mind off things.");
                Console.WriteLine("I'll keep our conversation light and positive. Let me know if there's anything specific you need.");
            }
            else if (feeling.Contains("tired") || feeling.Contains("exhausted") || feeling.Contains("sleepy"))
            {
                Console.WriteLine($"\nBot: I understand, {userName}. Cybersecurity can be tiring to think about.");
                Console.WriteLine("I'll keep this brief and focused on the most important tips for you.");
            }
            else if (feeling.Contains("anxious") || feeling.Contains("nervous") || feeling.Contains("worried"))
            {
                Console.WriteLine($"\nBot: It's completely normal to feel anxious about online security, {userName}.");
                Console.WriteLine("The good news is that knowledge is power. Let me help you feel more in control.");
                Console.WriteLine("We'll start with simple, easy-to-remember tips.");
            }
            else if (feeling.Contains("curious") || feeling.Contains("interested") || feeling.Contains("eager"))
            {
                Console.WriteLine($"\nBot: Curiosity is the best trait for learning cybersecurity, {userName}!");
                Console.WriteLine("I love that you're eager to protect yourself online.");
                Console.WriteLine("Let me share some fascinating insights with you.");
            }
            else
            {
                Console.WriteLine($"\nBot: Thank you for sharing that with me, {userName}.");
                Console.WriteLine("Whatever you're feeling today, I'm here to help you learn about staying safe online.");
                Console.WriteLine("Let's begin with something useful.");
            }

            Thread.Sleep(1500);
            Console.WriteLine("\nBot: Remember, taking care of your digital well-being is just as important as your physical health.");
            Thread.Sleep(1000);
        }

       
        /// Ask what user wants to learn about
        
        static void AskWhatUserWantsToLearn()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n{userName}, what would you like to learn about today?");
            Console.WriteLine("\nHere are some topics I can teach you:");
            Console.WriteLine("  ┌─────────────────────────────────────────────────────────┐");
            Console.WriteLine("  │ 1. Password Safety                                      │");
            Console.WriteLine("  │ 2. Phishing Attacks                                    │");
            Console.WriteLine("  │ 3. Safe Browsing Habits                                │");
            Console.WriteLine("  │ 4. Two-Factor Authentication (2FA)                     │");
            Console.WriteLine("  │ 5. General Cybersecurity Tips                          │");
            Console.WriteLine("  └─────────────────────────────────────────────────────────┘");
            Console.Write("\nPlease enter a number (1-5): ");
        }

     
        /// Handle topic selection from menu
    
        static void HandleTopicSelection(string input)
        {
            // Check for menu number selections
            if (input == "1" || input.Contains("password") || input.Contains("passphrase") || input.Contains("password safety") || input.Contains("Safety"))
            {
                currentTopic = "password";
                inDeepConversation = true;
                ShowPasswordDefinitionAndOptions();
            }
            else if (input == "2" || input.Contains("phish") || input.Contains("scam") || input.Contains("fake email") || input.Contains("attacks"))
                
            {
                currentTopic = "phishing";
                inDeepConversation = true;
                ShowPhishingDefinitionAndOptions();
            }
            else if (input == "3" || input.Contains("brows") || input.Contains("internet") || input.Contains("website") || input.Contains("searching habits"))
            {
                currentTopic = "browsing";
                inDeepConversation = true;
                ShowBrowsingDefinitionAndOptions();
            }
            else if (input == "4" || input.Contains("2fa") || input.Contains("two factor") || input.Contains("multi factor") || input.Contains("two-step verification"))
            {
                currentTopic = "2fa";
                inDeepConversation = true;
                ShowTwoFactorDefinitionAndOptions();
            }
            else if (input == "5" || input.Contains("general") || input.Contains("tip") || input.Contains("advice"))
            {
                currentTopic = "general";
                inDeepConversation = true;
                ShowGeneralDefinitionAndOptions();
            }
            else if (input.Contains("hello") || input.Contains("hi") || input.Contains("hey") || input.Contains("Good day") || input.Contains("Good evening") || input.Contains("Good Morning"))
            {
                Console.WriteLine($"Bot: Hello {userName}! How are you doing today?");
            }
            else if (input.Contains("thank") || input.Contains("thanks"))
            {
                Console.WriteLine($"Bot: You're welcome, {userName}! Would you like to learn about any cybersecurity topic?");
            }
            else
            {
                Console.WriteLine($"Bot: I didn't quite understand that. Please enter a number (1-5) to choose a topic.");
            }
        }

        
        /// Information about password and examples
       
        static void ShowPasswordDefinitionAndOptions()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    PASSWORD SAFETY                             ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");

            Console.WriteLine("\n📖 DEFINITION:");
            Console.WriteLine("Password safety is the practice of creating and managing strong,");
            Console.WriteLine("unique passwords to protect your online accounts from unauthorized access.");

            Console.WriteLine("\n" + new string('─', 60));
            Console.WriteLine("\nWhat would you like to learn about passwords?");
            Console.WriteLine("\n[1] Tips for creating strong passwords");
            Console.WriteLine("[2] Examples of weak vs strong passwords");
            Console.WriteLine("[3] Why password reuse is dangerous");
            Console.WriteLine("[4] How password managers can help");
            Console.WriteLine("[5] I understand, let me learn about another topic");
            Console.Write("\nYour choice (1-5): ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowPasswordTips();
                    break;
                case "2":
                    ShowPasswordExamples();
                    break;
                case "3":
                    ShowPasswordReuseDanger();
                    break;
                case "4":
                    ShowPasswordManagerHelp();
                    break;
                case "5":
                    inDeepConversation = false;
                    break;
                default:
                    Console.WriteLine("\nBot: Let me share some tips with you.");
                    ShowPasswordTips();
                    break;
            }
        }

        static void ShowPasswordTips()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n🔐 TIPS FOR STRONG PASSWORDS:");
            Console.WriteLine("  • Use at least 12-16 characters");
            Console.WriteLine("  • Combine uppercase and lowercase letters");
            Console.WriteLine("  • Include numbers and special symbols (!@#$%)");
            Console.WriteLine("  • Avoid personal info (birthdays, pet names)");
            Console.WriteLine("  • Never reuse passwords across different sites");
            Console.WriteLine("  • Enable Two-Factor Authentication when possible");

            AfterInfoPrompt();
        }

        static void ShowPasswordExamples()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n📝 EXAMPLES:");
            Console.WriteLine("\n❌ WEAK PASSWORDS (Avoid these!):");
            Console.WriteLine("  • 'password123' - Can be cracked INSTANTLY");
            Console.WriteLine("  • 'qwerty' - One of the most common passwords");
            Console.WriteLine("  • 'John1985' - Uses personal information");

            Console.WriteLine("\n✅ STRONG PASSWORDS (Use patterns like these):");
            Console.WriteLine("  • 'Purple$Coffee%Mountain#42'");
            Console.WriteLine("  • 'Sunset@Beach!2024'");
            Console.WriteLine("  • 'Correct-Horse-Battery-Staple' (XKCD method)");

            AfterInfoPrompt();
        }

        static void ShowPasswordReuseDanger()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n⚠️ WHY PASSWORD REUSE IS DANGEROUS:");
            Console.WriteLine("\nReal scenario:");
            Console.WriteLine("  1. You use 'MyPassword123' for email, banking, and social media");
            Console.WriteLine("  2. A small website you use gets hacked");
            Console.WriteLine("  3. Hackers get 'MyPassword123' from that site");
            Console.WriteLine("  4. They try that password on your email, banking, and social media");
            Console.WriteLine("  5. They gain access to EVERYTHING!");
            Console.WriteLine("\n💡 Solution: Use UNIQUE passwords for EVERY account!");

            AfterInfoPrompt();
        }

        static void ShowPasswordManagerHelp()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n🔑 HOW PASSWORD MANAGERS HELP:");
            Console.WriteLine("\nWithout password manager:");
            Console.WriteLine("  • You can only remember 5-10 passwords");
            Console.WriteLine("  • You reuse passwords (dangerous!)");
            Console.WriteLine("  • You use simple passwords");

            Console.WriteLine("\nWith password manager:");
            Console.WriteLine("  • You only remember ONE master password");
            Console.WriteLine("  • The manager creates 100+ UNIQUE, RANDOM passwords");
            Console.WriteLine("  • Example: 'xK9#mP2$vL5&qR7@wN3'");
            Console.WriteLine("  • You never need to memorize complex passwords!");
            Console.WriteLine("\n💡 Popular password managers: LastPass, Bitwarden, 1Password");

            AfterInfoPrompt();
        }


        /// Information about phishing and examples

        static void ShowPhishingDefinitionAndOptions()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    PHISHING ATTACKS                            ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");

            Console.WriteLine("\n📖 DEFINITION:");
            Console.WriteLine("Phishing is when attackers trick you into revealing sensitive");
            Console.WriteLine("information (passwords, credit cards) by pretending to be");
            Console.WriteLine("a legitimate company or person through fake emails, texts, or calls.");

            Console.WriteLine("\n" + new string('─', 60));
            Console.WriteLine("\nWhat would you like to learn about phishing?");
            Console.WriteLine("\n[1] How to spot phishing attempts (red flags)");
            Console.WriteLine("[2] Examples of phishing emails");
            Console.WriteLine("[3] Smishing (fake text messages) examples");
            Console.WriteLine("[4] Vishing (fake phone calls) examples");
            Console.WriteLine("[5] I understand, let me learn about another topic");
            Console.Write("\nYour choice (1-5): ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowPhishingRedFlags();
                    break;
                case "2":
                    ShowPhishingEmailExamples();
                    break;
                case "3":
                    ShowSmishingExamples();
                    break;
                case "4":
                    ShowVishingExamples();
                    break;
                case "5":
                    inDeepConversation = false;
                    break;
                default:
                    Console.WriteLine("\nBot: Let me show you how to spot phishing attempts.");
                    ShowPhishingRedFlags();
                    break;
            }
        }

        static void ShowPhishingRedFlags()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n HOW TO SPOT PHISHING (RED FLAGS):");
            Console.WriteLine("  • Urgent or threatening language ('Act now!' 'Your account will close!')");
            Console.WriteLine("  • Generic greetings ('Dear Customer' instead of your name)");
            Console.WriteLine("  • Poor grammar and spelling errors");
            Console.WriteLine("  • Suspicious sender email addresses (amaz0n.com instead of amazon.com)");
            Console.WriteLine("  • Links that don't match the company's real website");
            Console.WriteLine("  • Requests for personal information or passwords");

            AfterInfoPrompt();
        }

        static void ShowPhishingEmailExamples()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n EXAMPLE: Fake PayPal Email");
            Console.WriteLine("\nFrom: security@paypa1.com (NOTICE: 'paypa1' has number 1, not letter l)");
            Console.WriteLine("Subject: URGENT: Your account will be suspended!");
            Console.WriteLine("\nDear Customer,");
            Console.WriteLine("We noticed suspicious activity on your account.");
            Console.WriteLine("Click here to verify your account immediately:");
            Console.WriteLine("http://fake-paypal.xyz/verify");
            Console.WriteLine("Failure to do so within 24 hours will result in permanent closure.");

            Console.WriteLine("\n RED FLAGS IN THIS EMAIL:");
            Console.WriteLine("  • Wrong domain (paypa1.com vs paypal.com)");
            Console.WriteLine("  • Urgent threatening language");
            Console.WriteLine("  • Suspicious link (.xyz domain)");
            Console.WriteLine("  • Generic greeting ('Dear Customer')");

            AfterInfoPrompt();
        }

        static void ShowSmishingExamples()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n📱 EXAMPLE: Fake Bank Text Message (Smishing)");
            Console.WriteLine("\nText message received:");
            Console.WriteLine("'Your Bank of America account has been locked.'");
            Console.WriteLine("'Visit https://verify-bank.com to restore access immediately.'");

            Console.WriteLine("\n🔴 RED FLAGS:");
            Console.WriteLine("  • Generic bank name (not your actual bank)");
            Console.WriteLine("  • Suspicious URL (not bankofamerica.com)");
            Console.WriteLine("  • Creates urgency/pressure");

            Console.WriteLine("\n✅ WHAT TO DO:");
            Console.WriteLine("  • NEVER click the link");
            Console.WriteLine("  • Call your bank directly using their official number");
            Console.WriteLine("  • Delete the message");

            AfterInfoPrompt();
        }

        static void ShowVishingExamples()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n EXAMPLE: Fake IT Support Call (Vishing)");
            Console.WriteLine("\nCaller: 'This is Microsoft Support. We detected a virus on your computer.'");
            Console.WriteLine("Caller: 'Please install this software to fix the problem.'");

            Console.WriteLine("\n RED FLAGS:");
            Console.WriteLine("  • Microsoft does NOT make unsolicited calls");
            Console.WriteLine("  • They want remote access to your computer");
            Console.WriteLine("  • They create fear about viruses");

            Console.WriteLine("\n WHAT TO DO:");
            Console.WriteLine("  • Hang up immediately");
            Console.WriteLine("  • Legitimate companies don't call you first");
            Console.WriteLine("  • Never give remote access to strangers");

            AfterInfoPrompt();
        }

       // Information about safe browsing and examples
        static void ShowBrowsingDefinitionAndOptions()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    SAFE BROWSING HABITS                        ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");

            Console.WriteLine("\n📖 DEFINITION:");
            Console.WriteLine("Safe browsing means protecting yourself while using the internet");
            Console.WriteLine("by recognizing secure websites, avoiding dangerous links,");
            Console.WriteLine("and being cautious about what you download.");

            Console.WriteLine("\n" + new string('─', 60));
            Console.WriteLine("\nWhat would you like to learn about safe browsing?");
            Console.WriteLine("\n[1] How to identify secure websites (HTTPS vs HTTP)");
            Console.WriteLine("[2] Public Wi-Fi dangers and protection");
            Console.WriteLine("[3] Dangerous website red flags");
            Console.WriteLine("[4] General safe browsing tips");
            Console.WriteLine("[5] I understand, let me learn about another topic");
            Console.Write("\nYour choice (1-5): ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowHTTPSvsHTTP();
                    break;
                case "2":
                    ShowPublicWiFiDangers();
                    break;
                case "3":
                    ShowDangerousWebsiteFlags();
                    break;
                case "4":
                    ShowGeneralBrowsingTips();
                    break;
                case "5":
                    inDeepConversation = false;
                    break;
                default:
                    Console.WriteLine("\nBot: Let me explain how to identify secure websites.");
                    ShowHTTPSvsHTTP();
                    break;
            }
        }

        static void ShowHTTPSvsHTTP()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n HTTPS vs HTTP - What's the Difference?");

            Console.WriteLine("\n SECURE website (HTTPS):");
            Console.WriteLine("  • URL starts with 'https://'");
            Console.WriteLine("  • Shows a padlock icon 🔒 in the address bar");
            Console.WriteLine("  • Your connection is ENCRYPTED");
            Console.WriteLine("  • Hackers CANNOT read your data");
            Console.WriteLine("  • SAFE for passwords and credit cards");

            Console.WriteLine("\n INSECURE website (HTTP):");
            Console.WriteLine("  • URL starts with 'http://' (no 's')");
            Console.WriteLine("  • Shows 'Not Secure' warning");
            Console.WriteLine("  • Your connection is OPEN");
            Console.WriteLine("  • Hackers CAN see everything you type");
            Console.WriteLine("  • NEVER enter passwords or payment info here!");

            AfterInfoPrompt();
        }

        static void ShowPublicWiFiDangers()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n Public Wi-Fi Dangers:");

            Console.WriteLine("\n   WHAT COULD HAPPEN:");
            Console.WriteLine("  • Hackers create fake hotspots with similar names");
            Console.WriteLine("  • You connect thinking it's legitimate");
            Console.WriteLine("  • Hackers capture EVERYTHING you send:");
            Console.WriteLine("    - Emails and messages");
            Console.WriteLine("    - Passwords you type");
            Console.WriteLine("    - Credit card numbers");

            Console.WriteLine("\n  HOW TO PROTECT YOURSELF:");
            Console.WriteLine("  • Ask staff for the EXACT Wi-Fi name");
            Console.WriteLine("  • Use a VPN (Virtual Private Network)");
            Console.WriteLine("  • Avoid banking or shopping on public Wi-Fi");
            Console.WriteLine("  • Use your phone's cellular hotspot instead");

            AfterInfoPrompt();
        }

        static void ShowDangerousWebsiteFlags()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n Dangerous Website Red Flags:");

            Console.WriteLine("\nExample suspicious URL: 'http://amaz0n-discounts.xyz/sale'");

            Console.WriteLine("\n  RED FLAGS TO SPOT:");
            Console.WriteLine("  • 'amaz0n' instead of 'amazon' (zero instead of o)");
            Console.WriteLine("  • Unusual domain (.xyz instead of .com)");
            Console.WriteLine("  • HTTP not HTTPS (no padlock)");
            Console.WriteLine("  • Too-good-to-be-true discounts");

            Console.WriteLine("\n  Legitimate Amazon URL:");
            Console.WriteLine("  • 'https://www.amazon.com'");
            Console.WriteLine("  • HTTPS with padlock ✓");
            Console.WriteLine("  • Correct spelling ✓");
            Console.WriteLine("  • Trusted .com domain ✓");

            AfterInfoPrompt();
        }

        static void ShowGeneralBrowsingTips()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  General Safe Browsing Tips:");
            Console.WriteLine("  • Always check for HTTPS and padlock icon");
            Console.WriteLine("  • Keep your browser and extensions updated");
            Console.WriteLine("  • Don't download files from untrusted sources");
            Console.WriteLine("  • Clear your cache and cookies regularly");
            Console.WriteLine("  • Use ad-blockers and privacy extensions");
            Console.WriteLine("  • Be careful what you download");
            Console.WriteLine("  • Think before clicking on pop-ups");

            AfterInfoPrompt();
        }

      //  Information about two-factor authentication and examples
        static void ShowTwoFactorDefinitionAndOptions()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    TWO-FACTOR AUTHENTICATION (2FA)             ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");

            Console.WriteLine("\n DEFINITION:");
            Console.WriteLine("Two-Factor Authentication (2FA) adds an extra layer of security");
            Console.WriteLine("beyond just a password. It requires something you KNOW (password)");
            Console.WriteLine("PLUS something you HAVE (your phone or security key).");

            Console.WriteLine("\n" + new string('─', 60));
            Console.WriteLine("\nWhat would you like to learn about 2FA?");
            Console.WriteLine("\n[1] How 2FA protects you (real scenario)");
            Console.WriteLine("[2] Different types of 2FA");
            Console.WriteLine("[3] How to set up 2FA on Gmail");
            Console.WriteLine("[4] I understand, let me learn about another topic");
            Console.Write("\nYour choice (1-4): ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowHow2FAProtects();
                    break;
                case "2":
                    Show2FATypes();
                    break;
                case "3":
                    ShowSetup2FA();
                    break;
                case "4":
                    inDeepConversation = false;
                    break;
                default:
                    Console.WriteLine("\nBot: Let me explain how 2FA protects you.");
                    ShowHow2FAProtects();
                    break;
            }
        }

        static void ShowHow2FAProtects()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n HOW 2FA PROTECTS YOU:");

            Console.WriteLine("\n Scenario WITHOUT 2FA:");
            Console.WriteLine("  1. Hacker steals your password 'MyPassword123'");
            Console.WriteLine("  2. Hacker logs into your email");
            Console.WriteLine("  3. Hacker has FULL ACCESS - GAME OVER!");

            Console.WriteLine("\n Scenario WITH 2FA:");
            Console.WriteLine("  1. Hacker steals your password 'MyPassword123'");
            Console.WriteLine("  2. Hacker tries to log in");
            Console.WriteLine("  3. Website asks for verification code");
            Console.WriteLine("  4. Code is sent to YOUR phone only");
            Console.WriteLine("  5. Hacker CANNOT proceed - ACCESS DENIED!");

            Console.WriteLine("\n Result: Your account stays safe even with a stolen password!");

            AfterInfoPrompt();
        }

        static void Show2FATypes()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n TYPES OF 2FA:");

            Console.WriteLine("\n1. SMS Text Message:");
            Console.WriteLine("  • Website sends code to your phone via text");
            Console.WriteLine("  • You type the code to log in");
            Console.WriteLine("  •  Less secure (SIM swapping possible)");

            Console.WriteLine("\n2. Authenticator App:");
            Console.WriteLine("  • Google Authenticator, Authy, Microsoft Authenticator");
            Console.WriteLine("  • App generates 6-digit code every 30 seconds");
            Console.WriteLine("  •  More secure than SMS");

            Console.WriteLine("\n3. Physical Security Key:");
            Console.WriteLine("  • YubiKey or similar USB device");
            Console.WriteLine("  • Insert and tap to verify");
            Console.WriteLine("  • Most secure option");

            Console.WriteLine("\n4. Biometrics:");
            Console.WriteLine("  • Fingerprint scan");
            Console.WriteLine("  • Face ID");
            Console.WriteLine("  • Convenient and secure");

            AfterInfoPrompt();
        }

        static void ShowSetup2FA()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  HOW TO SET UP 2FA ON GMAIL (Example):");
            Console.WriteLine("\nStep-by-step:");
            Console.WriteLine("  1. Go to your Google Account settings");
            Console.WriteLine("  2. Click 'Security' on the left menu");
            Console.WriteLine("  3. Find '2-Step Verification' and click 'Get Started'");
            Console.WriteLine("  4. Enter your password again to confirm");
            Console.WriteLine("  5. Choose method: 'Authenticator app' or 'Text message'");
            Console.WriteLine("  6. Scan QR code with Google Authenticator");
            Console.WriteLine("  7. Enter the 6-digit code from the app");
            Console.WriteLine("  8. Save backup codes (print them or save securely)");
            Console.WriteLine("  9. Done! Your account is now 99.9% more secure!");

            AfterInfoPrompt();
        }

        // Information about general tips and examples
        static void ShowGeneralDefinitionAndOptions()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    GENERAL CYBERSECURITY TIPS                  ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");

            Console.WriteLine("\n DEFINITION:");
            Console.WriteLine("General cybersecurity tips are everyday practices that help");
            Console.WriteLine("protect your digital life from various online threats.");

            Console.WriteLine("\n" + new string('─', 60));
            Console.WriteLine("\nWhat would you like to learn about?");
            Console.WriteLine("\n[1] Software updates - Why they matter");
            Console.WriteLine("[2] The 3-2-1 backup rule");
            Console.WriteLine("[3] Social media privacy tips");
            Console.WriteLine("[4] All general tips summary");
            Console.WriteLine("[5] I understand, let me learn about another topic");
            Console.Write("\nYour choice (1-5): ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowSoftwareUpdatesImportance();
                    break;
                case "2":
                    ShowBackupRule();
                    break;
                case "3":
                    ShowSocialMediaTips();
                    break;
                case "4":
                    ShowAllGeneralTips();
                    break;
                case "5":
                    inDeepConversation = false;
                    break;
                default:
                    Console.WriteLine("\nBot: Let me share why software updates matter.");
                    ShowSoftwareUpdatesImportance();
                    break;
            }
        }

        static void ShowSoftwareUpdatesImportance()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n WHY SOFTWARE UPDATES MATTER:");

            Console.WriteLine("\nReal scenario: WannaCry ransomware attack (2017)");
            Console.WriteLine("  • Infected 200,000+ computers across 150 countries");
            Console.WriteLine("  • Caused billions in damages");
            Console.WriteLine("  • Affected hospitals, banks, and governments");
            Console.WriteLine("  • The FIX was available 2 months BEFORE the attack!");
            Console.WriteLine("  • Computers that didn't update got infected");

            Console.WriteLine("\n Moral: Updates close security holes before hackers exploit them.");
            Console.WriteLine("Always install updates when available!");

            AfterInfoPrompt();
        }

        static void ShowBackupRule()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n THE 3-2-1 BACKUP RULE:");

            Console.WriteLine("\nWhat it means:");
            Console.WriteLine("  • 3 copies of your important data");
            Console.WriteLine("  • 2 different storage types (external drive + cloud)");
            Console.WriteLine("  • 1 copy stored OFF-SITE (different physical location)");

            Console.WriteLine("\nExample implementation:");
            Console.WriteLine("  • Original: On your computer");
            Console.WriteLine("  • Copy 1: External hard drive at home");
            Console.WriteLine("  • Copy 2: Cloud backup (Google Drive, iCloud, Backblaze)");

            Console.WriteLine("\nWhy this matters:");
            Console.WriteLine("  • Ransomware can encrypt your computer files");
            Console.WriteLine("  • Fire/flood/theft can destroy physical backups");
            Console.WriteLine("  • Cloud alone could be hacked or locked");
            Console.WriteLine("  • With 3-2-1, you ALWAYS have a way to recover");

            AfterInfoPrompt();
        }

        static void ShowSocialMediaTips()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n SOCIAL MEDIA PRIVACY TIPS:");

            Console.WriteLine("\n What you post can be used against you:");
            Console.WriteLine("  • Photo of your new puppy → 'What's your pet's name?' (common security question)");
            Console.WriteLine("  • Birthday celebration post → Your birthdate is now public");
            Console.WriteLine("  • 'On vacation in Hawaii' → Your home is empty for thieves");
            Console.WriteLine("  • Work ID badge photo → Someone can copy your company credentials");

            Console.WriteLine("\n Best practices:");
            Console.WriteLine("  • Set profiles to PRIVATE");
            Console.WriteLine("  • Never post travel plans until AFTER returning");
            Console.WriteLine("  • Don't share photos of IDs, tickets, or sensitive documents");
            Console.WriteLine("  • Review privacy settings monthly");
            Console.WriteLine("  • Think before you post - once online, it stays online");

            AfterInfoPrompt();
        }

        static void ShowAllGeneralTips()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n COMPLETE CYBERSECURITY TIPS SUMMARY:");
            Console.WriteLine("  • Keep all software and devices updated");
            Console.WriteLine("  • Use antivirus and firewall protection");
            Console.WriteLine("  • Backup important data regularly (3-2-1 rule)");
            Console.WriteLine("  • Be careful what you share on social media");
            Console.WriteLine("  • Use unique passwords for each account");
            Console.WriteLine("  • Think before you click on links or attachments");
            Console.WriteLine("  • Lock your computer when stepping away");
            Console.WriteLine("  • Use Two-Factor Authentication when available");
            Console.WriteLine("  • Avoid public Wi-Fi for sensitive activities");
            Console.WriteLine("  • Be skeptical of unsolicited messages");

            Console.WriteLine("\n💡 Small consistent actions make you much safer than occasional big efforts.");

            AfterInfoPrompt();
        }

       
        /// Asks what to do after showing information
      
        static void AfterInfoPrompt()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n" + new string('─', 60));
            Console.WriteLine("\nWhat would you like to do now?");
            Console.WriteLine("[1] Learn more about this topic");
            Console.WriteLine("[2] Choose a different topic");
            Console.WriteLine("[3] Exit the program");
            Console.Write("\nYour choice (1-3): ");

            string? choice = Console.ReadLine();

            if (choice == "1")
            {
                // Go back to the options menu for current topic
                switch (currentTopic)
                {
                    case "password":
                        ShowPasswordDefinitionAndOptions();
                        break;
                    case "phishing":
                        ShowPhishingDefinitionAndOptions();
                        break;
                    case "browsing":
                        ShowBrowsingDefinitionAndOptions();
                        break;
                    case "2fa":
                        ShowTwoFactorDefinitionAndOptions();
                        break;
                    case "general":
                        ShowGeneralDefinitionAndOptions();
                        break;
                }
            }
            else if (choice == "2")
            {
                inDeepConversation = false;
                Console.WriteLine("\nBot: Great! Let's explore another topic.\n");
            }
            else if (choice == "3")
            {
                if (CheckUserSatisfaction())
                {
                    shouldExit = true;
                }
            }
            else
            {
                Console.WriteLine("\nBot: Let's continue with another topic.");
                inDeepConversation = false;
            }
        }

        
        /// Handle deep conversation when user wants to continue learning
       
        static void HandleDeepConversation(string input)
        {
            if (input.Contains("yes") || input.Contains("sure") || input.Contains("okay") ||
                input.Contains("ok") || input.Contains("another") || input.Contains("more"))
            {
                inDeepConversation = false;
                Console.WriteLine("\nBot: Great! Let's explore another topic then.");
            }
            else if (input.Contains("no") || input.Contains("not") || input.Contains("done") ||
                     input.Contains("finish") || input.Contains("enough"))
            {
                Console.WriteLine($"\nBot: I understand, {userName}.");
                if (CheckUserSatisfaction())
                {
                    shouldExit = true;
                }
                else
                {
                    inDeepConversation = false;
                }
            }
            else
            {
                Console.WriteLine($"\nBot: Would you like to learn about another cybersecurity topic, {userName}?");
                Console.WriteLine("Just say 'yes' to continue or 'no' if you're done for now.");
            }
        }

       
        /// Say goodbye with personalized message and proper exit
        
        static void SayGoodbye()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n╔" + new string('═', 60) + "╗");
            Console.WriteLine($"║  Goodbye, {userName}! Stay safe and secure online!            ║");
            Console.WriteLine("║  Remember: Cybersecurity is everyone's responsibility!       ║");
            Console.WriteLine("╚" + new string('═', 60) + "╝");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nBot: Thank you for learning with me today. Keep those passwords strong and stay vigilant!");

            Console.WriteLine("\nPress any key to exit the program...");
            Console.ReadKey();

            // Properly exit the program
            Environment.Exit(0);
        }
    }
}