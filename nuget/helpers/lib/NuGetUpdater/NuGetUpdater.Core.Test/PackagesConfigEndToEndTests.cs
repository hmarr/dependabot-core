﻿using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

namespace NuGetUpdater.Core.Test;

public class PackagesConfigEndToEndTests : EndToEndTestBase
{
    public PackagesConfigEndToEndTests()
    {
        MSBuildHelper.RegisterMSBuild();
    }

    [Fact]
    public async Task UpdateSingleDependencyInPackagesConfig()
    {
        // update Newtonsoft.Json from 7.0.1 to 13.0.1
        await TestUpdateForProject("Newtonsoft.Json", "7.0.1", "13.0.1",
            // existing
            """
            <Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
              <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
              <PropertyGroup>
                <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
              </PropertyGroup>
              <ItemGroup>
                <None Include="packages.config" />
              </ItemGroup>
              <ItemGroup>
                <Reference Include="Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed">
                  <HintPath>packages\Newtonsoft.Json.7.0.1\lib\net45\Newtonsoft.Json.dll</HintPath>
                  <Private>True</Private>
                </Reference>
              </ItemGroup>
              <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
            </Project>
            """,
            """
            <packages>
              <package id="Newtonsoft.Json" version="7.0.1" targetFramework="net45" />
            </packages>
            """,
            // expected
            """
            <Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
              <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
              <PropertyGroup>
                <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
              </PropertyGroup>
              <ItemGroup>
                <None Include="packages.config" />
              </ItemGroup>
              <ItemGroup>
                <Reference Include="Newtonsoft.Json, Version=13.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed">
                  <HintPath>packages\Newtonsoft.Json.13.0.1\lib\net45\Newtonsoft.Json.dll</HintPath>
                  <Private>True</Private>
                </Reference>
              </ItemGroup>
              <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
            </Project>
            """,
            """
            <?xml version="1.0" encoding="utf-8"?>
            <packages>
              <package id="Newtonsoft.Json" version="13.0.1" targetFramework="net45" />
            </packages>
            """);
    }

    [Fact]
    public async Task UpdateSingleDependencyInPackagesConfigButNotToLatest()
    {
        // update Newtonsoft.Json from 7.0.1 to 9.0.1, purposefully not updating all the way to the newest
        await TestUpdateForProject("Newtonsoft.Json", "7.0.1", "9.0.1",
            // existing
            """
            <Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
              <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
              <PropertyGroup>
                <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
              </PropertyGroup>
              <ItemGroup>
                <None Include="packages.config" />
              </ItemGroup>
              <ItemGroup>
                <Reference Include="Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed">
                  <HintPath>packages\Newtonsoft.Json.7.0.1\lib\net45\Newtonsoft.Json.dll</HintPath>
                  <Private>True</Private>
                </Reference>
              </ItemGroup>
              <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
            </Project>
            """,
            """
            <packages>
              <package id="Newtonsoft.Json" version="7.0.1" targetFramework="net45" />
            </packages>
            """,
            // expected
            """
            <Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
              <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
              <PropertyGroup>
                <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
              </PropertyGroup>
              <ItemGroup>
                <None Include="packages.config" />
              </ItemGroup>
              <ItemGroup>
                <Reference Include="Newtonsoft.Json, Version=9.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed">
                  <HintPath>packages\Newtonsoft.Json.9.0.1\lib\net45\Newtonsoft.Json.dll</HintPath>
                  <Private>True</Private>
                </Reference>
              </ItemGroup>
              <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
            </Project>
            """,
            """
            <?xml version="1.0" encoding="utf-8"?>
            <packages>
              <package id="Newtonsoft.Json" version="9.0.1" targetFramework="net45" />
            </packages>
            """);
    }

    [Fact]
    public async Task UpdateSpecifiedVersionInPackagesConfigButNotOthers()
    {
        // update Newtonsoft.Json from 7.0.1 to 13.0.1, but leave HtmlAgilityPack alone
        await TestUpdateForProject("Newtonsoft.Json", "7.0.1", "13.0.1",
            // existing
            """
            <Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
              <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
              <PropertyGroup>
                <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
              </PropertyGroup>
              <ItemGroup>
                <None Include="packages.config" />
              </ItemGroup>
              <ItemGroup>
                <Reference Include="HtmlAgilityPack, Version=1.11.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed">
                  <HintPath>packages\HtmlAgilityPack.1.11.0\lib\net45\HtmlAgilityPack.dll</HintPath>
                  <Private>True</Private>
                </Reference>
                <Reference Include="Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed">
                  <HintPath>packages\Newtonsoft.Json.7.0.1\lib\net45\Newtonsoft.Json.dll</HintPath>
                  <Private>True</Private>
                </Reference>
              </ItemGroup>
              <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
            </Project>
            """,
            """
            <packages>
              <package id="HtmlAgilityPack" version="1.11.0" targetFramework="net45" />
              <package id="Newtonsoft.Json" version="7.0.1" targetFramework="net45" />
            </packages>
            """,
            // expected
            """
            <Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
              <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
              <PropertyGroup>
                <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
              </PropertyGroup>
              <ItemGroup>
                <None Include="packages.config" />
              </ItemGroup>
              <ItemGroup>
                <Reference Include="HtmlAgilityPack, Version=1.11.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed">
                  <HintPath>packages\HtmlAgilityPack.1.11.0\lib\net45\HtmlAgilityPack.dll</HintPath>
                  <Private>True</Private>
                </Reference>
                <Reference Include="Newtonsoft.Json, Version=13.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed">
                  <HintPath>packages\Newtonsoft.Json.13.0.1\lib\net45\Newtonsoft.Json.dll</HintPath>
                  <Private>True</Private>
                </Reference>
              </ItemGroup>
              <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
            </Project>
            """,
            """
            <?xml version="1.0" encoding="utf-8"?>
            <packages>
              <package id="HtmlAgilityPack" version="1.11.0" targetFramework="net45" />
              <package id="Newtonsoft.Json" version="13.0.1" targetFramework="net45" />
            </packages>
            """);
    }

    [Fact]
    public async Task UpdatePackagesConfigWithNonStandardLocationOfPackagesDirectory()
    {
        // update Newtonsoft.Json from 7.0.1 to 13.0.1 with the actual assembly in a non-standard location
        await TestUpdateForProject("Newtonsoft.Json", "7.0.1", "13.0.1",
            // existing
            """
            <Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
              <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
              <PropertyGroup>
                <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
              </PropertyGroup>
              <ItemGroup>
                <None Include="packages.config" />
              </ItemGroup>
              <ItemGroup>
                <Reference Include="Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed">
                  <HintPath>some-non-standard-location\Newtonsoft.Json.7.0.1\lib\net45\Newtonsoft.Json.dll</HintPath>
                  <Private>True</Private>
                </Reference>
              </ItemGroup>
              <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
            </Project>
            """,
            """
            <packages>
              <package id="Newtonsoft.Json" version="7.0.1" targetFramework="net45" />
            </packages>
            """,
            // expected
            """
            <Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
              <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
              <PropertyGroup>
                <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
              </PropertyGroup>
              <ItemGroup>
                <None Include="packages.config" />
              </ItemGroup>
              <ItemGroup>
                <Reference Include="Newtonsoft.Json, Version=13.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed">
                  <HintPath>some-non-standard-location\Newtonsoft.Json.13.0.1\lib\net45\Newtonsoft.Json.dll</HintPath>
                  <Private>True</Private>
                </Reference>
              </ItemGroup>
              <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
            </Project>
            """,
            """
            <?xml version="1.0" encoding="utf-8"?>
            <packages>
              <package id="Newtonsoft.Json" version="13.0.1" targetFramework="net45" />
            </packages>
            """);
    }

    [Fact]
    public async Task UpdateBindingRedirectInAppConfig()
    {
        await TestUpdateForProject("Newtonsoft.Json", "7.0.1", "13.0.1",
            projectContents: """
                <Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
                  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
                  <PropertyGroup>
                    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
                  </PropertyGroup>
                  <ItemGroup>
                    <None Include="packages.config" />
                  </ItemGroup>
                  <ItemGroup>
                    <None Include="app.config" />
                  </ItemGroup>
                  <ItemGroup>
                    <Reference Include="Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed">
                      <HintPath>packages\Newtonsoft.Json.7.0.1\lib\net45\Newtonsoft.Json.dll</HintPath>
                      <Private>True</Private>
                    </Reference>
                  </ItemGroup>
                  <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
                </Project>
                """,
            packagesConfigContents: """
                <packages>
                  <package id="Newtonsoft.Json" version="7.0.1" targetFramework="net45" />
                </packages>
                """,
            additionalFiles: new[]
            {
                ("app.config", """
                    <configuration>
                      <runtime>
                        <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
                          <dependentAssembly>
                            <assemblyIdentity name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" culture="neutral" />
                            <bindingRedirect oldVersion="0.0.0.0-7.0.0.0" newVersion="7.0.0.0" />
                          </dependentAssembly>
                        </assemblyBinding>
                      </runtime>
                    </configuration>
                    """)
            },
            expectedProjectContents: """
                <Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
                  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
                  <PropertyGroup>
                    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
                  </PropertyGroup>
                  <ItemGroup>
                    <None Include="packages.config" />
                  </ItemGroup>
                  <ItemGroup>
                    <None Include="app.config" />
                  </ItemGroup>
                  <ItemGroup>
                    <Reference Include="Newtonsoft.Json, Version=13.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed">
                      <HintPath>packages\Newtonsoft.Json.13.0.1\lib\net45\Newtonsoft.Json.dll</HintPath>
                      <Private>True</Private>
                    </Reference>
                  </ItemGroup>
                  <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
                </Project>
                """,
            expectedPackagesConfigContents: """
                <?xml version="1.0" encoding="utf-8"?>
                <packages>
                  <package id="Newtonsoft.Json" version="13.0.1" targetFramework="net45" />
                </packages>
                """,
            additionalFilesExpected: new[]
            {
                ("app.config", """
                    <configuration>
                      <runtime>
                        <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
                          <dependentAssembly>
                            <assemblyIdentity name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" culture="neutral" />
                            <bindingRedirect oldVersion="0.0.0.0-13.0.0.0" newVersion="13.0.0.0" />
                          </dependentAssembly>
                        </assemblyBinding>
                      </runtime>
                    </configuration>
                    """)
            });
    }

    [Fact]
    public async Task UpdateBindingRedirectInWebConfig()
    {
        await TestUpdateForProject("Newtonsoft.Json", "7.0.1", "13.0.1",
            projectContents: """
                <Project ToolsVersion="4.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
                  <PropertyGroup>
                    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
                    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
                    <ProductVersion>
                    </ProductVersion>
                    <SchemaVersion>2.0</SchemaVersion>
                    <ProjectGuid>ac83fc79-b637-445b-acb0-9be238ad077f</ProjectGuid>
                    <ProjectTypeGuids>{349c5851-65df-11da-9384-00065b846f21};{fae04ec0-301f-11d3-bf4b-00c04f79efbc}</ProjectTypeGuids>
                    <OutputType>Library</OutputType>
                    <AppDesignerFolder>Properties</AppDesignerFolder>
                    <RootNamespace>TestProject</RootNamespace>
                    <AssemblyName>TestProject</AssemblyName>
                    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
                  </PropertyGroup>
                  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
                    <DebugSymbols>true</DebugSymbols>
                    <DebugType>full</DebugType>
                    <Optimize>false</Optimize>
                    <OutputPath>bin\</OutputPath>
                    <DefineConstants>DEBUG;TRACE</DefineConstants>
                    <ErrorReport>prompt</ErrorReport>
                    <WarningLevel>4</WarningLevel>
                  </PropertyGroup>
                  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
                    <DebugType>pdbonly</DebugType>
                    <Optimize>true</Optimize>
                    <OutputPath>bin\</OutputPath>
                    <DefineConstants>TRACE</DefineConstants>
                    <ErrorReport>prompt</ErrorReport>
                    <WarningLevel>4</WarningLevel>
                  </PropertyGroup>
                  <ItemGroup>
                    <Reference Include="Microsoft.CSharp" />
                    <Reference Include="Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed">
                      <HintPath>packages\Newtonsoft.Json.7.0.1\lib\net45\Newtonsoft.Json.dll</HintPath>
                      <Private>True</Private>
                    </Reference>
                    <Reference Include="System.Web.DynamicData" />
                    <Reference Include="System.Web.Entity" />
                    <Reference Include="System.Web.ApplicationServices" />
                    <Reference Include="System" />
                    <Reference Include="System.Data" />
                    <Reference Include="System.Core" />
                    <Reference Include="System.Data.DataSetExtensions" />
                    <Reference Include="System.Web.Extensions" />
                    <Reference Include="System.Xml.Linq" />
                    <Reference Include="System.Drawing" />
                    <Reference Include="System.Web" />
                    <Reference Include="System.Xml" />
                    <Reference Include="System.Configuration" />
                    <Reference Include="System.Web.Services" />
                    <Reference Include="System.EnterpriseServices" />
                  </ItemGroup>
                  <ItemGroup>
                    <None Include="packages.config" />
                    <Content Include="web.config" />
                    <Content Include="web.Debug.config">
                      <DependentUpon>web.config</DependentUpon>
                    </Content>
                    <Content Include="web.Release.config">
                      <DependentUpon>web.config</DependentUpon>
                    </Content>
                  </ItemGroup>
                  <ItemGroup>
                    <Compile Include="Properties\AssemblyInfo.cs" />
                  </ItemGroup>
                  <Import Project="$(MSBuildBinPath)\Microsoft.CSharp.targets" />
                  <Import Project="$(VSToolsPath)\WebApplications\Microsoft.WebApplication.targets" Condition="'$(VSToolsPath)' != ''" />
                  <!-- To modify your build process, add your task inside one of the targets below and uncomment it.
                        Other similar extension points exist, see Microsoft.Common.targets.
                  <Target Name="BeforeBuild">
                  </Target>
                  <Target Name="AfterBuild">
                  </Target>
                  -->
                </Project>
                """,
            packagesConfigContents: """
                <packages>
                  <package id="Newtonsoft.Json" version="7.0.1" targetFramework="net45" />
                </packages>
                """,
            additionalFiles: new[]
            {
                ("web.config", """
                    <configuration>
                      <runtime>
                        <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
                          <dependentAssembly>
                            <assemblyIdentity name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" culture="neutral" />
                            <bindingRedirect oldVersion="0.0.0.0-7.0.0.0" newVersion="7.0.0.0" />
                          </dependentAssembly>
                        </assemblyBinding>
                      </runtime>
                    </configuration>
                    """)
            },
            expectedProjectContents: """
                <Project ToolsVersion="4.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
                  <PropertyGroup>
                    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
                    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
                    <ProductVersion>
                    </ProductVersion>
                    <SchemaVersion>2.0</SchemaVersion>
                    <ProjectGuid>ac83fc79-b637-445b-acb0-9be238ad077f</ProjectGuid>
                    <ProjectTypeGuids>{349c5851-65df-11da-9384-00065b846f21};{fae04ec0-301f-11d3-bf4b-00c04f79efbc}</ProjectTypeGuids>
                    <OutputType>Library</OutputType>
                    <AppDesignerFolder>Properties</AppDesignerFolder>
                    <RootNamespace>TestProject</RootNamespace>
                    <AssemblyName>TestProject</AssemblyName>
                    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
                  </PropertyGroup>
                  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
                    <DebugSymbols>true</DebugSymbols>
                    <DebugType>full</DebugType>
                    <Optimize>false</Optimize>
                    <OutputPath>bin\</OutputPath>
                    <DefineConstants>DEBUG;TRACE</DefineConstants>
                    <ErrorReport>prompt</ErrorReport>
                    <WarningLevel>4</WarningLevel>
                  </PropertyGroup>
                  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
                    <DebugType>pdbonly</DebugType>
                    <Optimize>true</Optimize>
                    <OutputPath>bin\</OutputPath>
                    <DefineConstants>TRACE</DefineConstants>
                    <ErrorReport>prompt</ErrorReport>
                    <WarningLevel>4</WarningLevel>
                  </PropertyGroup>
                  <ItemGroup>
                    <Reference Include="Microsoft.CSharp" />
                    <Reference Include="Newtonsoft.Json, Version=13.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed">
                      <HintPath>packages\Newtonsoft.Json.13.0.1\lib\net45\Newtonsoft.Json.dll</HintPath>
                      <Private>True</Private>
                    </Reference>
                    <Reference Include="System.Web.DynamicData" />
                    <Reference Include="System.Web.Entity" />
                    <Reference Include="System.Web.ApplicationServices" />
                    <Reference Include="System" />
                    <Reference Include="System.Data" />
                    <Reference Include="System.Core" />
                    <Reference Include="System.Data.DataSetExtensions" />
                    <Reference Include="System.Web.Extensions" />
                    <Reference Include="System.Xml.Linq" />
                    <Reference Include="System.Drawing" />
                    <Reference Include="System.Web" />
                    <Reference Include="System.Xml" />
                    <Reference Include="System.Configuration" />
                    <Reference Include="System.Web.Services" />
                    <Reference Include="System.EnterpriseServices" />
                  </ItemGroup>
                  <ItemGroup>
                    <None Include="packages.config" />
                    <Content Include="web.config" />
                    <Content Include="web.Debug.config">
                      <DependentUpon>web.config</DependentUpon>
                    </Content>
                    <Content Include="web.Release.config">
                      <DependentUpon>web.config</DependentUpon>
                    </Content>
                  </ItemGroup>
                  <ItemGroup>
                    <Compile Include="Properties\AssemblyInfo.cs" />
                  </ItemGroup>
                  <Import Project="$(MSBuildBinPath)\Microsoft.CSharp.targets" />
                  <Import Project="$(VSToolsPath)\WebApplications\Microsoft.WebApplication.targets" Condition="'$(VSToolsPath)' != ''" />
                  <!-- To modify your build process, add your task inside one of the targets below and uncomment it.
                        Other similar extension points exist, see Microsoft.Common.targets.
                  <Target Name="BeforeBuild">
                  </Target>
                  <Target Name="AfterBuild">
                  </Target>
                  -->
                </Project>
                """,
            expectedPackagesConfigContents: """
                <?xml version="1.0" encoding="utf-8"?>
                <packages>
                  <package id="Newtonsoft.Json" version="13.0.1" targetFramework="net45" />
                </packages>
                """,
            additionalFilesExpected: new[]
            {
                ("web.config", """
                    <configuration>
                      <runtime>
                        <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
                          <dependentAssembly>
                            <assemblyIdentity name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" culture="neutral" />
                            <bindingRedirect oldVersion="0.0.0.0-13.0.0.0" newVersion="13.0.0.0" />
                          </dependentAssembly>
                        </assemblyBinding>
                      </runtime>
                    </configuration>
                    """)
            });
    }

    protected static async Task TestUpdateForProject(
        string dependencyName,
        string oldVersion,
        string newVersion,
        string projectContents,
        string packagesConfigContents,
        string expectedProjectContents,
        string expectedPackagesConfigContents,
        (string Path, string Content)[]? additionalFiles = null,
        (string Path, string Content)[]? additionalFilesExpected = null)
    {
        var realizedAdditionalFiles = new List<(string Path, string Content)>()
        {
            ("packages.config", packagesConfigContents),
        };
        if (additionalFiles is not null)
        {
            realizedAdditionalFiles.AddRange(additionalFiles);
        }

        var realizedAdditionalFilesExpected = new List<(string Path, string Content)>()
        {
            ("packages.config", expectedPackagesConfigContents),
        };
        if (additionalFilesExpected is not null)
        {
            realizedAdditionalFilesExpected.AddRange(additionalFilesExpected);
        }

        await TestUpdateForProject(dependencyName, oldVersion, newVersion, projectContents, expectedProjectContents, realizedAdditionalFiles.ToArray(), realizedAdditionalFilesExpected.ToArray());
    }
}