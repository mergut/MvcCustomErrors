# MvcCustomErrors

Display custom error pages using MVC views.

MvcCustomErrors unifies error page handling and centralizes the status code to displayed page mapping inside the IIS `<httpErrors>` section.


## Installation
WIP


## Usage

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
