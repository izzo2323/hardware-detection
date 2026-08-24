using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace HardwareDetector
{
    public static class WebServer
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static void Run(int port)
        {
            string wwwroot = Path.Combine(AppContext.BaseDirectory, "wwwroot");

            if (!Directory.Exists(wwwroot))
            {
                Console.WriteLine($"Error: wwwroot directory not found at '{wwwroot}'.");
                return;
            }

            var listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{port}/");

            try
            {
                listener.Start();
            }
            catch (HttpListenerException ex)
            {
                Console.WriteLine($"Error starting web console: {ex.Message}");
                return;
            }

            Console.WriteLine("Hardware Detector web console");
            Console.WriteLine("===========================");
            Console.WriteLine($"Listening on http://localhost:{port}/");
            Console.WriteLine("Press Ctrl+C to stop.");

            while (true)
            {
                HttpListenerContext context;
                try
                {
                    context = listener.GetContext();
                }
                catch (HttpListenerException)
                {
                    break;
                }

                try
                {
                    HandleRequest(context, wwwroot);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error handling request: {ex.Message}");
                    try
                    {
                        context.Response.StatusCode = 500;
                        context.Response.Close();
                    }
                    catch
                    {
                        // response may already be closed; nothing more to do
                    }
                }
            }
        }

        private static void HandleRequest(HttpListenerContext context, string wwwroot)
        {
            var request = context.Request;
            var response = context.Response;
            string path = request.Url?.AbsolutePath ?? "/";

            if (string.Equals(path, "/api/hardware", StringComparison.OrdinalIgnoreCase))
            {
                var snapshot = HardwareInspector.Collect();
                var json = JsonSerializer.Serialize(snapshot, JsonOptions);
                WriteResponse(response, Encoding.UTF8.GetBytes(json), "application/json");
                return;
            }

            if (path == "/")
            {
                path = "/index.html";
            }

            string requestedPath = Path.GetFullPath(Path.Combine(wwwroot, path.TrimStart('/')));
            string wwwrootFullPath = Path.GetFullPath(wwwroot);

            if (!requestedPath.StartsWith(wwwrootFullPath, StringComparison.Ordinal) || !File.Exists(requestedPath))
            {
                response.StatusCode = 404;
                WriteResponse(response, Encoding.UTF8.GetBytes("404 Not Found"), "text/plain");
                return;
            }

            byte[] bytes = File.ReadAllBytes(requestedPath);
            WriteResponse(response, bytes, GetContentType(requestedPath));
        }

        private static void WriteResponse(HttpListenerResponse response, byte[] bytes, string contentType)
        {
            response.ContentType = contentType;
            response.ContentLength64 = bytes.Length;
            response.OutputStream.Write(bytes, 0, bytes.Length);
            response.OutputStream.Close();
        }

        private static string GetContentType(string filePath) => Path.GetExtension(filePath).ToLowerInvariant() switch
        {
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".json" => "application/json",
            _ => "application/octet-stream"
        };
    }
}
