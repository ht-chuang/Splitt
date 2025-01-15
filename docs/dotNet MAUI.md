### Overview
Provides a series of platform-specific frameworks
- .Net for Android implemented by Mono
- .Net for iOS
- .Net for Mac Catalyst
- Windows UI 3 (WinUI 3)
All use the same .NET Base Class Library (BCL)

High level architecture:
  ![[MAUI architecture.png]]

(1) Code interacts with the .NET MAUI controls and API layer
(2) App code may directly exercise platform APIs, if required
(3) The Device Abstraction Layer directly consumes the native platform APIs

Android:
- compiled from C# into an intermediate language (IL), then just-in-time (JIT) compiled to a native assembly
iOS:
- fully ahead-of-time (AOT) compiled from C# into native ARM assembly code

### Program Files
`MauiProgram.cs`
- bootstraps the app
- serves as the cross-platform entry point of the app
- template startup code points to ‘App’ class in App.xaml

`App.xaml` + `App.xaml.cs`
- always 2 files with any XAML file
- `.xaml` file
	- contains XAML markup
	- contains app-wide XAML resources like colors, styles, or templates
- `.xaml.cs`
	- code file that is a child item of corresponding .xaml file
	- in the Solution Explorer
	- contains code created by user to interact with XAML markup
	- instantiates the Shell application
	- points to AppShell

`AppShell.xaml` + `AppShell.xaml.cs`
- defines the AppShell class
- defines visual hierarchy of the app

`MainPage.xaml` + `MainPage.xaml.cs`
- Startup page displayed by the app
- `MainPage.xaml`
	- defines the UI of the page
- `MainPage.xaml.cs`
	- contains code behind for the XAML, like code for a button click event

### XAML parts
#### Content Page
`<ContentPage>`
- root object for the xaml class
- can only have one child object
`<VerticalStackLayout>`
- can have multiple children objects
- arranges its children vertically, one after another
`<HorizontalStackLayout>`
- operates similar to `<VerticalStackLayout>`
- children are arranged horizontally
`<Image>`
- displays an image
`<Label>`
- displays text
`<Button>`
- can be pressed by the user
- raises the `Clicked` event
`Clicked="LearnMore_Clicked"`
- The `Clicked` event of the button is assigned to the `LearnMore_Clicked` event handler
- defined in the code-behind file
`<Editor>`
- multi-line text editor control
`<Grid>`
- layout control
- defines columns and rows to create cells and child controls are placed within those cells
- by default, single row and column (one cell)
- indexed starting at 0
- `ColumnDefinitions="*,*` defines two columns, using as much space as possible, which even distributes the columns

#### App Shell Page
`<Shell>`
- root object
`<TabBar>`
- content of the Shell
- doesn't represent UI elements
- organization of app's visual hiearchy
`<ShellContent>`
- points to a page to display
- set by `ContentTemplate`
- `Icon` property uses syntax to specify property value for each platform
