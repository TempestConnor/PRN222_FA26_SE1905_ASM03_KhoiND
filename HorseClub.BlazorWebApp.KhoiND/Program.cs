using HorseClub.BlazorWebApp.KhoiND.Components;
using HorseClub.Services.KhoiND;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Transient, not scoped: a circuit scope lives as long as the browser tab, and a
// long-lived DbContext breaks when two operations overlap.
builder.Services.AddTransient<IHorseServiceKhoiND, HorseServiceKhoiND>();
builder.Services.AddTransient<IOwnerServiceKhoiND, OwnerServiceKhoiND>();
builder.Services.AddTransient<IStaffProfileServiceKhoiND, StaffProfileServiceKhoiND>();
builder.Services.AddTransient<ISupplierServiceKhoiND, SupplierServiceKhoiND>();
builder.Services.AddTransient<ISupplyCategoryServiceKhoiND, SupplyCategoryServiceKhoiND>();
builder.Services.AddTransient<ISupplyItemServiceKhoiND, SupplyItemServiceKhoiND>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
