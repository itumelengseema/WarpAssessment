using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WarpAssessment.Helpers;
using WarpAssessment.Interfaces;
using WarpAssessment.Models;
using WarpAssessment.Services;

// Build dependency injection container
var services = new ServiceCollection();

// Configure logging
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

// Configure settings
var authSettings = new AuthSettings();
var rateLimitSettings = new RateLimitSettings();
var uploadSettings = new UploadSettings();
var fileSettings = new FileSettings();

services.AddSingleton(authSettings);
services.AddSingleton(rateLimitSettings);
services.AddSingleton(uploadSettings);
services.AddSingleton(fileSettings);

// Register services
services.AddSingleton<IPasswordGenerator, PasswordGenerator>();
services.AddSingleton<IDictionaryService, DictionaryService>();
services.AddSingleton<IZipService, ZipService>();
services.AddSingleton<IAuthService, AuthService>();
services.AddSingleton<IUploadService, UploadService>();
services.AddSingleton<AttackService>();

// Register RateLimiter
services.AddSingleton(new RateLimiter(rateLimitSettings.AuthenticateRequestsPerSecond));

// Configure HttpClient
services.AddHttpClient<IAuthService, AuthService>()
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(authSettings.TimeoutSeconds);
    });

services.AddHttpClient<IUploadService, UploadService>()
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(uploadSettings.TimeoutSeconds);
    });

// Build service provider
var serviceProvider = services.BuildServiceProvider();
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("=== Warp Development Password Assessment Started ===");

    // Get base directory for file operations
    var baseDirectory = AppContext.BaseDirectory;
    var dictPath = Path.Combine(baseDirectory, fileSettings.DictionaryFileName);
    var cvPath = Path.Combine(baseDirectory, fileSettings.CvFileName);

    logger.LogInformation($"Base directory: {baseDirectory}");
    logger.LogInformation($"Dictionary path: {dictPath}");
    logger.LogInformation($"CV path: {cvPath}");

    // Step 1: Generate passwords
    logger.LogInformation("Step 1: Generating passwords...");
    var passwordGenerator = serviceProvider.GetRequiredService<IPasswordGenerator>();
    var passwords = passwordGenerator.Generate();

    // Step 2: Write passwords to dictionary file
    logger.LogInformation("Step 2: Writing passwords to dictionary file...");
    var dictService = serviceProvider.GetRequiredService<IDictionaryService>();
    await dictService.WriteToFileAsync(passwords, dictPath);
    logger.LogInformation($"Successfully wrote passwords to {dictPath}");

    // Step 3: Attempt authentication with each password
    logger.LogInformation("Step 3: Attempting authentication...");
    var authService = serviceProvider.GetRequiredService<IAuthService>();
    var attackService = serviceProvider.GetRequiredService<AttackService>();
    
    // Re-read passwords from file to ensure consistency
    var passwordsFromFile = await dictService.ReadFromFileAsync(dictPath);
    var uploadUrl = await attackService.ExecuteAsync(passwordsFromFile);

    if (uploadUrl == null)
    {
        logger.LogError("Failed to find correct password. Authentication unsuccessful.");
        return;
    }

    logger.LogInformation($"Correct password found! Upload URL: {uploadUrl}");

    // Step 4: Create ZIP file with required files
    logger.LogInformation("Step 4: Creating ZIP file...");
    var zipService = serviceProvider.GetRequiredService<IZipService>();
    
    // Verify required files exist
    if (!File.Exists(cvPath))
    {
        logger.LogError($"CV file not found at {cvPath}. Please ensure cv.pdf exists in the project directory.");
        return;
    }

    // Get all C# source files in the project
    var sourceFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.cs", SearchOption.AllDirectories)
        .Where(f => !f.Contains("bin") && !f.Contains("obj"))
        .ToList();

    logger.LogInformation($"Found {sourceFiles.Count} source files to include in ZIP");

    var filesToZip = new List<string> { dictPath, cvPath };
    filesToZip.AddRange(sourceFiles);

    var base64Zip = zipService.CreateBase64Zip(filesToZip.ToArray());
    logger.LogInformation($"ZIP created successfully. Base64 size: {base64Zip.Length} characters");

    // Step 5: Upload the ZIP file
    logger.LogInformation("Step 5: Uploading ZIP file...");
    var uploadService = serviceProvider.GetRequiredService<IUploadService>();
    await uploadService.UploadAsync(uploadUrl, base64Zip);

    logger.LogInformation("=== Warp Development Password Assessment Completed Successfully ===");
}
catch (Exception ex)
{
    logger.LogError($"Fatal error: {ex.Message}");
    logger.LogError($"Stack trace: {ex.StackTrace}");
    Environment.Exit(1);
}
