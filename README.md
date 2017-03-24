# xamarin-forms-fsharp-boilerplate
Template for a Xamarin.Forms application with a F# PCL, for Visual Studio 2017

## Why I made this
After the release of Visual Studio 2017, where Xamarin is natively bundled with the product, I wanted to try out building cross-platform mobile apps on the .NET stack. Since VS 2017 also released with substantial improvements to F# tooling, I wanted to see how realistic it was to build functional (har-har) mobile apps on this stack.

Visual Studio 2017 shipped with many new project templates in the File > New > Project menu, including a new and simplified C# Xamarin.Forms template, but a F# Xamarin.Forms template was not one of them. Furthermore, I couldn't find a suitable template online on the Visual Studio Marketplace.

The vast majority F# templates for Xamarin.Forms on Github were built with previous versions of Visual Studio and Xamarin and had quite a bit of targeting issues that I found quite tedious to fix by hand. Many of these example solutions did not include a UWP target either, since that would have to be a C# project. I thought I would generate my own boilerplate solution that:
 1. Was built with Visual Studio 2017 and the accompanying version of Xamarin to avoid targeting errors
 2. Would mix a main F# PCL project for the majority of the Xamarin.Forms code and C# device-specific projects in order to avoid compatibility issues and to target UWP along with Android and iOS

## How I made this
- To generate this solution, I used VS 2017 Community Edition, build 15.0.0-RTW+26228.4 (stock version available on the official site on launch day).
- Generate a C# Xamarin.Forms project in Visual Studio 2017. Make sure to select the PCL approach rather than the Shared Project approach, since Sared Projects do not support F# yet, but you can swap out the C# PCL with a F# PCL.
- Generate a F# PCL project inside the solution. Ideally, you'd be able to name this PCL project the same thing as your Xamarin.Forms solution, but you cannot yet, since there is already a project with this name. We will rename this later after we've gotten rid of the C# PCL.
- Create the files `App.xaml`, `App.xaml.fs`, `MainPage.xaml`, and `MainPage.xaml.fs`. Make sure that the `.fsproj` file is set to compile the XAML. (See the **Notes** section below for more detail about the `<EmbeddedResource />` tags.)
- `App.xaml` and `MainPage.xaml` can be copied over directly from the C# PCL project, since XAML isn't language dependent. Just make sure that you change the namespace name.
- You will need to translate the code in the `MainPage.xaml.cs` and `App.xaml.cs` to their F# counterparts. I followed the example given in [this repo](https://github.com/FSharpForCSharpDevelopers/CrossPlatformXamarinForms/blob/master/FSharpCrossPlatformXamarinForms/FSharpCrossPlatformXamarinForms/MainPage.xaml.fs).
- If the Xamarin.Forms assemblies are not referenced in the F# PCL project, you will have to add it by right-clicking References > Add Reference > Assemblies > Browse and then find the Xamarin dll's. They will probably be in the `packages` folder in the outermost solution folder. Just make sure that all the Xamarin dll references in the C# PCL is also present in the F# PCL. Keep in mind that you might have to open up the `.fsproj` file later and manually change the dll reference paths to be more relative, since that Browse window likes giving rather elongated or absolute paths. (Avoid paths that have your Users folder, Documents folder, etc.)
- Go in the iOS, Android, and UWP projects and add the F# PCL as a reference, if it has not been done already.
- Make sure that right click the solution in Solution Explorer > Properties > Configuration Properties and that all projects are selected to build, and as many as possible are selected to deploy (at least Android and UWP).
- Now that you no longer need the C# PCL project, you can remove it! Be sure to go in Windows Explorer and delete the **correct** project folder (many of them are going to be named the same as your Xamarin.Forms project), since dereferencing it in a Visual Studio solution does not actually delete the project files.
- You can now rename your F# PCL project! Right-click on the project in Solution Explorer to rename it, but make sure you go through all of the source files after to rename namespaces, etc. `findstr` is useful in making sure you haven't missed anything. (See **Notes** below.)
- Now you can clean and build. This should hopefully show the "Welcome to Xamarin Forms" page.
- If there are problems, please see if there is a note in the **Notes** section that solves your problem. If not, please file an issue and/or fork, fix, and issue a PR!

## Future work
I hope to package this into a Visual Studio 2017 extension and publish it as a template on the Visual Studio Marketplace.

If I really wanted to be purist about using F#, I suppose I could swap out the Android and iOS projects with F# versions; however, I did not forsee changing much code in the device-specific projects in my Xamarin.Forms projects, so I kept them in their originally generated form.

Unfortunately, since UWP does not support Visual F#, we cannot swap out that project with a F# variant.

If you are interested in helping out with these changes, please feel free to fork and then send a Pull Request! It will be much appreciated!

## Notes/Troubleshooting
- When adding a new XAML file in the F# PCL project, make sure that the `.fsproj` file reads
```
<EmbeddedResource Include="Blah.xaml" />
```
rather than the VS 2017 generated default
```
<None Include="Blah.xaml" />
```
otherwise, your XAML will not compile properly. Also, make sure that the line for `MainPage.xaml` comes before `App.xaml` in the `.fsproj` file because I had problems getting it to build, since `App.xaml` does depend on elements in `MainPage.xaml`. If you are feeling fancier, we can also use a configuration more like the generated `.csproj` configuration as such:
```
  <ItemGroup>
    ...
    <Compile Include="Blah.xaml.fs" >
	  <DependentUpon>Blah.xaml</DependentUpon>
    </Compile>
	...
  </ItemGroup>
  <ItemGroup>
    ...
    <EmbeddedResource Include="Blah.xaml">
      <SubType>Designer</SubType>
      <Generator>MSBuild:UpdateDesignTimeXaml</Generator>
    </EmbeddedResource>
	...
  </ItemGroup>
```
where you can split out the `.xaml.fs` and the `.xaml` files into two different `<ItemGroup />`. In terms of ordering, at least in my experimentation, I found that ordering in the first group, with the `.xaml.fs` files, **does matter**, at least in the sense that all the pages that `App.xaml` depends on must come before it. In the second group, ordering does not matter, most likely since they will be compiled in order due to the `<DependentUpon />` tag in the first group.

- After re-naming your PCL, Visual Studio's IntelliSense might not be happy with the namespace name change and generate angry red scribbles all over your document. This is because previously built `.dll` files might have not been cleaned. You can manually go through and delete all `obj` and `bin` folders, or you can take the approach suggested in [this repo](https://github.com/moljac/Samples.Xamarin.Forms/blob/master/tutorial-samples/readme.md) and add an automated MSBuild cleaning action in the `fsproj` file:
```
  <Target Name="RemoveObjAndBin" AfterTargets="Clean">
    <RemoveDir Directories="$(BaseIntermediateOutputPath)" />
    <RemoveDir Directories="$(TargetDir)" />
  </Target>
  <Target Name="BeforeBuild">
    <!-- Remove obj folder -->
    <RemoveDir Directories="$(BaseIntermediateOutputPath)" />
    <!-- Remove bin folder -->
    <RemoveDir Directories="$(BaseOutputPath)" />
  </Target>	
```
It's also helpful to `findstr /S <old name> *` to absolutely make sure that all occurences of the old name have disappeared.
