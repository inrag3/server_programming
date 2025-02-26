using System.Net;
using Logger;

namespace Lab1;
public class SimpleServer
{
    private readonly HttpListener _listener;
    private readonly ILogger _logger;
    
    private const string ROOT = "wwwroot"; // Корневой каталог сервера
    
    public SimpleServer(string prefix, ILogger logger)
    {
        _logger = logger;
        _listener = new HttpListener();
        _listener.Prefixes.Add(prefix);
    }

    public void Start()
    {
        _listener.Start();

        try
        {
            while (_listener.IsListening)
            {
                HttpListenerContext context = _listener.GetContext();
                ProcessRequest(context);
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"Ошибка на сервере: {ex.Message}");
        }
    }
    
    private void ProcessRequest(HttpListenerContext context)
    {
        
        string rootPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\.."));
        string filePath = Path.Combine(rootPath, ROOT, context.Request.Url.LocalPath.Trim('/'));
        
        var statusCode = 404;
        byte[] responseBytes = "404 Not Found"u8.ToArray();

        if (File.Exists(filePath))
        {
            responseBytes = File.ReadAllBytes(filePath);
            context.Response.ContentType = GetMimeType(filePath);
            statusCode = 200;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentLength64 = responseBytes.Length;
        context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
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