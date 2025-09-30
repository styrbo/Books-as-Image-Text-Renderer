using SixLabors.Fonts;

namespace ConsoleApp1;

public static class RichTextParser {

    public static RichTextSegmentData[] Parse(string text, float defaultSize, TextAlignment defaultAlignment) {
        var firstTagIndex = text.IndexOf('<');
        if(firstTagIndex == -1) {
            return [new RichTextSegmentData(text, defaultSize, defaultAlignment)];
        }
        
        var segments = new List<RichTextSegmentData>();

        var i = 0;
        var lastTagIndex = 0;
        while (i < text.Length) {
            var currentChar = text[i];
            if (currentChar == '<') {
                var closingTagIndex = text.IndexOf('>', i);
                if (closingTagIndex == -1) {
                    // No closing tag found, treat the rest as normal text
                    i++;
                } else {
                    var tagContent = text.Substring(i + 1, closingTagIndex - i - 1);
                    var tagParts = tagContent.Split('=');
                    var tagName = tagParts[0].ToLower();
                    var tagValue = tagParts.Length > 1 ? tagParts[1] : null;
                    var endSegmentIndex = text.IndexOf($"</{tagName}", closingTagIndex + 1, StringComparison.Ordinal);
                    if (endSegmentIndex == -1) {
                        endSegmentIndex = text.Length;
                    }
                    var segmentText = text.Substring(closingTagIndex + 1, endSegmentIndex - closingTagIndex - 1);
                    var innerData = new RichTextSegmentData(segmentText, defaultSize, defaultAlignment);
                    var taggedData = TagCollection(innerData, tagName, tagValue);
                    
                    if(lastTagIndex > 0)
                        segments.Add(new RichTextSegmentData(text.Substring(lastTagIndex, i - lastTagIndex), defaultSize, defaultAlignment));
                    
                    segments.Add(taggedData);
                    
                    i = endSegmentIndex + tagName.Length + 3;
                    lastTagIndex = i;
                }
            } else {
                i++;
            }
        }
        
        if(lastTagIndex < text.Length) {
            segments.Add(new RichTextSegmentData(text.Substring(lastTagIndex), defaultSize, defaultAlignment));
        }

        return segments.ToArray();
}


    private static RichTextSegmentData TagCollection(RichTextSegmentData innerData, string tagName, string? tagValue) {
        switch (tagName) {
            case "size":
                if (float.TryParse(tagValue, out var newSize)) {
                    return innerData with { size = newSize };
                }
                return innerData;
            case "align":

                TextAlignment? newAlignment = null;
                switch (tagValue) {
                    case "left":
                        newAlignment = TextAlignment.Start;
                        break;
                    case "center":
                        newAlignment = TextAlignment.Center;
                        break;
                    case "right":
                        newAlignment = TextAlignment.End;
                        break;
                    default:
                        newAlignment = null;
                        break; 
                }
                
                if (newAlignment != null) {
                    return innerData with { textAlignment = newAlignment.Value };
                }
                return innerData;
            default:
                return innerData;
        }
        
    }
}

public readonly record struct RichTextSegmentData(string Text, float size, TextAlignment textAlignment);
