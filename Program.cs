using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SBVTranslator {
    class Program {
        
        static async Task Main(string[] args) {
            
            // Read settings
            var options = new Options(args);
            var error = options.Errors();
            if(error != null){
                Console.WriteLine(error);
            }
            
            // Translate subtitles
            using(var inputStream = File.OpenRead(options.InputFile))
            using(var inputReader = new StreamReader(inputStream))
            using(var outputStream = options.OutputFile != null ? File.OpenWrite(options.OutputFile) : null)
            using(var outputWriter = outputStream != null ? new StreamWriter(outputStream) : null)
            using(var translator = new TranslationService(
                    options.TranslateId,
                    options.TranslateSecret,
                    options.TranslateEndpoint)){
                foreach(var subtitle in SBVSubtitle.Parse(inputReader)) {
                    subtitle.Content = await translator.Translate(subtitle.Content, options.InputLanguage, options.OutputLanguage);
                    if(outputWriter != null){
                        outputWriter.WriteLine(subtitle);
                    } else{
                        Console.WriteLine(subtitle);
                    }
                }
            }
            
        }
    }
}
