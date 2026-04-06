# WarpAssessment

## Overview

This is a .NET 8 console application that demonstrates:

- Password brute-force logic
- File validation for required resources
- In-memory ZIP archive creation
- Base64 encoding of compressed data
- Upload of the encoded payload

After successful authentication, the app zips `dict.txt` and `cv.pdf`, converts the ZIP to Base64, and uploads it.

## Technologies

- .NET 8
- C#
- `System.IO`
- `System.IO.Compression`

## How It Works

1. Generate password candidates and try each one until a valid password is found.
2. Validate required files (`dict.txt` and `cv.pdf`).
3. Create a ZIP archive in memory using `ZipArchive` and `MemoryStream`.
4. Convert the ZIP bytes to Base64.
5. Upload the Base64 payload.

Example success flow:

```text
SUCCESS: Pa5SwOrD
ZIP START
dict.txt exists = True
cv.pdf exists = True
Adding to zip: dict.txt
Adding to zip: cv.pdf
Upload Status: OK
Done.
```

## Project Structure

```text
WarpAssessment/
├── Program.cs
├── Services/
│   └── ZipService.cs
├── dict.txt
├── cv.pdf
├── WarpAssessment.csproj
└── README.md
```

## Run

From the solution root:

```powershell
dotnet build ".\WarpAssessment.sln"
dotnet run --project ".\WarpAssessment\WarpAssessment.csproj"
```

## Submission Checklist

- Ensure `cv.pdf` and `dict.txt` are present in `WarpAssessment/`.
- Confirm app runs: `dotnet run --project ".\WarpAssessment\WarpAssessment.csproj"`.
- Clean project output before submitting:

```powershell
dotnet clean ".\WarpAssessment.sln"
Remove-Item -Recurse -Force ".\WarpAssessment\bin" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force ".\WarpAssessment\obj" -ErrorAction SilentlyContinue
```

- Zip the full `WarpAssessment/` project folder for submission.

## Author

Itumeleng Seema

