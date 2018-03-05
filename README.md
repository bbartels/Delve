# Delve

Delve is a simple framework for ASP.NET Core that adds easy pagination, filtering, sorting, selecting and expanding to an MVC project without being tightly coupled to an ORM.  

Core:  
[![Delve](https://img.shields.io/nuget/vpre/Delve.svg)](https://www.nuget.org/packages/Delve/0.9.6-alpha)  
AspNetCore Integration:  
[![Delve.AspNetCore](https://img.shields.io/nuget/vpre/Delve.AspNetCore.svg)](https://www.nuget.org/packages/Delve.AspNetCore/0.9.6-alpha)

## Features  
* Pagination, Sorting, Filtering, Selecting and Expanding (i.e. EFCore Include) capabilities.
* Easy ASP.Net Core Mvc integration
* Auto-Generation of X-Pagination header
* Optional default definitions for filtering, sorting and selecting
* Automatically returns a 400 Bad Request on malformed query
* Virtual Properties allow for extending API functionality without cluttering domain classes
* Not ORM dependant (Any ORM that uses IQueryable should work)
* Async pagination is supported

## Demo  
There is a Demo Project in the Demo/ directory, which demonstrates all of Delve's functionality.

## Usage

### 1. Add Delve to the MVC Project
Just append **.AddDelve()** to your **.AddMvc()** call in your **Startup.cs** file.
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc().AddDelve();
}
```

If you wish to change the default configuration of **Delve** you can add **DelveOptions** as a parameter to **.AddDelve()**.

```csharp
services.AddMvc().AddDelve(options => options.MaxPageSize = 15);
```

### 2. Add a QueryValidator to your Domain class
By deriving from **AbstractQueryValidator\<TDomain>** you can precisely define what is allowed to by queried by the user.  
By adding virtual properties you do not have to create a new property for something you only want to expose to the API (See examples below).

```csharp
using Delve.Validation;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    //"UserRoles" not shown here for brevity, check the demo project.
    public IEnumerable<UserRoles> { get; set; }
}

public class UserQueryValidator : AbstractQueryValidator<User>
{
    public UserQueryValidator()
    {
        //Adds Id and virtual property "Name" as default selects
        AddDefaultSelect("Id", u => u.Id);
        AddDefaultSelect("Name", u => u.LastName + " " + u.FirstName);
        
        //Adds virtual property as default sort
        AddDefaultSort(u => u.LastName + " " + u.LastName, true);
        //Adds default filter
        AddDefaultFilter(u => u.Id > 5);
        
        //Adds selecting/filtering/sorting with key="Id" for Id property
        CanSelect("Id", x => x.Id);
        CanFilter("Id", x => x.Id);
        CanOrder("Id", x => x.Id);
        
        //Adds a virtual property for the Age of the user calculated using the DateOfBirth
        //By using AllowAll() you can automatically add selecting/filtering/sorting for a property
        AllowAll("Age", x => Math.Round((DateTime.Now - x.DateOfBirth).TotalDays / 365, 2));
        
        //Adds a virtual property with key="Name" for the combination of LastName and FirstName
        AllowAll("Name", x => x.LastName + "" + x.FirstName);
        
        //Allows you to use "Include" in ORM's like EFCore
        //Expanding syntax is EF6 like. Branch into an IEnumerable navigation-property with a .Select().
        //All previous navigation-properties are automatically included. (in this case: both UserRoles and UserRoles.Role)
        //No need to do another CanExpand for the standalone UserRoles
        CanExpand(x => x.UserRoles.Select(ur => ur.Role);
    }
}
```

Now you just have to register the validator in your services:
```csharp
services.AddTransient<IQueryValidator<User>, UserQueryValidator>();
```

### 3. Configure your Controller
By simply adding a **IResourceParameter\<TDomain>** to the method signature Delve will automatically parse and validate the client request and upon an invalid request return a 400 BadRequest with a matching error message.

```csharp
using Delve.Models;
using Delve.AspNetCore;

public class UserController : Controller
{
    private readonly IUrlHelper _urlHelper;
    public UserController(IUrlHelper urlHelper)
    {
        _urlHelper = urlHelper;
    }
    
    public async Task<IActionResult> GetUsers(IResourceParameter<User> parameter)
    {
        var collection = Context.Set<User>()
                //Applies expands to the IQueryable<User>. Since delve isn't directly coupled to EFCore
                //you need to pass in the include method as a delegate.
                .ApplyIncludes((q, i) => q.Include(i), parameters)
                //Applies filters to the IQueryable<User>
                .ApplyFilters(parameters)
                //Applies sorts to the IQueryable<User>
                .ApplyOrderBy(parameters);
                
        //ToPagedResultAsync() will pull the matching IQueryable data from the database and applies pagination.
        //Since System.Linq doesn't provide a way to asynchronously Count() and ToList()
        //you will need to pass in the matching methods of your ORM for the pagination to work.
        //If you dont need async capabilities you can use ToPagedResult(). It will work without any delegates.
        var users =  await collection.ToPagedResultAsync(
        
                        async q => await q.CountAsync(), 
                        async q => await q.ToListAsync(), parameters);
                        
        //Adds paginationheader to the response
        this.AddPaginationHelper(parameter, users, _urlHelper);
        
        return Ok(users.ShapeData(parameter));
}
```

### 4. Send a request

### Filters: 
There are a couple of ways you can work with Delve's filters.  
By adding **filter=** to the query string you can filter on any in the QueryValidator defined virtual properties.  
You can add mulitple filters by separating these with commas (i.e. **filter=Id== 5, Name==John**).  
Furthermore you can define logical OR behaviour by separating the values of one filter with a '|' operator.  
This way you can check for a user called "John" or "Mary" (i.e. **filter=Name==John|Mary**).  

### FilterOperators

| Operator |        Interpretation       | Allowed Type      
|----------|-----------------------------|--------------
|   `==`   | Equal                       | object
|   `==*`  | CaseInsensitive Equal       | string
|   `!=`   | NotEqual                    | object
|   `!=*`  | CaseInsensitive NotEqual    | string
|   `>`    | GreaterThan                 | IComparable
|   `<`    | LessThan                    | IComparable
|   `>=`   | GreaterThanOrEqual          | IComparable
|   `<=`   | LessThanOrEqual             | IComparable
|   `?`    | Contains                    | string
|   `?*`   | CaseInsensitive Contains    | string
|   `^`    | StartsWith                  | string
|   `^*`   | CaseInsensitive StartsWith  | string
|   `$`    | EndsWith                    | string
|   `$*`   | CaseInsensitive EndsWith    | string

### Sorting

Just as Filtering, Sorting is delimited by commas, though unlike with Filtering order of the sorts matter.  
Meaning, if you have **"(orderby=Name, -Age)"** in your query, it will first order by Name ascending and then by Age descending **(Linq equivalent: .OrderBy(x => x.Name).ThenByDescending(x => x.Age);)**.

### Selecting

If your entity has numerous columns and as a consumer of the API you are only really interested in a couple of things you can save bandwith by using Select.  
Just like before selects are delimited by commas and allow you to select on any virtual property defined in the **QueryValidator**.

### Expanding

If your Entity references another relation you can include that relation using the **expand** keyword.  
This will allow you to perform further operations on navigation properties.  

Example for the query validation definition:  
```csharp
CanExpand(x => x.UserRoles.Select(ur => ur.Role);
```  
This will add a way to allow to include both **UserRoles** and **UserRoles.Role** in your query.
Note: CanExpand does not require a key, it will automatically take the name of the properties as the individual keys and separate each layer of depth by a '.'.

##### Request Example:  
```
GET /api/Users
?filter=        Age>=20, Name$*Smith|Bullock
&orderby=       -Age, Name
&select=        Id, Name, Age
&expand=        UserRoles
&pageNumber=    1
&pageSize=      10
```

##### X-Pagination Response Header:  
```
{   
    "currentPage":1,
    "pageSize":5,
    "totalPages":4,
    "totalCount":20,
    "previousPageLink":null,
    "nextPageLink": "{address}/api/Users?pageNumber=1
                                        &pageSize=10
                                        &filter=Age>=20,Name$*Smith|Bullock
                                        &orderby=-Age,Name
                                        &select=Id,Name,Age
                                        &expand=UserRoles"
}
```

## Licensing
Delve is licensed under MIT.

## Contribution
This project is still at an early stage in development so any contributions are welcomed!
Even if it is just a suggestions/discussions about how to improve upon Delve!
