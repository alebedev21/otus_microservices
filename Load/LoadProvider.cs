﻿using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Load;
public class LoadProvider(int index, string baseUrl, CancellationToken token)
{
    private readonly HttpClient _httpClient = new HttpClient()
    {
        BaseAddress = new Uri(baseUrl),
    };

    private readonly Random _random = new Random();
    private readonly CancellationToken _token = token;
    private readonly int _index = index;

    public async Task GenerateLoad()
    {
        while(!_token.IsCancellationRequested)
        {
            await ImitateDelay();
            
            var id = await Create();
            await Get(id);
            await Edit(id);
            await Delete(id);
        }

        WriteToConsole("Done.");
    }

    private async Task GetAll()
    {
        while (true)
        {
            HttpResponseMessage? response = null;
            try
            {
                response = await _httpClient.GetAsync("/users");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                if (ex is not HttpRequestException)
                {
                    WriteToConsole($"{ex.GetType()}: {ex.Message}");
                }

                if (ex is HttpRequestException)
                {
                    continue;
                }
            }
            finally
            {
                if (response != null)
                {
                    WriteToConsole($"GetAll: {response.StatusCode}");
                }
            }

            response?.Dispose();
            break;
        }
    }

    private async Task Get(string id)
    {
        while (true)
        {
            HttpResponseMessage? response = null;
            try
            {
                response = await _httpClient.GetAsync($"/user/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                if (ex is not HttpRequestException)
                {
                    WriteToConsole($"{ex.GetType()}: {ex.Message}");
                }

                if (ex is HttpRequestException)
                {
                    continue;
                }
            }
            finally
            {
                if (response != null)
                {
                    WriteToConsole($"Get: {response.StatusCode}");
                }
            }

            response?.Dispose();
            break;
        }
    }

    private async Task Delete(string id)
    {
        while (true)
        {
            HttpResponseMessage? response = null;
            try
            {
                response = await _httpClient.DeleteAsync($"/user/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                if (ex is not HttpRequestException)
                {
                    WriteToConsole($"{ex.GetType()}: {ex.Message}");
                }

                if (ex is HttpRequestException)
                {
                    continue;
                }
            }
            finally
            {
                if (response != null)
                {
                    WriteToConsole($"Delete: {response.StatusCode}");
                }
            }

            response?.Dispose();
            break;
        }
    }

    private async Task Edit(string id)
    {
        UserUpdateRequest request = new(
            id,
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());

        while (true)
        {
            HttpResponseMessage? response = null;
            try
            {
                response = await _httpClient.PutAsync(
                    "/user",
                    JsonContent.Create(request, new MediaTypeHeaderValue("application/json")));

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                if (ex is not HttpRequestException)
                {
                    WriteToConsole($"{ex.GetType()}: {ex.Message}");
                }

                if (ex is HttpRequestException)
                {
                    continue;
                }
            }
            finally
            {
                if (response != null)
                {
                    WriteToConsole($"Edit: {response.StatusCode}");
                }
            }

            response?.Dispose();
            break;
        }
    }

    private async Task<string> Create()
    {
        UserAddRequest request = new(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());
        
        while (true)
        {
            HttpResponseMessage? response = null;
            try
            {
                response = await _httpClient.PostAsync(
                    "/user",
                    JsonContent.Create(request, new MediaTypeHeaderValue("application/json")));
                
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                if (ex is not HttpRequestException)
                {
                    WriteToConsole($"{ex.GetType()}: {ex.Message}");
                }

                if (ex is HttpRequestException)
                {
                    continue;
                }
            }
            finally
            {
                if (response != null)
                {
                    WriteToConsole($"Create: {response.StatusCode}");
                }
            }

            string id = response!.Headers.Location!.ToString().Split('/')[2];           
            response.Dispose();    
            
            return id;
        }
    }

    private void WriteToConsole(string message) => ConsoleWriter.WriteLine($"{_index}: {message}");

    private async Task ImitateDelay()
    {
        var value = _random.Next(1, 101);
        await Task.Delay(value);
    }
}