# MvcCustomErrors

[![NuGet Version](https://badge.fury.io/nu/MvcCustomErrors.svg)](https://www.nuget.org/packages/MvcCustomErrors)
[![NuGet Downloads](https://img.shields.io/nuget/dt/MvcCustomErrors.svg)](https://www.nuget.org/packages/MvcCustomErrors)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/mergut/MvcCustomErrors/master/LICENSE)
[![Build Status](https://ci.appveyor.com/api/projects/status/0n4p2yapfmd7saf9/branch/master?svg=true)](https://ci.appveyor.com/project/mergut/mvccustomerrors/branch/master)
[![Coverage Status](https://coveralls.io/repos/github/mergut/MvcCustomErrors/badge.svg?branch=master)](https://coveralls.io/github/mergut/MvcCustomErrors?branch=master)

Display custom error pages using MVC views.

MvcCustomErrors allows displaying error pages with MVC views using existing MVC layouts without having to duplicate layout and styling into `.aspx` or `.html` files.
Mapping IIS errors from `<httpErrors>` section is also supported.


## Installation
MvcCustomErrors is available as a NuGet package. You can install it using the NuGet Package Manager Console window:
```
PM> Install-Package MvcCustomErrors
```

*The default globally registered `HandleErrorAttribute` should be removed unless you want to apply a diffrent display to errors occuring inside a controller action.*

## Usage
Out of the box 404, 500 and fallback views are provided.

Adding pages for other error numbers is as simple as creating a new view file in `~/Views/Error/` folder in the form of `Http###` where `###` is the HTTP code.

Example; for a 403 error, add a new view `~/Views/Error/Http403.cshtml`:
```html
@{
    ViewBag.Title = "Forbidden";
}
<h2>403 - Forbidden</h2>
<p>You do not have permission to view this resource using the credentials that you supplied.</p>
```


## Customization

In order to customize the controller action executing when an error occurs, create an action method in `ErrorController` matching the view name.

Example; to customize the 500 error, create a new method in `ErrorController`:
```csharp
public ActionResult Http500()
{
    this.Response.Clear();
    this.Response.StatusCode = 500;
    this.Response.TrySkipIisCustomErrors = true;
    
    // Custom processing, view model, ...

    return View();
}
```
