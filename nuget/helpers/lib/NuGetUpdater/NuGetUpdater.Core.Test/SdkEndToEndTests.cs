﻿using System.Threading.Tasks;

using Xunit;

namespace NuGetUpdater.Core.Test;

public class SdkEndToEndTests : EndToEndTestBase
{
    [Fact]
    public async Task UpdateVersionAttribute_InProjectFile_ForPackageReferenceInclude()
    {
        // update Newtonsoft.Json from 9.0.1 to 13.0.1
        await TestUpdateForProject("Newtonsoft.Json", "9.0.1", "13.0.1",
            // initial
            """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>netstandard2.0</TargetFramework>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" Version="9.0.1" />
              </ItemGroup>
            </Project>
            """,
            // expected
            """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>netstandard2.0</TargetFramework>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" Version="13.0.1" />
              </ItemGroup>
            </Project>
            """);
    }

    [Fact]
    public async Task UpdateVersionAttribute_InProjectFile_ForPackageReferenceUpdate()
    {
        // update Newtonsoft.Json from 9.0.1 to 13.0.1
        await TestUpdateForProject("Newtonsoft.Json", "9.0.1", "13.0.1",
            // initial
            """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>netstandard2.0</TargetFramework>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Update="Newtonsoft.Json" Version="9.0.1" />
              </ItemGroup>
            </Project>
            """,
            // expected
            """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>netstandard2.0</TargetFramework>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Update="Newtonsoft.Json" Version="13.0.1" />
              </ItemGroup>
            </Project>
            """);
    }

    [Fact]
    public async Task UpdateVersionAttribute_InDirectoryPackages_ForPackageVersion()
    {
        // update Newtonsoft.Json from 9.0.1 to 13.0.1
        await TestUpdateForProject("Newtonsoft.Json", "9.0.1", "13.0.1",
            // initial
            projectContents: """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>netstandard2.0</TargetFramework>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" />
              </ItemGroup>
            </Project>
            """,
            additionalFiles: new[]
            {
                ("Directory.Packages.props", """
                    <Project>
                    <PropertyGroup>
                        <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
                    </PropertyGroup>

                    <ItemGroup>
                        <PackageVersion Include="Newtonsoft.Json" Version="9.0.1" />
                    </ItemGroup>
                    </Project>
                    """)
            },
            // expected
            expectedProjectContents: """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>netstandard2.0</TargetFramework>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" />
              </ItemGroup>
            </Project>
            """,
            additionalFilesExpected: new[]
            {
                ("Directory.Packages.props", """
                    <Project>
                    <PropertyGroup>
                        <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
                    </PropertyGroup>

                    <ItemGroup>
                        <PackageVersion Include="Newtonsoft.Json" Version="13.0.1" />
                    </ItemGroup>
                    </Project>
                    """)
            });
    }

    [Fact]
    public async Task UpdatePropertyValue_InProjectFile_ForPackageReferenceInclude()
    {
        await TestUpdateForProject("Newtonsoft.Json", "9.0.1", "13.0.1",
            // initial
            """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>netstandard2.0</TargetFramework>
                <NewtonsoftJsonPackageVersion>9.0.1</NewtonsoftJsonPackageVersion>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" Version="$(NewtonsoftJsonPackageVersion)" />
              </ItemGroup>
            </Project>
            """,
            // expected
            """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>netstandard2.0</TargetFramework>
                <NewtonsoftJsonPackageVersion>13.0.1</NewtonsoftJsonPackageVersion>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" Version="$(NewtonsoftJsonPackageVersion)" />
              </ItemGroup>
            </Project>
            """);
    }

    [Fact]
    public async Task UpdatePropertyValue_InProjectFile_ForPackageReferenceUpdate()
    {
        await TestUpdateForProject("Newtonsoft.Json", "9.0.1", "13.0.1",
            // initial
            """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>netstandard2.0</TargetFramework>
                <NewtonsoftJsonPackageVersion>9.0.1</NewtonsoftJsonPackageVersion>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Update="Newtonsoft.Json" Version="$(NewtonsoftJsonPackageVersion)" />
              </ItemGroup>
            </Project>
            """,
            // expected
            """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>netstandard2.0</TargetFramework>
                <NewtonsoftJsonPackageVersion>13.0.1</NewtonsoftJsonPackageVersion>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Update="Newtonsoft.Json" Version="$(NewtonsoftJsonPackageVersion)" />
              </ItemGroup>
            </Project>
            """);
    }

    [Fact]
    public async Task UpdatePropertyValue_InDirectoryProps_ForPackageVersion()
    {
        // update Newtonsoft.Json from 9.0.1 to 13.0.1
        await TestUpdateForProject("Newtonsoft.Json", "9.0.1", "13.0.1",
            // initial
            projectContents: """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>netstandard2.0</TargetFramework>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" />
              </ItemGroup>
            </Project>
            """,
            additionalFiles: new[]
            {
                ("Directory.Packages.props", """
                    <Project>
                    <PropertyGroup>
                        <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
                        <NewtonsoftJsonPackageVersion>9.0.1</NewtonsoftJsonPackageVersion>
                    </PropertyGroup>

                    <ItemGroup>
                        <PackageVersion Include="Newtonsoft.Json" Version="$(NewtonsoftJsonPackageVersion)" />
                    </ItemGroup>
                    </Project>
                    """)
            },
            // expected
            expectedProjectContents: """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>netstandard2.0</TargetFramework>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" />
              </ItemGroup>
            </Project>
            """,
            additionalFilesExpected: new[]
            {
                ("Directory.Packages.props", """
                    <Project>
                    <PropertyGroup>
                        <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
                        <NewtonsoftJsonPackageVersion>13.0.1</NewtonsoftJsonPackageVersion>
                    </PropertyGroup>

                    <ItemGroup>
                        <PackageVersion Include="Newtonsoft.Json" Version="$(NewtonsoftJsonPackageVersion)" />
                    </ItemGroup>
                    </Project>
                    """)
            });
    }

    [Fact]
    public async Task UpdatePropertyValue_InDirectoryProps_ForPackageReferenceInclude()
    {
        await TestUpdateForProject("Newtonsoft.Json", "9.0.1", "13.0.1",
            // initial project
            projectContents: """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>netstandard2.0</TargetFramework>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" Version="$(NewtonsoftJsonPackageVersion)" />
              </ItemGroup>
            </Project>
            """,
            additionalFiles: new[]
            {
                // initial props file
                ("Directory.Build.props", """
                    <Project>
                    <PropertyGroup>
                        <NewtonsoftJsonPackageVersion>9.0.1</NewtonsoftJsonPackageVersion>
                    </PropertyGroup>
                    </Project>
                    """)
            },
            // expected project
            expectedProjectContents: """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>netstandard2.0</TargetFramework>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" Version="$(NewtonsoftJsonPackageVersion)" />
              </ItemGroup>
            </Project>
            """,
            additionalFilesExpected: new[]
            {
                // expected props file
                ("Directory.Build.props", """
                    <Project>
                    <PropertyGroup>
                        <NewtonsoftJsonPackageVersion>13.0.1</NewtonsoftJsonPackageVersion>
                    </PropertyGroup>
                    </Project>
                    """)
            });
    }

    [Fact]
    public async Task UpdatePropertyValue_InProps_ForPackageReferenceInclude()
    {
        await TestUpdateForProject("Newtonsoft.Json", "9.0.1", "13.0.1",
            // initial project
            projectContents: """
            <Project Sdk="Microsoft.NET.Sdk">
              <Import Project="my-properties.props" />

              <PropertyGroup>
                <TargetFramework>netstandard2.0</TargetFramework>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" Version="$(NewtonsoftJsonPackageVersion)" />
              </ItemGroup>
            </Project>
            """,
            additionalFiles: new[]
            {
                // initial props file
                ("Version.props", """
                    <Project>
                    <PropertyGroup>
                        <NewtonsoftJsonPackageVersion>9.0.1</NewtonsoftJsonPackageVersion>
                    </PropertyGroup>
                    </Project>
                    """)
            },
            // expected project
            expectedProjectContents: """
            <Project Sdk="Microsoft.NET.Sdk">
              <Import Project="my-properties.props" />

              <PropertyGroup>
                <TargetFramework>netstandard2.0</TargetFramework>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" Version="$(NewtonsoftJsonPackageVersion)" />
              </ItemGroup>
            </Project>
            """,
            additionalFilesExpected: new[]
            {
                // expected props file
                ("Version.props", """
                    <Project>
                    <PropertyGroup>
                        <NewtonsoftJsonPackageVersion>13.0.1</NewtonsoftJsonPackageVersion>
                    </PropertyGroup>
                    </Project>
                    """)
            });
    }

    [Fact]
    public async Task UpdatePropertyValue_InProps_ForPackageVersion()
    {
        // update Newtonsoft.Json from 9.0.1 to 13.0.1
        await TestUpdateForProject("Newtonsoft.Json", "9.0.1", "13.0.1",
            // initial
            projectContents: """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>netstandard2.0</TargetFramework>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" />
              </ItemGroup>
            </Project>
            """,
            additionalFiles: new[]
            {
                // initial props files
                ("Directory.Packages.props", """
                    <Project>
                    <PropertyGroup>
                        <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
                    </PropertyGroup>

                    <ItemGroup>
                        <PackageVersion Include="Newtonsoft.Json" Version="$(NewtonsoftJsonPackageVersion)" />
                    </ItemGroup>
                    </Project>
                    """),
                ("Version.props", """
                    <Project>
                    <PropertyGroup>
                        <NewtonsoftJsonPackageVersion>9.0.1</NewtonsoftJsonPackageVersion>
                    </PropertyGroup>
                    </Project>
                    """)
            },
            // expected
            expectedProjectContents: """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>netstandard2.0</TargetFramework>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" />
              </ItemGroup>
            </Project>
            """,
            additionalFilesExpected: new[]
            {
                // expected props files
                ("Directory.Packages.props", """
                    <Project>
                    <PropertyGroup>
                        <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
                    </PropertyGroup>

                    <ItemGroup>
                        <PackageVersion Include="Newtonsoft.Json" Version="$(NewtonsoftJsonPackageVersion)" />
                    </ItemGroup>
                    </Project>
                    """),
                ("Version.props", """
                    <Project>
                    <PropertyGroup>
                        <NewtonsoftJsonPackageVersion>13.0.1</NewtonsoftJsonPackageVersion>
                    </PropertyGroup>
                    </Project>
                    """)
            });
    }

    [Fact]
    public async Task UpdatePropertyValue_InProps_ThenSubstituted_ForPackageVersion()
    {
        // update Newtonsoft.Json from 9.0.1 to 13.0.1
        await TestUpdateForProject("Newtonsoft.Json", "9.0.1", "13.0.1",
            // initial
            projectContents: """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>netstandard2.0</TargetFramework>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" />
              </ItemGroup>
            </Project>
            """,
            additionalFiles: new[]
            {
                // initial props files
                ("Directory.Packages.props", """
                    <Project>
                    <PropertyGroup>
                        <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
                        <NewtonsoftJsonPackageVersion>$(NewtonsoftJsonVersion)</NewtonsoftJsonPackageVersion>
                    </PropertyGroup>

                    <ItemGroup>
                        <PackageVersion Include="Newtonsoft.Json" Version="$(NewtonsoftJsonPackageVersion)" />
                    </ItemGroup>
                    </Project>
                    """),
                ("Version.props", """
                    <Project>
                    <PropertyGroup>
                        <NewtonsoftJsonVersion>9.0.1</NewtonsoftJsonVersion>
                    </PropertyGroup>
                    </Project>
                    """)
            },
            // expected
            expectedProjectContents: """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>netstandard2.0</TargetFramework>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Include="Newtonsoft.Json" />
              </ItemGroup>
            </Project>
            """,
            additionalFilesExpected: new[]
            {
                // expected props files
                ("Directory.Packages.props", """
                    <Project>
                    <PropertyGroup>
                        <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
                        <NewtonsoftJsonPackageVersion>$(NewtonsoftJsonVersion)</NewtonsoftJsonPackageVersion>
                    </PropertyGroup>

                    <ItemGroup>
                        <PackageVersion Include="Newtonsoft.Json" Version="$(NewtonsoftJsonPackageVersion)" />
                    </ItemGroup>
                    </Project>
                    """),
                ("Version.props", """
                    <Project>
                    <PropertyGroup>
                        <NewtonsoftJsonVersion>13.0.1</NewtonsoftJsonVersion>
                    </PropertyGroup>
                    </Project>
                    """)
            });
    }
}
