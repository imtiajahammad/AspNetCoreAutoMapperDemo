using AutoMapper;

namespace AspNetCoreAutoMapperDemo;

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
