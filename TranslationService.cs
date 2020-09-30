using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Translate;
using Amazon.Translate.Model;

namespace SBVTranslator {
    class TranslationService : IDisposable {
        
        private AmazonTranslateClient AWSTranslateClient;
        
        public TranslationService(string id, string secret, string endpoint) {
            this.AWSTranslateClient = new AmazonTranslateClient(id, secret, Amazon.RegionEndpoint.GetBySystemName(endpoint));
        }
        
        public async Task<string> Translate(string text, string sourceLanguageCode, string targetLanguageCode){
            var request = new TranslateTextRequest() {
                    Text = text,
                    SourceLanguageCode = sourceLanguageCode,
                    TargetLanguageCode = targetLanguageCode
                };
            var response = await this.AWSTranslateClient.TranslateTextAsync(request, CancellationToken.None);
            return response.TranslatedText;
        }
        
        public void Dispose() {
            this.AWSTranslateClient.Dispose();
        }
        
    }
}