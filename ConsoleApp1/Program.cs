using ConsoleApp1;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

var fontPath = $@"{Environment.CurrentDirectory}\Resources\Minecraftia-Regular v2.ttf";
var textPath = $@"{Environment.CurrentDirectory}\Resources\test.txt";
var canvasWidth = 2048;
var canvasMaxHeight = 10000;
var maxLettersPerImage = 10000;
var downscaleFactor = 4;
float fontSize = 32f;

Console.WriteLine("start");

var backgroundColor = Rgba32.ParseHex("#00000000");
var textColor = Rgba32.ParseHex("#000000");

var drawingOptions = new DrawingOptions {
    GraphicsOptions = new GraphicsOptions {
        Antialias = false,
    },
};
var brush = new SolidBrush(textColor);

var text = System.IO.File.ReadAllText(textPath);
var textSegments = RichTextParser.Parse(text, fontSize, TextAlignment.Start);
var wordsBySegmentId = textSegments.SelectMany(segment => 
    segment.Text.Split(" ")
        .Select(word => ($" {word}", Array.IndexOf(textSegments, segment))))
    .ToArray();

var fontsCollection = textSegments
    .GroupBy(segment => segment.size)
    .ToDictionary(group => group.Key, group => {;
        var collection = new FontCollection().Add(fontPath);
        return collection.CreateFont(group.Key, FontStyle.Regular);
    });

var textTotalHeight = 0f;
var optionsPerSegment = textSegments.Select(segment => {
    var option = new RichTextOptions(fontsCollection[segment.size]) {
        LineSpacing = 1.5f,
        Origin = new PointF(new SizeF(height: textTotalHeight,
            width: segment.textAlignment == TextAlignment.Center ? canvasWidth / 2f : 0)), // это baseline
        VerticalAlignment = VerticalAlignment.Top,
        HorizontalAlignment = (HorizontalAlignment)segment.textAlignment,
        WordBreaking = WordBreaking.BreakWord,
        WrappingLength = canvasWidth,
    };
    
    var textSize = TextMeasurer.MeasureSize(segment.Text, option);
    textTotalHeight += (int)textSize.Height;

    return option;
}).ToArray();

var builder = new System.Text.StringBuilder();
var breakPoints = new List<int>();
for (var i = 0; i < wordsBySegmentId.Length; i++) {
    var (word, segmentId) = wordsBySegmentId[i];
    builder.Append(word);
    var linesCount = TextMeasurer.CountLines(builder.ToString(), optionsPerSegment[segmentId]);
    if (linesCount > 1) {
        breakPoints.Add(i);
        builder.Clear();
    }
}


var imagesNeededToFitAllText = (int)Math.Ceiling(textTotalHeight / canvasMaxHeight);
var processedTextHeight = 0;
/*
for (int i = 0; i < imagesNeededToFitAllText; i++) {
    var heightLeft = Math.Min(canvasMaxHeight, (int) Math.Ceiling(textTotalHeight - processedTextHeight));
    using var image = new Image<Rgba32>(canvasWidth, heightLeft, backgroundColor);
    image.Mutate(ctx => ctx
        .DrawText(drawingOptions, richTextOptions, richTextSegmentData.Text, brush, null)
    );
    
    var textSize = TextMeasurer.MeasureSize(richTextSegmentData.Text, richTextOptions);
    offset.Height += (int)textSize.Height;
}

var offset = Size.Empty;
foreach (var richTextSegmentData in textSegments) {
    var collection = new FontCollection().Add(fontPath);
    var font = collection.CreateFont(richTextSegmentData.size, FontStyle.Regular);

    RichTextOptions richTextOptions = new(font) {
        LineSpacing = 1.5f,
        Origin = new PointF(new SizeF(height: offset.Height, width: richTextSegmentData.textAlignment == TextAlignment.Center ? canvasSize.x / 2f : 0)), // это baseline
        VerticalAlignment = VerticalAlignment.Top,
        HorizontalAlignment = (HorizontalAlignment) richTextSegmentData.textAlignment,
        WordBreaking = WordBreaking.BreakWord,
        WrappingLength = canvasSize.x,
    };
}

image.Mutate(ctx => 
    ctx.Resize(new Size(canvasSize.x / downscaleFactor, canvasSize.y / downscaleFactor), KnownResamplers.NearestNeighbor, false)
    );
//downscale at halp size
image.SaveAsPng(@$"{Environment.CurrentDirectory}\output.png");

Console.WriteLine("finish");
Console.ReadKey();
*/