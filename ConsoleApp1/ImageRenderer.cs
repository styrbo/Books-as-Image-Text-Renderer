using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ConsoleApp1;

public class ImageRenderer {

    public ImageRenderer(Config config, string fontPath) {
        _drawingOptions = new DrawingOptions {
            GraphicsOptions = new GraphicsOptions {
                Antialias = false,
            },
        };
        var textColor = Color.ParseHex(config.hexTextColor);
        _brush = new SolidBrush(textColor);
        
        _fontSize = config.fontSize;
        _fontPath = fontPath;
        _maxLettersPerImage = config.maxLettersPerImage;
        
        _backgroundColor = Color.ParseHex(config.hexBackgroundColor);
        _canvasWidth = config.canvasWidth;
        _canvasHeight = config.canvasHeight;
        _downscaleFactor = config.downscaleFactor;
    }
    
    private readonly DrawingOptions _drawingOptions;
    private readonly SolidBrush _brush;
    private readonly float _fontSize;
    private readonly string _fontPath;
    private readonly int _maxLettersPerImage;
    private readonly Color _backgroundColor;
    private readonly int _canvasWidth;
    private readonly int _canvasHeight;
    private readonly int _downscaleFactor;
    
    public void Render(string text, string textName) {
        Directory.CreateDirectory(@$"{Environment.CurrentDirectory}\Resources\Output\{textName}");
        
        var textSegments = RichTextParser.Parse(text, _fontSize, TextAlignment.Start);
        var lettersBySegment = textSegments.SelectMany(segment => {
                return segment.Text.ToCharArray().Select(@char => (@char, Array.IndexOf(textSegments, segment)));
            })
            .ToArray();

        var fontsCollection = textSegments
            .GroupBy(segment => segment.size)
            .ToDictionary(group => group.Key, group => {
                ;
                var collection = new FontCollection().Add(_fontPath);
                return collection.CreateFont(group.Key, FontStyle.Regular);
            });

        var imagesNeededToFitAllText = (int)Math.Ceiling(lettersBySegment.Length / (float)_maxLettersPerImage);
        var renderedLettersCount = 0;
        for (int i = 0; i < imagesNeededToFitAllText; i++) {
            var textToRender = lettersBySegment[renderedLettersCount..Math.Min(renderedLettersCount + _maxLettersPerImage, lettersBySegment.Length)];

            var richTextSegments = textToRender //todo: somewhere here is bug because we losing reach text segments
                .GroupBy(t => t.Item2)
                .Select(g => {
                    return new RichTextSegmentData(new string(g.Select(t => t.Item1).ToArray()),
                        textSegments[g.Key].size,
                        textSegments[g.Key].textAlignment);
                })
                .ToArray();

            var textTotalHeight = 0f;
            var optionsPerSegment = richTextSegments.Select(segment => {
                var option = new RichTextOptions(fontsCollection[segment.size]) {
                    LineSpacing = 1.5f,
                    Origin = new PointF(new SizeF(height: textTotalHeight,
                        width: segment.textAlignment == TextAlignment.Center ? _canvasWidth / 2f : 0)), // this baseline
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = (HorizontalAlignment)segment.textAlignment,
                    WordBreaking = WordBreaking.BreakWord,
                    WrappingLength = _canvasWidth,
                };

                var textSize = TextMeasurer.MeasureSize(segment.Text, option);
                textTotalHeight += (int)textSize.Height;

                return option;
            }).ToArray();

            using var image = new Image<Rgba32>(_canvasWidth, _canvasHeight, _backgroundColor);

            var offset = Size.Empty;
            for (var segmentIndex = 0; segmentIndex < richTextSegments.Length; segmentIndex++) {
                var richTextSegmentData = richTextSegments[segmentIndex];
                var richTextOptions = optionsPerSegment[segmentIndex];
                image.Mutate(ctx => ctx
                    .DrawText(_drawingOptions, richTextOptions, richTextSegmentData.Text, _brush, null)
                );

                var textSize = TextMeasurer.MeasureSize(richTextSegmentData.Text, richTextOptions);
                offset.Height += (int)textSize.Height;
            }

            renderedLettersCount += _maxLettersPerImage;
            
            image.Mutate(ctx => {
                ctx.Resize(
                    _canvasWidth / _downscaleFactor, _canvasHeight / _downscaleFactor,
                    KnownResamplers.NearestNeighbor);
            });
            
            image.SaveAsPng(@$"{Environment.CurrentDirectory}\Resources\Output\{textName}\{i}.png");
        }
    }
}
