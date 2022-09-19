
using BankAPI.DataContext;
using BankAPI.IResponsitory;
using BankAPI.Responsitory;
using Microsoft.EntityFrameworkCore;
using Wkhtmltopdf.NetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("connectString"))
    );
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
builder.Services.AddScoped<IContact, ContactService>();
builder.Services.AddScoped<INews, NewService>();
builder.Services.AddScoped<ICustomer, CustomerService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<ITransaction,TransactionService>();
builder.Services.AddScoped<ICard, CardService>();
builder.Services.AddScoped<IAdmin, AdminService>();
builder.Services.AddScoped<IAccount, AccountService>();
builder.Services.AddScoped<IAdmin, AdminService>();
builder.Services.AddScoped<INotification, NotificationsService>();
builder.Services.AddScoped<ICheque, ChequeService>();
builder.Services.AddScoped<IConfigurationTransfer, ConfigurationService>();
builder.Services.AddScoped<IMessage, MessageService>();
builder.Services.AddSwaggerGen();
builder.Services.AddWkhtmltopdf("wkhtmltopdf");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();

app.MapControllers();

app.Run();
