using Microsoft.EntityFrameworkCore;
using TXConstructionManagement.Data;
using TXConstructionManagement.Services.Interfaces;
using TXConstructionManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS
builder.Services.AddCors(opt =>
    opt.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// DI
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IWorkflowEngineService, WorkflowEngineService>();
builder.Services.AddScoped<IMonthlyPlanService, MonthlyPlanService>();
builder.Services.AddScoped<ISecondaryOperationService, SecondaryOperationService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<SettlementService>();
builder.Services.AddScoped<InspectionService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<DocumentAutoService>();
builder.Services.AddScoped<WorkflowConfigService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<AnalysisService>();
builder.Services.AddScoped<ReviewMeetingService>();
builder.Services.AddScoped<IWarningService, WarningService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandling();
app.UseAuditLogging();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();