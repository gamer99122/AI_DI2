#!/bin/bash
# 批次創建所有範例的 .csproj 文件

echo "=== 創建 .csproj 文件 ==="
echo ""

created=0

# 查找所有包含 Program.cs 但沒有 .csproj 的目錄
find Examples -name "Program.cs" -type f | while read -r program_file; do
    dir=$(dirname "$program_file")
    project_name=$(basename "$dir")
    csproj_file="$dir/${project_name}.csproj"

    # 如果 .csproj 不存在，創建它
    if [ ! -f "$csproj_file" ]; then
        echo "創建: $csproj_file"

        # 根據項目名稱選擇模板
        if [[ $project_name == *"MinimalApi"* ]] || [[ $project_name == *"Controller"* ]] || \
           [[ $project_name == *"Razor"* ]] || [[ $project_name == *"Blazor"* ]] || \
           [[ $project_name == *"Middleware"* ]] || [[ $project_name == *"Filter"* ]]; then
            # Web 項目模板
            cat > "$csproj_file" << 'EOF'
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
EOF

        elif [[ $project_name == *"HttpClient"* ]]; then
            # HttpClient 項目
            cat > "$csproj_file" << 'EOF'
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
EOF

        elif [[ $project_name == *"Cache"* ]]; then
            # Cache 項目
            cat > "$csproj_file" << 'EOF'
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
EOF

        elif [[ $project_name == *"Performance"* ]] || [[ $project_name == *"ObjectPool"* ]]; then
            # ObjectPool 項目
            cat > "$csproj_file" << 'EOF'
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
EOF

        elif [[ $project_name == *"Logger"* ]] || [[ $project_name == *"Configuration"* ]] || \
             [[ $project_name == *"Options"* ]] || [[ $project_name == *"Hosted"* ]]; then
            # 完整模板（包含常用套件）
            cat > "$csproj_file" << 'EOF'
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
EOF

        else
            # 簡單模板（只有 DI）
            cat > "$csproj_file" << 'EOF'
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
EOF
        fi

        ((created++))
    fi
done

echo ""
echo "✅ 創建了 $created 個 .csproj 文件"
echo ""
echo "現在可以編譯和運行範例："
echo "  cd Examples/Basic/Basic01_WithoutDI"
echo "  dotnet run"
