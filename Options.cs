using System;
using Microsoft.Extensions.Configuration;

namespace SBVTranslator {
    class Options {
        
        public string TranslateId { get; set; }
        public string TranslateSecret { get; set; }
        public string TranslateEndpoint { get; set; }
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public string InputLanguage { get; set; }
        public string OutputLanguage { get; set; }
        
        public Options(string[] cmdLineArgs){
            var config = ReadConfiguration(cmdLineArgs);
            this.TranslateId = config["TranslateID"];
            this.TranslateSecret = config["TranslateSecret"];
            this.InputFile = config["InputFile"];
            this.OutputFile = config["OutputFile"];
            this.TranslateEndpoint = config["TranslateEndpoint"] ?? "us-east-1";
            this.InputLanguage = config["InputLanguage"] ?? "en";
            this.OutputLanguage = config["OutputLanguage"] ?? "en";
        }
        
        // Returns null if there are no errors
        public string Errors() {
            if(this.InputFile == null)
                return "You must provide an 'InputFile'.";
            if(this.TranslateId == null)
                return "You must provide 'TranslateID': An AWS account"
                    + " ID for accessing the AWS Translation services.";
            if(this.TranslateSecret == null)
                return "You must provide 'TranslateSecret': An AWS account"
                    + " secret for accessing the AWS Translation services.";
            if(this.InputLanguage == this.OutputLanguage)
                return "You must provide an 'InputLanguage' or an 'OutputLanguage'"
                    + " (default is 'en'), and they must differ.";
            return null;
        }
        
        private IConfiguration ReadConfiguration(string[] args){
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            configBuilder.AddCommandLine(args);
#if DEBUG
            configBuilder.AddUserSecrets<Program>();
#endif
            return configBuilder.Build();
        }
        
    }
}