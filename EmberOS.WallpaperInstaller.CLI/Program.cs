using System.Net;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

    private const int SPI_SETDESKWALLPAPER = 0x0014;
    private const int SPIF_UPDATEINIFILE = 0x01;
    private const int SPIF_SENDWININICHANGE = 0x02;

    static async Task Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: SetWallpaper <protocol_url>");
            return;
        }

        string protocolUrl = args[0];
        string imageUrl = ExtractImageUrl(protocolUrl);

        Console.WriteLine($"{imageUrl}");

        if (string.IsNullOrEmpty(imageUrl))
        {
            Console.WriteLine("Invalid URL format.");
            Console.ReadLine();
            return;
        }

        string imagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "wallpaper.jpg");

        try
        {
            using (WebClient client = new WebClient())
            {
                await client.DownloadFileTaskAsync(new Uri(imageUrl), imagePath);
            }

            SetWallpaper(imagePath);

            Console.WriteLine("Wallpaper set successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.ReadLine();
        }
    }

    private static string ExtractImageUrl(string protocolUrl)
    {
        string newProtocolUrl = string.Empty;
        if (protocolUrl.StartsWith("downloadandinstallwallpaperonthedesktop://"))
        {
            newProtocolUrl = Uri.UnescapeDataString(protocolUrl.Substring("downloadandinstallwallpaperonthedesktop://".Length));
        }
        if (newProtocolUrl.EndsWith("/"))
        {
            newProtocolUrl = newProtocolUrl.Substring(0, newProtocolUrl.Length - 1);
        }
        Console.WriteLine($"{newProtocolUrl}");
        return newProtocolUrl;
    }

    private static void SetWallpaper(string imagePath)
    {
        SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
    }
}
