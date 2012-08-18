UnitTestingXSD
==============

Code for blog post: 

Requirements:
 - Visual Studio 2010 or 2012
 - NuGet Package Manager
 - xUnit test runner

If you don't have NuGet Package Manager:
1) Open Tools -> Extension Manager
2) Select Online Gallery
3) Type NuGet in Search window
4) Select NuGet "Package Manager" and click "Download"

If you don't have xUnit test runner
- For Visual Studio 2010 
  - get one here: https://github.com/quetzalcoatl/xvsr10/downloads
  - follow Installation instruction from Readme: https://github.com/quetzalcoatl/xvsr10
  
- For Visual Studio 2012
   - get one here: http://visualstudiogallery.msdn.microsoft.com/463c5987-f82b-46c8-a97e-b1cde42b9099
   - followin installation instructions

Compilation:
- Add xUnit 
  - right click your project and select "Manage NuGet Packages"
  - type "xUnit" in the Search window
  - select "xUnit.Net" 
  - click "Install"
- Compile 
  - Select Build -> Build Solution from Menu
  
Running tests:
- Visual Studio 2010 
  - Right click your project and select "Enable Test Extensions"
  - Ctrl + R, A to run tests

- Visual Studio 2012
  - Ctrl + R, A to run tests

