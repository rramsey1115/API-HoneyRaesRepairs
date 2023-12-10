using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using HoneyRaesAPI.Models;
using HoneyRaesAPI.Models.DTOs;

// -------Lists to hold our data---------------------------------
List<Customer> customers = new List<Customer> {
    new() { Id = 1, Name = "Alice", Address = "123 Main St" },
    new() { Id = 2, Name = "Bob", Address = "456 Oak St" },
    new() { Id = 3, Name = "Charlie", Address = "789 Pine St" },
    new() { Id = 4, Name = "David", Address = "101 Elm St" },
    new() { Id = 5, Name = "Eva", Address = "202 Maple St" },
    new() { Id = 6, Name = "Frank", Address = "303 Cedar St" },
    new() { Id = 7, Name = "Grace", Address = "404 Birch St" },
    new() { Id = 8, Name = "Henry", Address = "505 Walnut St" },
    new() { Id = 9, Name = "Ivy", Address = "606 Spruce St" },
    new() { Id = 10, Name = "Jack", Address = "707 Fir St" }
};

List<Employee> employees = new List<Employee> {
    new() { Id = 1, Name = "Sue", Specialty = "Apple Products" },
    new() { Id = 2, Name = "Wes", Specialty = "PC/Android Products" },
    new() { Id = 3, Name = "Gina", Specialty = "Laptop Repairs" },
    new() { Id = 4, Name = "Tom", Specialty = "Laptop Repairs" },
    new() { Id = 5, Name = "Linda", Specialty = "Apple Products" }
};

List<ServiceTicket> serviceTickets = new List<ServiceTicket> {
    // Tickets for Customer 1
    new ServiceTicket { Id = 1, CustomerId = 1, EmployeeId = 1, Description = "MacBook won't start", Emergency = true, DateCompleted = new DateTime(2020, 12, 1, 8, 22, 30) },
    new ServiceTicket { Id = 2, CustomerId = 1, EmployeeId = 2, Description = "Broken iPhone screen", Emergency = true, DateCompleted = null },
    new ServiceTicket { Id = 3, CustomerId = 1, EmployeeId = null, Description = "Wi-Fi connectivity problems", Emergency = false, DateCompleted = null },

    // Tickets for Customer 2
    new ServiceTicket { Id = 4, CustomerId = 2, EmployeeId = 1, Description = "iMac slow performance", Emergency = false, DateCompleted = new DateTime(2023, 12, 5, 11, 43, 21) },
    new ServiceTicket { Id = 5, CustomerId = 2, EmployeeId = 4, Description = "Dell laptop keyboard issues", Emergency = true, DateCompleted = null },

    // Tickets for Customer 3
    new ServiceTicket { Id = 6, CustomerId = 3, EmployeeId = 5, Description = "Windows laptop won't boot", Emergency = true, DateCompleted = new DateTime(2023, 12, 5, 14, 09, 09) },
    new ServiceTicket { Id = 7, CustomerId = 3, EmployeeId = null, Description = "Printer not printing", Emergency = false, DateCompleted = null },

    // Tickets for Customer 4
    new ServiceTicket { Id = 8, CustomerId = 4, EmployeeId = 4, Description = "Network connection lost", Emergency = false, DateCompleted = null },
    new ServiceTicket { Id = 9, CustomerId = 4, EmployeeId = 3, Description = "Laptop overheating", Emergency = false, DateCompleted = new DateTime(2023, 12, 2, 13, 11, 16) },

    // Tickets for Customer 5
    new ServiceTicket { Id = 10, CustomerId = 5, EmployeeId = null, Description = "Android phone freezing", Emergency = true, DateCompleted = null },
    new ServiceTicket { Id = 11, CustomerId = 5, EmployeeId = 1, Description = "Printer paper jam", Emergency = true, DateCompleted = null },

    // Tickets for Customer 6
    new ServiceTicket { Id = 12, CustomerId = 6, EmployeeId = 3, Description = "MacBook battery replacement", Emergency = false, DateCompleted = null },
    new ServiceTicket { Id = 13, CustomerId = 6, EmployeeId = 5, Description = "PC won't turn on", Emergency = true, DateCompleted = new DateTime(2023, 2, 3, 16, 16, 45) },

    // Tickets for Customer 7 -- should return as my inactive customer since all tickets are closed and dates are over a year old
    new ServiceTicket { Id = 14, CustomerId = 7, EmployeeId = 2, Description = "iPhone not charging", Emergency = true, DateCompleted = new DateTime(2022, 2, 4, 14, 34, 2) },
    new ServiceTicket { Id = 15, CustomerId = 7, EmployeeId = 4, Description = "Network printer setup", Emergency = false, DateCompleted = new DateTime(2022, 3, 3, 14, 14, 44) }
};

var builder = WebApplication.CreateBuilder(args);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//  GET all service tickets---------------------------------------------------------
app.MapGet("/servicetickets", () =>
{
    return serviceTickets.Select(t =>
        new ServiceTicketDTO
        {
            Id = t.Id,
            CustomerId = t.CustomerId,
            EmployeeId = t.EmployeeId,
            Description = t.Description,
            Emergency = t.Emergency,
            DateCompleted = t.DateCompleted
        }
    );
});

// GET service ticket by Id----------------------------------------------------------
app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);

    if (serviceTicket == null)
    {
        return Results.NotFound();
    }

    Employee employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    Customer customer = customers.FirstOrDefault(c => c.Id == serviceTicket.CustomerId);

    return Results.Ok(new ServiceTicketDTO
    {
        Id = serviceTicket.Id,
        CustomerId = serviceTicket.CustomerId,
        Customer = customer == null ? null : new CustomerDTO
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address
        },
        EmployeeId = serviceTicket.EmployeeId,
        Employee = employee == null ? null : new EmployeeDTO
        {
            Id = employee.Id,
            Name = employee.Name,
            Specialty = employee.Specialty
        },
        Description = serviceTicket.Description,
        Emergency = serviceTicket.Emergency,
        DateCompleted = serviceTicket.DateCompleted
    });
});

// GET all employees ----------------------------------------------------------------
app.MapGet("/employees", () =>
{
    return employees.Select(e => new EmployeeDTO
    {
        Id = e.Id,
        Name = e.Name,
        Specialty = e.Specialty
    });
});

// GET employees by Id---------------------------------------------------------------
app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    List<ServiceTicket> tickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
    return Results.Ok(new EmployeeDTO
    {
        Id = employee.Id,
        Name = employee.Name,
        Specialty = employee.Specialty,
        ServiceTickets = tickets.Select(t => new ServiceTicketDTO
        {
            Id = t.Id,
            CustomerId = t.CustomerId,
            EmployeeId = t.EmployeeId,
            Description = t.Description,
            Emergency = t.Emergency,
            DateCompleted = t.DateCompleted
        }).ToList()
    });
});

// GET all customers ----------------------------------------------------------------
app.MapGet("/customers", () =>
{
    return customers.Select(c => new CustomerDTO
    {
        Id = c.Id,
        Name = c.Name,
        Address = c.Address
    });
});

// GET customers by id---------------------------------------------------------------
app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(new CustomerDTO
    {
        Id = customer.Id,
        Name = customer.Name,
        Address = customer.Address
    });
});

// POST a service ticket--------------------------------------------------------------
app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{
    // creates a new id (SQL will do this for us like JSON Server did!)
    serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;

    // Get the customer data to check that the customerid for the service ticket is valid
    Customer customer = customers.FirstOrDefault(c => c.Id == serviceTicket.CustomerId);

    // if the client did not provide a valid customer id, this is a bad request
    if (customer == null)
    {
        return Results.BadRequest();
    }

    serviceTickets.Add(serviceTicket);

    // Created returns a 201 status code with a link in the headers to where the new resource can be accessed
    return Results.Created($"/servicetickets/{serviceTicket.Id}", new ServiceTicketDTO
    {
        Id = serviceTicket.Id,
        CustomerId = serviceTicket.CustomerId,
        Customer = new CustomerDTO
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address
        },
        Description = serviceTicket.Description,
        Emergency = serviceTicket.Emergency
    });

});

// DELETE a service ticket by Id------------------------------------------------------
app.MapDelete("servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    serviceTickets.Remove(serviceTicket);
});

// UPDATE/PUT service ticket by id----------------------------------------------------
app.MapPut("/servicetickets/{id}", (int id, ServiceTicket serviceTicket) =>
{
    ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);

    if (ticketToUpdate == null)
    {
        return Results.NotFound();
    }
    if (id != serviceTicket.Id)
    {
        return Results.BadRequest();
    }

    ticketToUpdate.CustomerId = serviceTicket.CustomerId;
    ticketToUpdate.EmployeeId = serviceTicket.EmployeeId;
    ticketToUpdate.Description = serviceTicket.Description;
    ticketToUpdate.Emergency = serviceTicket.Emergency;
    ticketToUpdate.DateCompleted = serviceTicket.DateCompleted;

    return Results.NoContent();
});

// UPDATE/POST completed ticket by id-------------------------------------------------
app.MapPost("/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(st => st.Id == id);
    ticketToComplete.DateCompleted = DateTime.Today;
});

// ************************************************************************************
//                                 EXPLORER CHAPTERS
// ************************************************************************************

// GET all service tickets which are emergencies AND incomplete------------------------
app.MapGet("servicetickets/emergency", () =>
{
    // find tickets emergency true and datecompleted null
    List<ServiceTicket> filteredResults = serviceTickets.Where(st => st.Emergency == true && st.DateCompleted == null).ToList();
    if (filteredResults.Count == 0) { return Results.NotFound(); }

    // iterate filteredResults and return new list of ServiceTicketDTOs
    return Results.Ok(filteredResults.Select(fr =>
    {
        // find associated customer
        Customer foundCustomer = customers.FirstOrDefault(c => c.Id == fr.CustomerId);

        return new ServiceTicketDTO
        {
            Id = fr.Id,
            CustomerId = fr.CustomerId,
            Customer = new CustomerDTO
            {
                Id = foundCustomer.Id,
                Name = foundCustomer.Name,
                Address = foundCustomer.Address
            },
            EmployeeId = fr.EmployeeId,
            Description = fr.Description,
            Emergency = fr.Emergency
        };
    }));
});

// GET all service tickets currently unassigned----------------------------------------
app.MapGet("servicetickets/available", () =>
{
    List<ServiceTicket> availableTix = serviceTickets.Where(st => st.EmployeeId == null).ToList();
    if (availableTix == null) { return Results.NoContent(); }

    // iterate filteredResults and return new list of ServiceTicketDTOs
    return Results.Ok(availableTix.Select(at =>
    {
        // find associated customer
        Customer foundCustomer = customers.FirstOrDefault(c => c.Id == at.CustomerId);

        return new ServiceTicketDTO
        {
            Id = at.Id,
            CustomerId = at.CustomerId,
            Customer = new CustomerDTO
            {
                Id = foundCustomer.Id,
                Name = foundCustomer.Name,
                Address = foundCustomer.Address
            },
            EmployeeId = at.EmployeeId,
            Description = at.Description,
            Emergency = at.Emergency
        };
    }));


});

// GET all inactive customers----------------------------------------------------------
// all customers who haven't had a service ticket closed for them in over a year
app.MapGet("customers/inactive", () =>
{

    // collection to store results 
    List<Customer> inactiveCustomers = new List<Customer>();

    // define timeSpan 
    DateTime oneYearAgo = DateTime.Now - TimeSpan.FromDays(365);

    // iterate all customers
    foreach (Customer c in customers)
    {
        //find tickets for that customer which HAVE BEEN COMPLETED
        List<ServiceTicket> customerTickets = serviceTickets.Where(st => st.CustomerId == c.Id && st.DateCompleted != null).ToList();
        List<ServiceTicket> customerOpenTickets = serviceTickets.Where(st => st.CustomerId == c.Id && st.DateCompleted == null).ToList();
        int count = 0; //holds how many tickets match criteria for inactivity for each customer

        if (customerOpenTickets.Count == 0) //if they DO have open tickets, we don't want then to be inactive, so do nothing
        {
            // iterate customerTickets - find if dateCompleted is over 1 year ago
            foreach (ServiceTicket ct in customerTickets)
            {
                // if timeSinceClosed more than a year ago, add a counter for each true instance
                if (ct.DateCompleted < oneYearAgo)
                {
                    count++;
                }
            }
            if (count > 0) //count will be zero if no tickets meet 1 year old criteria
            {
                // this customer (c) has at least one ticket which meets all criteria, but will only be added once, even if multiple tickets meet criteria
                inactiveCustomers.Add(c);
            }
        }
    }

    // could have eliminated need for counter and removed duplicates from inactiveCustomers here
    // iterate inactiveCustomers and return CustomerDTO
    return inactiveCustomers.Select(inactiveC =>
        new CustomerDTO
        {
            Id = inactiveC.Id,
            Name = inactiveC.Name,
            Address = inactiveC.Address
        });
});

// Get all available employees---------------------------------------------------------
// employees NOT currently assigned to an incomplete service ticket
app.MapGet("employees/available", () =>
{
    // container list for results
    List<Employee> availableEmployees = new List<Employee>(employees);

    // find all assigned service tickets which are still incomplete (assignedOpenTickets)
    List<ServiceTicket> assignedOpenTickets = serviceTickets.Where(t => t.EmployeeId != null && t.DateCompleted == null).ToList();

    // find all employees with assignedOpenTickets
    List<Employee> employeesWithAssignedTickets = new List<Employee>();

    foreach (ServiceTicket openTicket in assignedOpenTickets)
    {
        foreach (Employee employee in employees)
        {
            if (openTicket.EmployeeId == employee.Id)
            {
                // remove employee from available list of all employees
                availableEmployees.Remove(employee);
            }
        }
    }

    // return updated list as result
    return availableEmployees.Select(ae =>
    {
        List<ServiceTicket> ticketsForAe = serviceTickets.Where(st => st.EmployeeId == ae.Id).ToList();

        return new EmployeeDTO
        {
            Id = ae.Id,
            Name = ae.Name,
            Specialty = ae.Specialty,
            ServiceTickets = ticketsForAe.Select(tae =>
            new ServiceTicketDTO
            {
                Id = tae.Id,
                CustomerId = tae.CustomerId,
                EmployeeId = tae.EmployeeId,
                Description = tae.Description,
                Emergency = tae.Emergency,
                DateCompleted = tae.DateCompleted
            }).ToList()
        };
    });
});

// GET employee's customers-------------------------------------------------------------
// all customers for whom a given employee has been assigned to a service ticket
// (whether completed or not)
app.MapGet("employees/{id}/customers", (int id) =>
{
    // find serviceTickets for employee based on id (foundServiceTickets)
    List<ServiceTicket> foundServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();

    // list all customers connected to foundServiceTickets
    // matchingCustomers
    List<Customer> matchingCustomers = new List<Customer>();
    foreach (ServiceTicket foundST in foundServiceTickets)
    {
        foreach (Customer customer in customers)
        {
            if (foundST.CustomerId == customer.Id)
            {
                matchingCustomers.Add(customer);
            }
        }
    }

    // eliminate duplicate customers in matchingCustomers
    List<Customer> uniqueCustomers = matchingCustomers.Distinct().ToList();

    if (uniqueCustomers.Count == 0)
    {
        return Results.NoContent();
    }

    // return CustomerDTO for each entry in uniqueCustomers
    var result = uniqueCustomers.Select(uc => new CustomerDTO
    {
        Id = uc.Id,
        Name = uc.Name,
        Address = uc.Address
    });

    return Results.Ok(result);

});

// Get employee of the month------------------------------------------------------------
// return single employee who has completed most service tickets in the last month
app.MapGet("employees/month/{monthInt}", (int monthInt) =>
{
    // find all completed service tickets 
    List<ServiceTicket> completedTix = serviceTickets.Where(ticket => ticket.DateCompleted != null).ToList();

    // find all service tickets from past month (monthlyCompletedTickets)
    List<ServiceTicket> monthlyCompletedTix = new List<ServiceTicket>();
    int thisMonth = monthInt;
    int thisYear = DateTime.Now.Year;

    foreach (ServiceTicket completedT in completedTix)
    {
        DateTime ticketDate = completedT.DateCompleted.Value;
        var ticketMonth = ticketDate.Month;
        var ticketYear = ticketDate.Year;
        if (ticketMonth == thisMonth && ticketYear == thisYear)
        {
            monthlyCompletedTix.Add(completedT);
        }
    }

    // if no tickets found for month searched, retun no content
    if (monthlyCompletedTix.Count == 0)
    {
        return Results.Ok(monthlyCompletedTix);
    }

    // group monthlyCompletedTix by employeeId
    var groupedTix = monthlyCompletedTix.GroupBy(t => t.EmployeeId).OrderByDescending(group => group.Count()).First();

    // return employee whose employeeId matches groupedTix.Key (highest occuring employeeId in grouping above)
    List<Employee> resultingArrOfEmployees = new List<Employee>();
    foreach (ServiceTicket servTick in groupedTix)
    {
        foreach (Employee emplo in employees)
        {
            if (emplo.Id == servTick.EmployeeId)
            {
                resultingArrOfEmployees.Add(emplo);
            }
        }
    }

    // if no employees found --- return empty array
    if (resultingArrOfEmployees == null)
    {
        return Results.Ok(resultingArrOfEmployees);
    }

    // if multiple employees are found - return array with both employees
    if (resultingArrOfEmployees.Count > 1)
    {
        return Results.Ok(resultingArrOfEmployees.Select(resultEmployee =>
        {
            List<ServiceTicket> rTix = serviceTickets.Where(servT => servT.EmployeeId == resultEmployee.Id).ToList();

            return Results.Ok(new EmployeeDTO
            {
                Id = resultEmployee.Id,
                Name = resultEmployee.Name,
                Specialty = resultEmployee.Specialty,
                ServiceTickets = rTix.Select(rt => new ServiceTicketDTO
                {
                    Id = rt.Id,
                    CustomerId = rt.CustomerId,
                    EmployeeId = rt.EmployeeId,
                    Description = rt.Description,
                    Emergency = rt.Emergency,
                    DateCompleted = rt.DateCompleted
                }).ToList()
            });
        }).ToList());
    }
    else
    {
        Employee result = resultingArrOfEmployees[0];

        // find serviceTickets for result
        List<ServiceTicket> resTix = serviceTickets.Where(servT => servT.EmployeeId == result.Id).ToList();

        return Results.Ok(new EmployeeDTO
        {
            Id = result.Id,
            Name = result.Name,
            Specialty = result.Specialty,
            ServiceTickets = resTix.Select(rt => new ServiceTicketDTO
            {
                Id = rt.Id,
                CustomerId = rt.CustomerId,
                EmployeeId = rt.EmployeeId,
                Description = rt.Description,
                Emergency = rt.Emergency,
                DateCompleted = rt.DateCompleted
            }).ToList()
        });
    };
});

// GET all completed service tickets---------------------------------------------------
// in order of completion data, oldest first
app.MapGet("servicetickets/complete", () =>
{
    var completedTickets = serviceTickets.Where(st => st.DateCompleted != null).ToList();

    if (completedTickets == null)
    {
        return Results.NoContent();
    }

    // Group completed tickets by DateCompleted
    var orderedTickets = completedTickets.GroupBy(t => t.DateCompleted).ToList();

    // flatten in order to select the ticket istead of Igrouping
    var flattenedTickets = orderedTickets.SelectMany(group => group).ToList();

    // Use flattenedTickets to return results to client
    return Results.Ok(flattenedTickets.Select(flatT =>
    {
        // find employee
        Employee employee = employees.FirstOrDefault(e => e.Id == flatT.EmployeeId);
        // find customer
        Customer customer = customers.FirstOrDefault(c => c.Id == flatT.CustomerId);

        return new ServiceTicketDTO
        {
            Id = flatT.Id,
            CustomerId = flatT.CustomerId,
            Customer = new CustomerDTO
            {
                Id = customer.Id,
                Name = customer.Name,
                Address = customer.Address
            },
            EmployeeId = flatT.EmployeeId,
            Employee = new EmployeeDTO
            {
                Id = employee.Id,
                Name = employee.Name,
                Specialty = employee.Specialty
            },
            Description = flatT.Description,
            Emergency = flatT.Emergency,
            DateCompleted = flatT.DateCompleted
        };
    }
    ).ToList());
});


// GET all priority tickets-------------------------------------------------------------
// return all incompete tickets
// in order first by whether they are emergenciees THEN -->
// by whether they are assigned or not (unassigned first)
app.MapGet("servicetickets/priority", () => {
    // get incomplete tickets
    List<ServiceTicket> incompleteTix = serviceTickets.Where(st => st.DateCompleted == null).ToList();

    // group by emergency first THEN unassigned first
    var groupedTix = incompleteTix.OrderBy(t => t.Emergency == false).ThenBy(t => t.EmployeeId != null).ToList();

    // use grouped array to return resulting array of tickets to client
    return groupedTix.Select(gt => new ServiceTicketDTO
    {
        Id = gt.Id,
        CustomerId = gt.CustomerId,
        DateCompleted = gt.DateCompleted,
        Description = gt.Description,
        Emergency = gt.Emergency,
        EmployeeId = gt.EmployeeId,
    }).ToList();

});

app.Run();