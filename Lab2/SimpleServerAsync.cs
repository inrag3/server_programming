using System.Net;
using Logger;

namespace Lab2;

public class SimpleServerAsync
{
    private readonly HttpListener _listener;
    private readonly ILogger _logger;
    
    private const string ROOT = "wwwroot"; // Корневой каталог сервера
    
    public SimpleServerAsync(string prefix, ILogger logger)
    {
        _logger = logger;
        _listener = new HttpListener();
        _listener.Prefixes.Add(prefix);
    }

    public async Task StartAsync()
    {
        var tasks = new List<Task>();
        
        _listener.Start();

        try
        {
            while (_listener.IsListening)
            {
                HttpListenerContext context = await _listener.GetContextAsync();
                Task task = ProcessRequestAsync(context);
                tasks.Add(task);
            }
            
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger.Error($"Ошибка на сервере: {ex.Message}");
        }
    }
    
    private async Task ProcessRequestAsync(HttpListenerContext context)
    {
        string rootPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\.."));
        string filePath = Path.Combine(rootPath, ROOT, context.Request.Url.LocalPath.Trim('/'));
        
        var statusCode = 404;
        byte[] responseBytes = "404 Not Found"u8.ToArray();

        if (File.Exists(filePath))
        {
            responseBytes = await File.ReadAllBytesAsync(filePath);
            context.Response.ContentType = GetMimeType(filePath);
            statusCode = 200;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentLength64 = responseBytes.Length;
        await context.Response.OutputStream.WriteAsync(responseBytes);
        context.Response.OutputStream.Close();

        _logger.Info($"{context.Request.HttpMethod} {context.Request.Url.LocalPath} -> {statusCode}");
    }
    
    private string GetMimeType(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLower();
        return extension switch
        {
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".txt" => "text/plain",
        };
    }
}