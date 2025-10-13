namespace ConsoleApp1;

[Serializable]
public class Config {
    public int canvasWidth { get; set; } = 1;
    public int canvasHeight { get; set; } = 2;
    public int maxLettersPerImage { get; set; } = 3;
    public int downscaleFactor { get; set; } = 4;
    public float fontSize { get; set; } = 5;
    public float lineSpacing { get; set; } = 6;
    public string hexBackgroundColor { get; set; } = "1";
    public string hexTextColor { get; set; } = "2";
    public string fontPath { get; set; } = "3";
}
