# 批次創建所有範例的 .csproj 文件

$projectTemplate = @'
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
    <PackageReference Include="Microsoft.Extensions.Http" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Caching.Memory" Version="8.0.0" />
  </ItemGroup>

</Project>
'@

$webProjectTemplate = @'
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

</Project>
'@

# 基礎範例
for ($i = 1; $i -le 40; $i++) {
    $num = $i.ToString("D2")
    $dirs = Get-ChildItem -Path "Examples\Basic" -Directory -Filter "Basic${num}_*"

    foreach ($dir in $dirs) {
        $projectName = $dir.Name
        $csprojPath = Join-Path $dir.FullName "$projectName.csproj"

        if (!(Test-Path $csprojPath)) {
            # 判斷是否為 Web 專案
            if ($projectName -match "MinimalApi|Controller|Razor|Blazor|Middleware|Filter") {
                Set-Content -Path $csprojPath -Value $webProjectTemplate
            } else {
                Set-Content -Path $csprojPath -Value $projectTemplate
            }
            Write-Host "✅ 創建: $csprojPath" -ForegroundColor Green
        }
    }
}

# 中階範例
for ($i = 1; $i -le 5; $i++) {
    $num = $i.ToString("D2")
    $dirs = Get-ChildItem -Path "Examples\Intermediate" -Directory -Filter "Intermediate${num}_*" -ErrorAction SilentlyContinue

    foreach ($dir in $dirs) {
        $projectName = $dir.Name
        $csprojPath = Join-Path $dir.FullName "$projectName.csproj"

        if (!(Test-Path $csprojPath)) {
            Set-Content -Path $csprojPath -Value $projectTemplate
            Write-Host "✅ 創建: $csprojPath" -ForegroundColor Green
        }
    }
}

# 進階範例
for ($i = 1; $i -le 5; $i++) {
    $num = $i.ToString("D2")
    $dirs = Get-ChildItem -Path "Examples\Advanced" -Directory -Filter "Advanced${num}_*" -ErrorAction SilentlyContinue

    foreach ($dir in $dirs) {
        $projectName = $dir.Name
        $csprojPath = Join-Path $dir.FullName "$projectName.csproj"

        if (!(Test-Path $csprojPath)) {
            Set-Content -Path $csprojPath -Value $projectTemplate
            Write-Host "✅ 創建: $csprojPath" -ForegroundColor Green
        }
    }
}

Write-Host "`n✅ 所有 .csproj 文件創建完成！" -ForegroundColor Cyan
