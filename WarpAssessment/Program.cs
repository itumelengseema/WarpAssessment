using WarpAssessment.Services;

var generator = new PasswordGenerator();
var passwords = generator.Generate();

var dictService = new DictionaryService();
var dictPath = Path.Combine(AppContext.BaseDirectory, "dict.txt");
var cvPath = Path.Combine(AppContext.BaseDirectory, "cv.pdf");

await dictService.WriteToFileAsync(passwords, dictPath);

var httpClient = new HttpClient();

var authService = new AuthService(httpClient);
var attacker = new AttackService(authService);

var url = await attacker.ExecuteAsync(passwords);

if (url == null)
{
    Console.WriteLine("Password not found.");
    return;
}

var zipService = new ZipService();

var base64Zip = zipService.CreateBase64Zip(new[]
{
    dictPath,
    cvPath
});

var uploader = new UploadService(httpClient);
await uploader.UploadAsync(url, base64Zip);

Console.WriteLine("Done.");