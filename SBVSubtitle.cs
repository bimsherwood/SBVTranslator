using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;

namespace SBVTranslator {
    class SBVSubtitle {
        
        private const string TimeSpanFormat = @"hh\:mm\:ss\.fff";
        
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        
        private string _content;
        public string Content {
            get { return _content; }
            set { this._content = Cleanse(value); }
        }
        
        // Parse one Subtitle from an SBV file.
        // Returns null if there is no subtitle to read.
        public static SBVSubtitle ParseSingle(TextReader reader){
                
            // Read the next non-empty line as a timestamp
            string timestampLine;
            do {
                timestampLine = reader.ReadLine();
                if(timestampLine == null) return null;
            } while(string.IsNullOrWhiteSpace(timestampLine));
            
            // Parse the timestamp
            var parts = timestampLine.Split(',');
            if(parts.Length != 2)
                throw new FormatException("Invalid timestamp.");
            TimeSpan start, end;
            if(!ParseTimestamp(parts[0], out start))
                throw new FormatException($"Invalid timestamp '{parts[0]}'.");
            if(!ParseTimestamp(parts[1], out end))
                throw new FormatException($"Invalid timestamp '{parts[1]}'.");
            var subtitle = new SBVSubtitle();
            subtitle.Start = start;
            subtitle.End = end;
            
            // Collect the content
            var contentBuilder = new StringBuilder();
            for(;;) {
                var line = reader.ReadLine();
                if(string.IsNullOrWhiteSpace(line)) break;
                contentBuilder.AppendLine(line);
            }
            subtitle.Content = contentBuilder.ToString();
            
            return subtitle;
        }
        
        public static IEnumerable<SBVSubtitle> Parse(TextReader reader){
            for(;;){
                var subtitle = ParseSingle(reader);
                if(subtitle == null) yield break;
                yield return subtitle;
            }
        }
        
        // Removes empty lines
        private string Cleanse(string str) {
            while(str.Contains("\n\n")) {
                str = str.Replace("\n\n", "\n");
            }
            while(str.Contains("\r\n\r\n")) {
                str = str.Replace("\r\n\r\n", "\r\n");
            }
            while(str.Contains("\r\r")) {
                str = str.Replace("\r\r", "\r");
            }
            return str;
        }
        
        // Parse timestamp
        private static bool ParseTimestamp(string input, out TimeSpan output){
            return TimeSpan.TryParseExact(
                input,
                TimeSpanFormat,
                CultureInfo.InvariantCulture,
                TimeSpanStyles.AssumeNegative,
                out output);
        }
        
        public override string ToString() {
            var builder = new StringBuilder();
            builder.Append(this.Start.ToString(TimeSpanFormat));
            builder.Append(',');
            builder.AppendLine(this.End.ToString(TimeSpanFormat));
            builder.Append(this.Content);
            return builder.ToString();
        }
    }
}