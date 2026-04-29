using Asp.Versioning.ApiExplorer;
using Hangfire;

namespace InsuranceApp.WebApi.Extensions;

internal static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseDeveloperTools(this IApplicationBuilder app)
    {
        app.UseSwagger();

        var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
        app.UseSwaggerUI(options =>
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    $"InsuranceApp WebAPI {description.GroupName.ToUpperInvariant()}");
            }
        });

        app.UseHangfireDashboard();

        return app;
    }
}