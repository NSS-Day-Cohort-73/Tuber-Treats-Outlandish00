using TuberTreats.Models;
using TuberTreats.Models.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

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

app.UseAuthorization();

List<Customer> customers = new List<Customer>
{
    new Customer
    {
        Id = 1,
        Name = "Claude",
        Address = "1223 Go Home Way",
    },
    new Customer
    {
        Id = 2,
        Name = "Jason",
        Address = "8675 I got it Street",
    },
    new Customer
    {
        Id = 3,
        Name = "Chowder",
        Address = "455 Crazy Road",
    },
    new Customer
    {
        Id = 4,
        Name = "Eugune",
        Address = "324 Bikini Bottom Road",
    },
    new Customer
    {
        Id = 5,
        Name = "Uzu",
        Address = "565 Burning Leaf Road",
    },
};

List<TuberDriver> tuberDrivers = new List<TuberDriver>
{
    new TuberDriver { Id = 1, Name = "John" },
    new TuberDriver { Id = 2, Name = "Joseph" },
    new TuberDriver { Id = 3, Name = "Craig" },
};

List<Topping> toppings = new List<Topping>
{
    new Topping { Id = 1, Name = "Cheese" },
    new Topping { Id = 2, Name = "Butter" },
    new Topping { Id = 3, Name = "Chives" },
    new Topping { Id = 4, Name = "Sour Cream" },
    new Topping { Id = 5, Name = "Bacon" },
};

List<TuberOrder> tuberOrders = new List<TuberOrder>
{
    new TuberOrder
    {
        Id = 1,
        OrderPlacedOnDate = DateTime.Today,
        CustomerId = 5,
        TuberDriverId = 1,
        DeliveredOnDate = null,
    },
    new TuberOrder
    {
        Id = 2,
        OrderPlacedOnDate = DateTime.Today,
        CustomerId = 3,
        TuberDriverId = 2,
        DeliveredOnDate = null,
    },
    new TuberOrder
    {
        Id = 3,
        OrderPlacedOnDate = DateTime.Today,
        CustomerId = 2,
        TuberDriverId = 3,
        DeliveredOnDate = null,
    },
};
List<TuberTopping> tuberToppings = new List<TuberTopping>
{
    new TuberTopping
    {
        Id = 1,
        TuberOrderId = 2,
        ToppingId = 1,
    },
    new TuberTopping
    {
        Id = 2,
        TuberOrderId = 3,
        ToppingId = 3,
    },
    new TuberTopping
    {
        Id = 3,
        TuberOrderId = 3,
        ToppingId = 2,
    },
    new TuberTopping
    {
        Id = 4,
        TuberOrderId = 3,
        ToppingId = 5,
    },
};

//add endpoints here

//get all tuber orders
app.MapGet(
    "/tuberorders",
    () =>
    {
        var foundTuberOrders = tuberOrders.Select(o => new TuberOrderDTO
        {
            Id = o.Id,
            OrderPlacedOnDate = o.OrderPlacedOnDate,
            CustomerId = o.CustomerId,
            TuberDriverId = o.TuberDriverId,
            DeliveredOnDate = null,
            Toppings = tuberToppings
                .Where(tt => tt.TuberOrderId == o.Id)
                .SelectMany(tt => toppings.Where(top => top.Id == tt.ToppingId))
                .Select(top => new ToppingDTO { Id = top.Id, Name = top.Name })
                .ToList(),
        });
        return foundTuberOrders;
    }
);

//get a specific tuber order by id
app.MapGet(
    "/tuberorders/{id}",
    (int id) =>
    {
        try
        {
            TuberOrder foundOrder = tuberOrders.First(t => t.Id == id);

            return Results.Ok(
                new TuberOrderDTO
                {
                    Id = foundOrder.Id,
                    OrderPlacedOnDate = foundOrder.OrderPlacedOnDate,
                    CustomerId = foundOrder.CustomerId,
                    TuberDriverId = foundOrder.TuberDriverId,
                    DeliveredOnDate = foundOrder.DeliveredOnDate,
                    Toppings = tuberToppings
                        .Where(tt => tt.TuberOrderId == foundOrder.Id)
                        .SelectMany(tt => toppings.Where(top => top.Id == tt.ToppingId))
                        .Select(top => new ToppingDTO { Id = top.Id, Name = top.Name })
                        .ToList(),
                }
            );
        }
        catch
        {
            return Results.NotFound();
        }
    }
);

//adds a new tuber order
app.MapPost(
    "/tuberorders",
    (TuberOrder newTuberOrder) =>
    {
        try
        {
            newTuberOrder.Id = tuberOrders.Max(o => o.Id) + 1;
            newTuberOrder.OrderPlacedOnDate = DateTime.Now;
            tuberOrders.Add(newTuberOrder);
            return Results.Created(
                $"/tuberorders/{newTuberOrder.Id}",
                new TuberOrderDTO
                {
                    Id = newTuberOrder.Id,
                    OrderPlacedOnDate = newTuberOrder.OrderPlacedOnDate,
                    CustomerId = newTuberOrder.CustomerId,
                    TuberDriverId = newTuberOrder.TuberDriverId,
                    DeliveredOnDate = newTuberOrder.DeliveredOnDate,
                    Toppings = null,
                }
            );
        }
        catch
        {
            return Results.NotFound();
        }
    }
);

//assigns a tuberDriverId to a specific order
app.MapPut(
    "/tuberorders/{id}",
    (int id, TuberOrder updatedTuberOrder) =>
    {
        try
        {
            TuberOrder foundOrder = tuberOrders.FirstOrDefault(t => t.Id == id);
            if (foundOrder == null)
            {
                return Results.NotFound();
            }
            foundOrder.TuberDriverId = updatedTuberOrder.TuberDriverId;
            return Results.Ok(
                new TuberOrderDTO
                {
                    Id = foundOrder.Id,
                    OrderPlacedOnDate = foundOrder.OrderPlacedOnDate,
                    CustomerId = foundOrder.CustomerId,
                    TuberDriverId = foundOrder.TuberDriverId,
                    DeliveredOnDate = foundOrder.DeliveredOnDate,
                    Toppings = null,
                }
            );
        }
        catch
        {
            return Results.BadRequest();
        }
    }
);

// gives a specific order a dateDelivered property
app.MapPost(
    "/tuberorders/{id}/complete",
    (int id) =>
    {
        try
        {
            TuberOrder orderToComplete = tuberOrders.First(order => order.Id == id);
            orderToComplete.DeliveredOnDate = DateTime.Now;
            return Results.Ok(
                new TuberOrderDTO
                {
                    Id = orderToComplete.Id,
                    OrderPlacedOnDate = orderToComplete.OrderPlacedOnDate,
                    CustomerId = orderToComplete.CustomerId,
                    TuberDriverId = orderToComplete.TuberDriverId,
                    DeliveredOnDate = orderToComplete.DeliveredOnDate,
                    Toppings = null,
                }
            );
        }
        catch
        {
            return Results.BadRequest();
        }
    }
);

//gets all toppings
app.MapGet(
    "/toppings",
    () =>
    {
        return toppings.Select(t => new ToppingDTO { Id = t.Id, Name = t.Name });
    }
);

//gets topping by id
app.MapGet(
    "/toppings/{id}",
    (int id) =>
    {
        try
        {
            Topping foundTopping = toppings.FirstOrDefault(t => t.Id == id);
            return Results.Ok(new ToppingDTO { Id = foundTopping.Id, Name = foundTopping.Name });
        }
        catch
        {
            return Results.NotFound();
        }
    }
);

//get all tuber toppings
app.MapGet(
    "/tubertoppings",
    () =>
    {
        try
        {
            var foundTuberToppings = tuberToppings.Select(t => new TuberToppingDTO
            {
                Id = t.Id,
                TuberOrderId = t.TuberOrderId,
                ToppingId = t.ToppingId,
            });
            return Results.Ok(foundTuberToppings);
        }
        catch
        {
            return Results.NotFound();
        }
    }
);

//new Topping to order
app.MapPost(
    "/tubertoppings",
    (TuberTopping newTuberTopping) =>
    {
        try
        {
            newTuberTopping.Id = tuberToppings.Max(t => t.Id) + 1;
            tuberToppings.Add(newTuberTopping);
            return Results.Created(
                $"/tubbertoppings/{newTuberTopping.Id}",
                new TuberToppingDTO
                {
                    Id = newTuberTopping.Id,
                    TuberOrderId = newTuberTopping.TuberOrderId,
                    ToppingId = newTuberTopping.ToppingId,
                }
            );
        }
        catch
        {
            return Results.NotFound();
        }
    }
);

//remove a topping from an order
app.MapDelete(
    "/tubertoppings/{id}",
    (int id) =>
    {
        TuberTopping foundTuberTopping = tuberToppings.FirstOrDefault(t => t.Id == id);
        tuberToppings.Remove(foundTuberTopping);
        return Results.NoContent();
    }
);

//get all customers

app.MapGet(
    "/customers",
    () =>
    {
        return customers.Select(c => new CustomerDTO
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address,
        });
    }
);

//get customer by Id with expanded properties
app.MapGet(
    "/customers/{id}",
    (int id) =>
    {
        try
        {
            Customer foundCustomer = customers.FirstOrDefault(c => c.Id == id);
            List<TuberOrder> foundOrders = tuberOrders
                .Where(to => to.CustomerId == foundCustomer.Id)
                .ToList();

            return Results.Ok(
                new CustomerDTO
                {
                    Id = foundCustomer.Id,
                    Name = foundCustomer.Name,
                    Address = foundCustomer.Address,
                    TuberOrders = foundOrders
                        .Select(fo => new TuberOrderDTO
                        {
                            Id = fo.Id,
                            OrderPlacedOnDate = fo.OrderPlacedOnDate,
                            CustomerId = fo.CustomerId,
                            TuberDriverId = fo.TuberDriverId,
                            DeliveredOnDate = fo.DeliveredOnDate,
                            Toppings = tuberToppings
                                .Where(tt => tt.TuberOrderId == fo.Id)
                                .SelectMany(tt => toppings.Where(top => top.Id == tt.ToppingId))
                                .Select(top => new ToppingDTO { Id = top.Id, Name = top.Name })
                                .ToList(),
                        })
                        .ToList(),
                }
            );
        }
        catch
        {
            return Results.NotFound();
        }
    }
);

//post new customer
app.MapPost(
    "/customers",
    (Customer newCustomer) =>
    {
        try
        {
            newCustomer.Id = customers.Max(c => c.Id) + 1;
            customers.Add(newCustomer);
            return Results.Created(
                $"/customers/{newCustomer.Id}",
                new CustomerDTO
                {
                    Id = newCustomer.Id,
                    Name = newCustomer.Name,
                    Address = newCustomer.Address,
                }
            );
        }
        catch
        {
            return Results.NotFound();
        }
    }
);

//Delete a customer
app.MapDelete(
    "/customers/{id}",
    (int id) =>
    {
        try
        {
            Customer foundCustomer = customers.FirstOrDefault(c => c.Id == id);
            customers.Remove(foundCustomer);
            return Results.NoContent();
        }
        catch
        {
            return Results.NotFound();
        }
    }
);

//get all TuberDrivers
app.MapGet(
    "/tuberdrivers",
    () =>
    {
        try
        {
            return Results.Ok(
                tuberDrivers.Select(td => new TuberDriverDTO { Id = td.Id, Name = td.Name })
            );
        }
        catch
        {
            return Results.NotFound();
        }
    }
);

//get employee by Id with their deliveries
app.MapGet(
    "/tuberdrivers/{id}",
    (int id) =>
    {
        TuberDriver foundTuberDriver = tuberDrivers.FirstOrDefault(td => td.Id == id);
        List<TuberOrder> foundTuberOrders = tuberOrders
            .Where(to => to.TuberDriverId == id)
            .ToList();
        return Results.Ok(
            new TuberDriverDTO
            {
                Id = foundTuberDriver.Id,
                Name = foundTuberDriver.Name,
                TuberDeliveries = foundTuberOrders
                    .Select(to => new TuberOrderDTO
                    {
                        Id = to.Id,
                        OrderPlacedOnDate = to.OrderPlacedOnDate,
                        CustomerId = to.CustomerId,
                        TuberDriverId = to.TuberDriverId,
                        DeliveredOnDate = to.DeliveredOnDate,
                        Toppings = tuberToppings
                            .Where(tt => tt.TuberOrderId == to.Id)
                            .SelectMany(tt => toppings.Where(top => top.Id == tt.ToppingId))
                            .Select(top => new ToppingDTO { Id = top.Id, Name = top.Name })
                            .ToList(),
                    })
                    .ToList(),
            }
        );
    }
);

app.Run();

//don't touch or move this!
public partial class Program { }
