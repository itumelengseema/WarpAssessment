using System.IO.Compression;
using WarpAssessment.Interfaces;

namespace WarpAssessment.Services;

public class ZipService:IZipService
{
    public string CreateBase64Zip(string[] filePaths)
    {
       using var memoryStream = new MemoryStream();

       Console.WriteLine("ZIP START");

       using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen:true))
       {
           foreach (var file in filePaths)
           {
               var fileName = Path.GetFileName(file);
               var exists = File.Exists(file);
               Console.WriteLine($"{fileName} exists = {exists}");

               if (!exists)
               {
                   Console.WriteLine($"File {file} not found.");
                   continue;
               }
               
               Console.WriteLine($"Adding to zip: {fileName}");
               
               var entry = archive.CreateEntry(fileName);
               
               using (var entryStream = entry.Open())
               using (var fileStream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
               {
                   fileStream.CopyTo(entryStream);
               }
               
         
           }
       }
       memoryStream.Seek(0, SeekOrigin.Begin);
       
       return Convert.ToBase64String(memoryStream.ToArray());
    }
}