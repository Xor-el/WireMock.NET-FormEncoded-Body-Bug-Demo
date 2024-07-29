// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using WireMock.Net.Testcontainers;

var container = new WireMockContainerBuilder().WithMappings(Helper.MappingsPath)
                                              .WithAutoRemove(true)
                                              .WithCleanUp(true)
                                              .Build();

await container.StartAsync().ConfigureAwait(false);

var publicBaseUrl = container.GetPublicUrl() + "api";

Console.WriteLine("baseUrl = " + publicBaseUrl);

var wireMockAdminClient = container.CreateWireMockAdminClient();

var settings = await wireMockAdminClient.GetSettingsAsync();
Console.WriteLine("settings = " + JsonConvert.SerializeObject(settings, Formatting.Indented));

var mappings = await wireMockAdminClient.GetMappingsAsync();
Console.WriteLine("mappings = " + JsonConvert.SerializeObject(mappings, Formatting.Indented));

var client = container.CreateClient();

Console.WriteLine("=================================Starting OrderedContent Tests=================================");

// grant_type is first both here and in the mapping file
var orderedContent = new FormUrlEncodedContent(
[
    new KeyValuePair<string, string>("grant_type", "client_credentials"),
    new KeyValuePair<string, string>("client_id", "DDCD99EE1531484E4E21D5EC9FBA5D8B"),
    new KeyValuePair<string, string>("client_secret", "RERDRDk5RUUxNTMxNDg0RTRFMjFENUVDOUZCQTVEOEI=")
]);

//var exactOrderedContentResponse = await client.PostAsync($"{publicBaseUrl}/exact", orderedContent).ConfigureAwait(false);

//Console.WriteLine("ExactOrderedContent Response = " + exactOrderedContentResponse.StatusCode);

//var regexOrderedContentResponse = await client.PostAsync($"{publicBaseUrl}/regex", orderedContent).ConfigureAwait(false);

//Console.WriteLine("RegexOrderedContent Response = " + regexOrderedContentResponse.StatusCode);

var formUrlEncodedOrderedContentResponse = await client.PostAsync($"{publicBaseUrl}/formurlencoded", orderedContent).ConfigureAwait(false);

Console.WriteLine("FormUrlEncodedOrderedContent Response = " + formUrlEncodedOrderedContentResponse.StatusCode);

Console.WriteLine("=================================Starting UnOrderedContent Tests=================================");

// order changed, grant_type is now last here but still first in mapping file
var unOrderedContent = new FormUrlEncodedContent(
[
    new KeyValuePair<string, string>("client_id", "DDCD99EE1531484E4E21D5EC9FBA5D8B"),
    new KeyValuePair<string, string>("client_secret", "RERDRDk5RUUxNTMxNDg0RTRFMjFENUVDOUZCQTVEOEI="),
    new KeyValuePair<string, string>("grant_type", "client_credentials"),
]);

//var exactUnOrderedContentResponse = await client.PostAsync($"{publicBaseUrl}/exact", unOrderedContent).ConfigureAwait(false);

//Console.WriteLine("ExactOrderedContent Response = " + exactUnOrderedContentResponse.StatusCode);

//var regexUnOrderedContentResponse = await client.PostAsync($"{publicBaseUrl}/regex", unOrderedContent).ConfigureAwait(false);

//Console.WriteLine("RegexOrderedContent Response = " + regexUnOrderedContentResponse.StatusCode);

var formUrlEncodedUnOrderedContentResponse = await client.PostAsync($"{publicBaseUrl}/formurlencoded", unOrderedContent).ConfigureAwait(false);

Console.WriteLine("FormUrlEncodedUnOrderedContent Response = " + formUrlEncodedUnOrderedContentResponse.StatusCode);


await container.StopAsync();

Console.WriteLine("Finished Operations");

Console.ReadLine();

public static class Helper
{
    private static Lazy<string> RootProjectPath { get; } = new(() =>
    {
        var directoryInfo = new DirectoryInfo(AppContext.BaseDirectory);
        do
        {
            directoryInfo = directoryInfo.Parent!;
        } while (!directoryInfo.Name.EndsWith("WireMock.NET-FormEncoded-Body-Bug-Demo", StringComparison.OrdinalIgnoreCase));

        return directoryInfo.FullName;
    });

    public static string MappingsPath => Path.Combine(RootProjectPath.Value, "Mappings");
}



