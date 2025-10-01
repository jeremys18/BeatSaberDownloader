// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;
using System.Text;

string host = "identify-us-west-2.acrcloud.com";
string dataType = "audio";
string protocol = "https";
string endpoint = "/v1/identify";
string accessKey = "1d6ab347d3937b1e4a6a5ca8c59e7428";
string accessSecret = "AVm7lDzuZxir5C6PGSnMF9crqt0inTKvrAOsAxbY";
int timeout = 50 * 1000; // ms

byte[] wavFile = File.ReadAllBytes(@"C:\BeatSaber\SongFiles\song.egg").Take(4000).ToArray();

// var fileStream = new FileStream(filePath, FileMode.Open);

string method = "POST";
string sigVersion = "1";
string timestamp = DateTime.Now.Ticks.ToString();
string sigStr = method + "\n" + endpoint + "\n" + accessKey + "\n" + dataType + "\n" + sigVersion + "\n" + timestamp;
string signature = ComputeSignature(sigStr, accessSecret);
Console.WriteLine(signature);
var formContent = new MultipartFormDataContent();

formContent.Add(new StringContent(accessKey), "\"access_key\"");
formContent.Add(new StringContent(timestamp), "\"timestamp\"");
formContent.Add(new StringContent(signature), "\"signature\"");
formContent.Add(new StringContent(dataType), "\"data_type\"");
formContent.Add(new StringContent(sigVersion), "\"signature_version\"");
formContent.Add(new StringContent(wavFile.Length.ToString()), "\"sample_bytes\"");

var filestream = new ByteArrayContent(wavFile, 0, wavFile.Length);
filestream.Headers.Add("Content-Type", "application/octet-stream");
formContent.Add(filestream, "\"sample\"");

HttpClient client = new HttpClient();
var httpresponse = await client.PostAsync("https://" + host + "/v1/identify", formContent);
string responseBody = await httpresponse.Content.ReadAsStringAsync();

Console.WriteLine(responseBody);
Console.ReadLine();

static string ComputeSignature(string body, string secret)
{
    HMACSHA1 hmac = new HMACSHA1(Encoding.UTF8.GetBytes(secret));
    byte[] stringBytes = Encoding.UTF8.GetBytes(body);
    byte[] hashedValue = hmac.ComputeHash(stringBytes);
    string res = Convert.ToBase64String(hashedValue, 0, hashedValue.Length);
    return res;
}
