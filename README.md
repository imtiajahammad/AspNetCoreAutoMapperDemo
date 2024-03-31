## A Step by Step Guide of using AutoMapper in ASP.NET Core

### What is AutoMapper?
AutoMapper is a convention-based object-oriented mapper that transforms a source object into a destination object.
- If both source and destination objects have the same properties with the same types, then AutoMapper can perform with almost zero configuration
- With complex/dissimilar objects, you can still use automapper's built-in type converters, transformers and resolvers to perform the mapping
- 


1. Create a new folder named **AspNetCoreAutoMapperDemo** and go to that directory  
    ```
    mkdir AspNetCoreAutoMapperDemo
    cd AspNetCoreAutoMapperDemo
    ```
2. Create a new solution named **AspNetCoreAutoMapperDemo**
    ```
    dotnet new sln -n AspNetCoreAutoMapperDemo
    ```
3. Add a new project into the solutin 
    ```
    dotnet new mvc -f net6.0 -n AspNetCoreAutoMapperDemo
    ```
4. Add the project to the solution
    ```
    dotnet sln add AspNetCoreAutoMapperDemo/AspNetCoreAutoMapperDemo.csproj
    ```
5. Open the project in vscode
    ```
    code .
    ```
6. Now add the required packages for automapper
    ```
    dotnet add package AutoMapper -v 12.0.1
    dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection -v 12.0.1
    ```
7. Configure AutoMapper into services
    ```
    builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly()); 
    ```
8. Create two classes that can be used for mapping. One for source object and the other for destination object
    ```
    dotnet new class -n Employee
    dotnet new class -n EmployeeModel
    ```
    ```
    public class Employee
    {
    public int Id { get; set; }
    public string Title { get; set; }
    public string Name { get; set; }
    public int? Age { get; set; }  
    public DateTime? RegistrationDate { get; set; }
    }
    ```
    ```
    public class EmployeeModel 
    {
    public int Id { get; set; }
    public string Title { get; set; }
    public string Name { get; set; }
    public int? Age { get; set; }
    public DateTime? RegistrationDate { get; set; }
    }
    ```
9. Now we will create AutoMapper Mapping Profiles. While doing the mapping from one type to another, AutoMapper looks for mapping profiles. Mapping profiles gives AutoMapper all information and configuration related to mappings. To create a mapping profile, we need to create a class that derives from the **Profile** class available in **AutoMapper** library
    ```
    dotnet new class -n MappingProfile
    ```
    ```
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeModel>();
        }
    }
    ```
10. AutoMapper has a service called **IMapper** that can be injected into any component or service to map objects from one type to another. Lets create **IEmployeeService** interface and **EmployeeService** class and inject **IMapper** service in the constructor.
    ```
    mkdir Services
    cd services
    dotnet new interface -n IEmployeeService
    dotnet new class -n EmployeeService
    ```
    ```
    public interface IEmployeeService
    {
    List<EmployeeModel> GetEmployees(); 
    }
    ```
    ```
    public class EmployeeService : IEmployeeService
    {
        private readonly IMapper _mapper;
 
        public EmployeeService(IMapper mapper)
        {
            _mapper = mapper;
        } 
 
        public List<EmployeeModel> GetEmployees()
        {
            var employees = new List<Employee>()
            {
                new Employee()
                {
                    Id = 1, 
                    Title = "Mr", 
                    Name = "Simon", 
                    Age = 32, 
                    RegistrationDate = new DateTime(2015, 12, 5)
                },
                new Employee()
                {
                    Id = 2, 
                    Name = "David", 
                    Age = 35, 
                    RegistrationDate = new DateTime(2013, 3, 15)                     
                },
                new Employee()
                {
                    Id = 3, 
                    Title = "Mr", 
                    Name = "Peter", 
                    Age = 29
                }
            };
            return _mapper.Map<List<EmployeeModel>>(employees);
        } 
    }
    ```
11. Inject the **IEmployeeService** implementation into the services
    ```
    builder.Services.AddScoped<IEmployeeService, EmployeeService>();
    ```
12. Inject the **IEmployeeService** into HomeController and return the **List<EmployeeModel>** from the **Index** action method to the rezor view
    ```
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmployeeService _employeeService;

        public HomeController(ILogger<HomeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        public IActionResult Index()
        {
            return View(_employeeService.GetEmployees());
        }
    }
    ```
13. Update the **Index.cshtml** razor view to represent the **List<Employee>** and run the application to see the list we are updating.
    ```
    @model IEnumerable<EmployeeModel>

    @{
        ViewData["Title"] = "Index";
        Layout = "~/Views/Shared/_Layout.cshtml";
    }

    <div class="row">
        <div class="col">
            <h1>Employees</h1>
        </div> 
    </div>
    <br />
    <table class="table">
        <thead>
            <tr>
                <th>@Html.DisplayNameFor(model => model.Id)</th>
                <th>@Html.DisplayNameFor(model => model.Title)</th>
                <th>@Html.DisplayNameFor(model => model.Name)</th>
                <th>@Html.DisplayNameFor(model => model.Age)</th>
                <th>@Html.DisplayNameFor(model => model.RegistrationDate)</th> 
            </tr>
        </thead>
        <tbody>
            @foreach (var item in Model)
            {
                <tr>
                    <td>@Html.DisplayFor(modelItem => item.Id)</td>
                    <td>@Html.DisplayFor(modelItem => item.Title)</td>
                    <td>@Html.DisplayFor(modelItem => item.Name)</td>
                    <td>@Html.DisplayFor(modelItem => item.Age)</td>
                    <td>@Html.DisplayFor(modelItem => item.RegistrationDate)</td> 
                </tr>
            }
        </tbody>
    </table>
    ```
14. For large projects, there could be hundreds of objects to map, and adding the **CreateMap** statement for every single mapping seems quite a non-productive and time-consuming task. We can avoid this type of mapping configurations using a C# interface and some reflection. The interface has a single method called **Mapping** and it has a default implementation that simply calls the same **CreateMap** method. Now create an interface **IMapFrom** 
    ```
    dotnet new interface -n IMapFrom
    ```
    ```
    public interface IMapFrom<T>
    {
    void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
    }
    ```
15. Now update the **EmployeeModel** class to implement **IMapFrom** interface. This tells AutoMapper that we want to map the **EmployeeModel** with the **Employee** class
    ```
    public class EmployeeModel : IMapFrom<Employee>
        {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public DateTime? RegistrationDate { get; set; }
        }
    ```
16. This makes the configuratin a lot simple. We can implement **IMapFrom<T>** interface on any class we want to map with the **T** class and AutoMapper will do the rest. Once we have all those classes ready to map, we do not want to register them all manually. So let's add some automation in our **MappingProfile** class we created earlier.
    ```
    public class MappingProfile : Profile
    {
        /*public MappingProfile()
        {
            CreateMap<Employee, EmployeeModel>();
        }*/
        public MappingProfile()
        {
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        }
    
        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                .ToList();
    
            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);
    
                var methodInfo = type.GetMethod("Mapping") ?? 
                                type.GetInterface("IMapFrom`1").GetMethod("Mapping");
    
                methodInfo?.Invoke(instance, new object[] { this });
            }
        }

    }
    ```
    We are using the **.NET Reflection** framework to scan the assembly and looking for all classes that are implementing the **IMapFrom** interface. Once we have all those types available, we are simply invoking their Mapping method. 
    Now run the project and check out the Index page is working or not with this configuration update.
    17. To define custom mappings, you need to implement the **Mapping** method available in the **IMapFrom** interface we created above. Suppose we do not want to map the **RegistrationDate** Property of the **Employee** class, we can use **ForMember** method that allows us to customize individual members of the class. We need to specify the property we want to customize and then we can call the ignore method. The ignore method skips the mapping of the current property  
    ```
    public class EmployeeModel : IMapFrom<Employee>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public DateTime? RegistrationDate { get; set; }
        
        public void Mapping(Profile profile)
        {
            var c = profile.CreateMap<Employee, EmployeeModel>().ForMember(d => d.RegistrationDate, opt => opt.Ignore());
        }
    }
    ```
    Run the project and we will notice that the data of **RegistrationDate** property is not mapped from **Employee** object to **EmployeeModel** object

18. The title property of employee David is not specified and this is why Title property column is empty for David. We can tell AutoMapper to replace the null value with any custom value. 
    ```
    public class EmployeeModel : IMapFrom<Employee>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public DateTime? RegistrationDate { get; set; }
        
        public void Mapping(Profile profile)
        {
            var c = profile.CreateMap<Employee, EmployeeModel>()
                .ForMember(d => d.RegistrationDate, opt => opt.Ignore())
                .ForMember(d => d.Title, opt => opt.NullSubstitute("N/A"));
        }
    }
    ```
    Now run the project and see that the null value for Title is replaced with N/A.  

19. We can also map dissimilar objects using the AutoMapper **MapFrom** method. Lets add a new property **OfficeAddress** in the Employee class and another new property **WorkAddress** in EmployeeModel class. Update the Employee, EmployeeModel, EmployeeService, Index as following-

    ```
    public class Employee
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }  
        public DateTime? RegistrationDate { get; set; }
        public string OfficeAddress { get; set; }
    }
    ```
    ```
    public class EmployeeModel : IMapFrom<Employee>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string WorkAddress { get; set; }
        
        public void Mapping(Profile profile)
        {
            var c = profile.CreateMap<Employee, EmployeeModel>()
                .ForMember(d => d.RegistrationDate, opt => opt.Ignore())
                .ForMember(d => d.Title, opt => opt.NullSubstitute("N/A"))
                .ForMember(d => d.WorkAddress, opt => opt.MapFrom(s => s.OfficeAddress));
        }
    }
    ```
    ```
    public class EmployeeService : IEmployeeService
    {
        private readonly IMapper _mapper;
    
        public EmployeeService(IMapper mapper)
        {
            _mapper = mapper;
        } 
    
        public List<EmployeeModel> GetEmployees()
        {
            var employees = new List<Employee>()
            {
                new Employee()
                {
                    Id = 1, 
                    Title = "Mr", 
                    Name = "Simon", 
                    Age = 32, 
                    RegistrationDate = new DateTime(2015, 12, 5)
                },
                new Employee()
                {
                    Id = 2, 
                    Name = "David", 
                    Age = 35, 
                    RegistrationDate = new DateTime(2013, 3, 15),
                    OfficeAddress = "123 ABC Street"                     
                },
                new Employee()
                {
                    Id = 3, 
                    Title = "Mr", 
                    Name = "Peter", 
                    Age = 29
                }
            };
    
            return _mapper.Map<List<EmployeeModel>>(employees);
        } 
    }
    ```
    ```
    @model IEnumerable<EmployeeModel>

    @{
        ViewData["Title"] = "Index";
        Layout = "~/Views/Shared/_Layout.cshtml";
    }

    <div class="row">
        <div class="col">
            <h1>Employees</h1>
        </div> 
    </div>
    <br />
    <table class="table">
        <thead>
            <tr>
                <th>@Html.DisplayNameFor(model => model.Id)</th>
                <th>@Html.DisplayNameFor(model => model.Title)</th>
                <th>@Html.DisplayNameFor(model => model.Name)</th>
                <th>@Html.DisplayNameFor(model => model.Age)</th>
                <th>@Html.DisplayNameFor(model => model.RegistrationDate)</th> 
                <th>@Html.DisplayNameFor(model => model.WorkAddress)</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var item in Model)
            {
                <tr>
                    <td>@Html.DisplayFor(modelItem => item.Id)</td>
                    <td>@Html.DisplayFor(modelItem => item.Title)</td>
                    <td>@Html.DisplayFor(modelItem => item.Name)</td>
                    <td>@Html.DisplayFor(modelItem => item.Age)</td>
                    <td>@Html.DisplayFor(modelItem => item.RegistrationDate)</td> 
                    <td>@Html.DisplayFor(modelItem => item.WorkAddress)</td>
                </tr>
            }
        </tbody>
    </table>
    ```
    Now run the project and see the update for **WorkAddress** property.


#### Reference: https://www.ezzylearning.net/tutorial/a-step-by-step-guide-of-using-automapper-in-asp-net-core