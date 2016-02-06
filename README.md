# MvcCustomErrors

[![NuGet Version](https://img.shields.io/nuget/v/MvcCustomErrors.svg)](https://www.nuget.org/packages/MvcCustomErrors)
[![NuGet Downloads](https://img.shields.io/nuget/dt/MvcCustomErrors.svg)](https://www.nuget.org/packages/MvcCustomErrors)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/mergut/MvcCustomErrors/master/LICENSE)
[![Build status](https://ci.appveyor.com/api/projects/status/0n4p2yapfmd7saf9/branch/master?svg=true)](https://ci.appveyor.com/project/mergut/mvccustomerrors/branch/master)

Display custom error pages using MVC views.

MvcCustomErrors unifies error page handling and centralizes the status code to displayed page mapping inside the IIS `<httpErrors>` section.


## Installation
MvcCustomErrors is available as a NuGet package. You can install it using the NuGet Package Manager Console window:
```
PM> Install-Package MvcCustomErrors
```

## Customization

This package provides views for InternalServerError (500) and NotFound (404) status codes.

You can easily add other views for specific status codes.

Add a new method inside `ErrorController`:
```csharp
public ActionResult Forbidden()
{
    this.Response.StatusCode = 403;
    
    return View();
}
```

Add a new view `~/Views/Error/Forbidden.cshtml`:
```html
@{
    ViewBag.Title = "Forbidden";
}
<h2>403 - Forbidden</h2>
<p>You do not have permission to view this resource using the credentials that you supplied.</p>
```

Add mapping inside `web.config`:
```xml
<system.webServer>
  <httpErrors errorMode="Custom" existingResponse="Replace">
    <remove statusCode="403" />
    <error statusCode="403" path="/Error/Forbidden" responseMode="ExecuteURL" />
  </httpErrors>
</system.webServer>
```


## Implementation

A generic error page (Error.aspx) executes when an exception occurs inside the application.
This page only sets the status code according to the occurred exception and suppresses any output.
Then IIS `httpErrors` section takes over and displays the page corresponding to the status code.
