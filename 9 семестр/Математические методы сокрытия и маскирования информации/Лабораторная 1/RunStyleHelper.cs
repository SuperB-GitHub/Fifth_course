using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Linq;

namespace Лабораторная_1
{
    internal static class RunStyleHelper
    {
        public sealed class RefStyle
        {
            public string Color = "000000";
            public string Background = "Прозрачный";
            public double FontSize = 12.0;
            public int Scale = 100;
            public int Spacing = 0;

            public string BackgroundFill = "FFFFFF";
        }

        public static RefStyle BuildRefStyle(Run lastRun)
        {
            var props = lastRun.RunProperties!;
            string bg = GetRunBackground(props);
            return new RefStyle
            {
                Color = GetRunColor(props, lastRun),
                Background = bg,
                BackgroundFill = NormalizeFill(bg),
                FontSize = GetEffectiveFontSize(lastRun),
                Scale = GetRunScaleValue(props, lastRun),
                Spacing = GetRunSpacingValue(props, lastRun)
            };
        }

        public static string NormalizeFill(string bg)
        {
            if (string.IsNullOrEmpty(bg) ||
                bg.Equals("Прозрачный", StringComparison.OrdinalIgnoreCase))
                return "FFFFFF";

            const string fillPrefix = "Заливка: #";
            if (bg.StartsWith(fillPrefix, StringComparison.OrdinalIgnoreCase))
                return bg.Substring(fillPrefix.Length);

            return "FFFFFF";
        }

        public static string GetRunColor(RunProperties? props, Run run)
        {
            var colorElement = props?.Color ?? GetStyleProperty<Color>(run);
            return colorElement?.Val?.Value ?? "Авто (черный по умолчанию)";
        }

        public static string GetRunBackground(RunProperties? props)
        {
            var shading = props?.Shading;
            if (shading?.Fill != null && !string.IsNullOrEmpty(shading.Fill.Value))
            {
                string fill = shading.Fill.Value;
                if (!fill.Equals("auto", StringComparison.OrdinalIgnoreCase) &&
                    !fill.Equals("FFFFFF", StringComparison.OrdinalIgnoreCase))
                {
                    return $"Заливка: #{fill}";
                }
            }

            var highlight = props?.Highlight;
            if (highlight?.Val != null)
            {
                var val = highlight.Val.Value;
                if (val != HighlightColorValues.White && val != HighlightColorValues.None)
                    return $"Маркер: {val}";
            }

            return "Прозрачный";
        }

        public static int GetRunScaleValue(RunProperties? props, Run run)
        {
            var el = props?.CharacterScale ?? GetStyleProperty<CharacterScale>(run);
            return (int)(el?.Val?.Value ?? 100);
        }

        public static int GetRunSpacingValue(RunProperties? props, Run run)
        {
            var el = props?.Spacing ?? GetStyleProperty<Spacing>(run);
            return el?.Val?.Value ?? 0;
        }

        public static double GetEffectiveFontSize(Run run)
        {
            double direct = 12.0;
            var directEl = run.RunProperties?.FontSize;
            if (directEl?.Val?.Value != null &&
                double.TryParse(directEl.Val.Value, out double h1))
            {
                direct = h1 / 2.0;
            }

            double fromStyle = double.MaxValue;
            var styleEl = GetStyleProperty<FontSize>(run);
            if (styleEl?.Val?.Value != null &&
                double.TryParse(styleEl.Val.Value, out double h2))
            {
                fromStyle = h2 / 2.0;
            }

            return Math.Min(direct, fromStyle);
        }

        public static T GetStyleProperty<T>(Run run) where T : OpenXmlElement
        {
            var styleId = run.RunProperties?.RunStyle?.Val?.Value;
            if (string.IsNullOrEmpty(styleId)) return null!;

            var mainPart = run.Ancestors<Document>().FirstOrDefault()?.MainDocumentPart;
            var stylePart = mainPart?.StyleDefinitionsPart;
            if (stylePart?.Styles == null) return null!;

            var style = stylePart.Styles.Elements<Style>()
                .FirstOrDefault(s => s.StyleId == styleId);

            if (style == null) return null!;

            var styleProps = style.StyleRunProperties;
            if (styleProps == null) return null!;

            return styleProps.Elements<T>().FirstOrDefault()!;
        }
    }
}