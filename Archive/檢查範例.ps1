# 檢查所有範例是否正確
# 並為缺少 .csproj 的範例創建項目文件

$ErrorActionPreference = "Continue"

# 基本 .csproj 模板
$basicTemplate = @'
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Options" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging.Console" Version="8.0.0" />
  </ItemGroup>

</Project>
'@

$webTemplate = @'
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
  </ItemGroup>

</Project>
'@

$httpTemplate = @'
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Http" Version="8.0.0" />
  </ItemGroup>

</Project>
'@

$cacheTemplate = @'
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Caching.Memory" Version="8.0.0" />
  </ItemGroup>

</Project>
'@

$poolTemplate = @'
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.ObjectPool" Version="8.0.0" />
  </ItemGroup>

</Project>
'@

$simpleTemplate = @'
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
  </ItemGroup>

</Project>
'@

Write-Host "=== 檢查並創建 .csproj 文件 ===" -ForegroundColor Cyan
Write-Host ""

$totalCount = 0
$successCount = 0
$failedCount = 0
$createdCount = 0

# 查找所有 Program.cs 文件
$programFiles = Get-ChildItem -Path "Examples" -Recurse -Filter "Program.cs"

foreach ($file in $programFiles) {
    $dir = $file.Directory
    $projectName = $dir.Name
    $csprojPath = Join-Path $dir.FullName "$projectName.csproj"

    $totalCount++

    # 如果 .csproj 不存在，創建它
    if (!(Test-Path $csprojPath)) {
        Write-Host "  [創建] $projectName.csproj" -ForegroundColor Yellow

        # 根據項目名稱選擇模板
        $template = $simpleTemplate
        if ($projectName -match "MinimalApi|Controller|Razor|Blazor|Middleware|Filter") {
            $template = $webTemplate
        } elseif ($projectName -match "HttpClient") {
            $template = $httpTemplate
        } elseif ($projectName -match "MemoryCache|Cache") {
            $template = $cacheTemplate
        } elseif ($projectName -match "Performance|ObjectPool") {
            $template = $poolTemplate
        } elseif ($projectName -match "Logger|Configuration|Options|Hosted") {
            $template = $basicTemplate
        }

        Set-Content -Path $csprojPath -Value $template -Encoding UTF8
        $createdCount++
    }

    # 嘗試編譯
    Write-Host "  [檢查] $projectName" -NoNewline
    Push-Location $dir.FullName

    $buildOutput = dotnet build --nologo -v q 2>&1
    $buildSuccess = $LASTEXITCODE -eq 0

    Pop-Location

    if ($buildSuccess) {
        Write-Host " ✅" -ForegroundColor Green
        $successCount++
    } else {
        Write-Host " ❌" -ForegroundColor Red
        Write-Host "    錯誤: $buildOutput" -ForegroundColor Red
        $failedCount++
    }
}

Write-Host ""
Write-Host "=== 檢查結果 ===" -ForegroundColor Cyan
Write-Host "總範例數:   $totalCount"
Write-Host "創建項目:   $createdCount" -ForegroundColor Yellow
Write-Host "編譯成功:   $successCount" -ForegroundColor Green
Write-Host "編譯失敗:   $failedCount" -ForegroundColor Red
Write-Host ""

if ($failedCount -eq 0) {
    Write-Host "✅ 所有範例都可以正確編譯！" -ForegroundColor Green
} else {
    Write-Host "⚠️ 有 $failedCount 個範例編譯失敗，請檢查錯誤訊息。" -ForegroundColor Yellow
}
