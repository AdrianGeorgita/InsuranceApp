using FluentAssertions;
using InsuranceApp.Application.Common.Constants;
using InsuranceApp.IntegrationTests.Extensions;

namespace InsuranceApp.IntegrationTests.RateLimiter;

public class RateLimiterTests : IntegrationTestBase
{
    [Fact]
    public async Task ListAllFeeConfigurations_GivenManyConcurrentRequests_ShouldReturnTooManyRequests()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        const int requestsToSend = 150;
        const int requestsLimit = 100;
        const string apiUrl = "http://localhost:5285/api/admin/fees";

        var failedRequests = 0;
        await Parallel.ForAsync(0, requestsToSend, async (_, _) =>
        {
            var response = await HttpClient.GetAsync(apiUrl, CancellationToken.None);
            if (!response.IsSuccessStatusCode)
            {
                Interlocked.Increment(ref failedRequests);
            }
        });

        failedRequests.Should().Be(requestsToSend - requestsLimit);
    }

    [Fact]
    public async Task ListAllFeeConfigurations_GivenMoreRequestBatchesWithADelayBetweenThem_ShouldNotFailAnyRequest()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        const int requestsLimit = 100;
        const int batchesToSend = 2;
        const string apiUrl = "http://localhost:5285/api/admin/fees";

        var failedRequests = 0;
        for (var i = 0; i < batchesToSend; i++)
        {
            await Parallel.ForAsync(0, requestsLimit, async (_, _) =>
            {
                var response = await HttpClient.GetAsync(apiUrl, CancellationToken.None);
                if (!response.IsSuccessStatusCode)
                {
                    Interlocked.Increment(ref failedRequests);
                }
            });

            await Task.Delay(TimeSpan.FromSeconds(10));
        }


        failedRequests.Should().Be(0);
    }

    [Fact]
    public async Task ListAllFeeConfigurations_GivenLessRequestsThanTheLimit_ShouldNotFailAnyRequest()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        const int requestsToSend = 50;
        const string apiUrl = "http://localhost:5285/api/admin/fees";

        var failedRequests = 0;
        await Parallel.ForAsync(0, requestsToSend, async (_, _) =>
        {
            var response = await HttpClient.GetAsync(apiUrl, CancellationToken.None);
            if (!response.IsSuccessStatusCode)
            {
                Interlocked.Increment(ref failedRequests);
            }
        });

        failedRequests.Should().Be(0);
    }
}