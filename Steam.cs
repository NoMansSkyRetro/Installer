using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace NMSLegacyVersionInstaller
{
    public static class Steam
    {
        public static string Username { get; set; }
        public static string Password { get; set; }
        public static string ID { get; set; }

        public static void TryGetSteamID()
        {
            string apiUrl = $"https://steamid.co/php/api.php?action=steamID&id={Username}";

            try
            {
                using (var client = new WebClient())
                {
                    string jsonResponse = client.DownloadString(apiUrl);

                    // Find the start and end index of the steamID64 value
                    int startIndex = jsonResponse.IndexOf("\"steamID64\":\"");
                    if (startIndex != -1)
                    {
                        startIndex += 13; // Move to the beginning of the actual value
                        int endIndex = jsonResponse.IndexOf("\"", startIndex);

                        if (endIndex != -1)
                        {
                            ID = jsonResponse.Substring(startIndex, endIndex - startIndex);
                            return;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                // Handle exception (e.g., network error)
                Console.WriteLine($"API call failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            // Return an empty string if steamID64 key is not found or an error occurred
            ID = string.Empty;
        }

    }
}
