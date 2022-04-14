using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.IO;

namespace youtube_download_and_converter_desktop{
    public class ApiClient{
        private const string baseUrl = "https://youtube-downloader-converter.herokuapp.com";

        public async Task<string> GetVideoName(string ytUrl){
            var client = new HttpClient();
            var response = await client.GetAsync(baseUrl + "/name?url=" + ytUrl);
            if(response.StatusCode != HttpStatusCode.OK){
                throw new WebException(await response.Content.ReadAsStringAsync());
            }
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<Stream> GetVideo(string ytUrl, string type){
            var client = new HttpClient();
            var response = await client.GetAsync(baseUrl + "/download?url=" + ytUrl + "&type=" + type);
            if(response.StatusCode != HttpStatusCode.OK){
                throw new WebException("Erro ao buscar video: " + await response.Content.ReadAsStringAsync());
            }
            return await response.Content.ReadAsStreamAsync();
        }
    }
}