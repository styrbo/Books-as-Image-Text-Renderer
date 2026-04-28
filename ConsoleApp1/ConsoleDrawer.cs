namespace ConsoleApp1;

public static class ConsoleDrawer {
    public static int DrawError(string errorMessage) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"error: {errorMessage}");
        Console.ReadKey();
        return -1;
    }

    public static void DrawText(string text) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(text);
        Console.ResetColor();
    }
    
    public static bool AskPermissionToContinue(string message) {
        while (true) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{message} [Y/N]");
            Console.ResetColor();
            var key = Console.ReadKey();

            switch (key.Key) {
                case ConsoleKey.Y:
                    return true;
                case ConsoleKey.N:
                    return false;
            }
        }
    }
}
