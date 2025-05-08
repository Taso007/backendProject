using System.Net;
using System.Net.Sockets;
using System.Text;


//created TCP server on port 8080 
var listener = new TcpListener(IPAddress.Any, 8080);
listener.Start();

//accepting incoming client connections always
while (true)
{
    var client = listener.AcceptTcpClient();

    //For each incoming connection, use threadpool to handle the request.
    //Handling multiple concurrent connections efficiently.
    ThreadPool.QueueUserWorkItem(_ => HandleClient(client));
}

//Handle Client Connections (Using Threads)
static void HandleClient(TcpClient client)
{
    try
    {
        var stream = client.GetStream();
        var reader = new StreamReader(stream, Encoding.UTF8);
        var writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };

        //Read the request line, first line is the header
        string requestLine = reader.ReadLine();
        Console.WriteLine($"Request header{requestLine}");

        // Log the client request to a file
        LogRequest(client, requestLine);

        if (string.IsNullOrEmpty(requestLine))
        {
            return;
        }

        //parsing the request line
        var tokens = requestLine.Split(' ');
        //if token length is less then 2 it means that  Request is broken or invalid
        if (tokens.Length < 2)
        {
            return;
        }

        var method = tokens[0];
        var url = tokens[1];

        //If the request method is not GET, return a "405 Method Not Allowed" response.
        if(method != "GET")
        {
            var status = "405 Method Not Allowed";
            var contentType = "text/html";
            var body = LoadErrorPage();
            SendResponse(writer, status, contentType, body);
            return;
        }

        //Extract the requested filename from the request URL.
        string fileName = url.TrimStart('/');
        if (string.IsNullOrEmpty(fileName))
        {
            fileName = "index.html";
        }

        //Check if the file extension is valid (.html, .css, .js). If not, return "403 Forbidden".  
        string extension = Path.GetExtension(fileName);
        if(extension != ".html" && extension != ".css" && extension != ".js")
        {
            var status = "403 Forbidden";
            var contentType = "text/html";
            var body = LoadErrorPage();
            SendResponse(writer, status, contentType, body);
            return;
        }

        //Look for the file in the specified folder: so first build the full filepath
        string filePath = Path.Combine("webroot", fileName);
        //If the file exists, send its content with the appropriate HTTP headers.
        if (File.Exists(filePath))
        {
            //first, lets read the file content
            var fileBytes = File.ReadAllBytes(filePath);
            var contentType = GetContentType(extension);

            //write hhtp headers
            writer.WriteLine($"HTTP/1.1 200 OK");
            writer.WriteLine($"Content-Type: {contentType}");
            writer.WriteLine($"Content-Length: {fileBytes.Length}");
            writer.WriteLine(); // Blank line between headers and body
            writer.Flush();

            //and lastly send the file content
            stream.Write(fileBytes, 0, fileBytes.Length);
        }
        //If the file does not exist, return a 404 Not Found response.
        else
        {
            var status = "404 Not Found";
            var contentType = "text/html";
            var body = LoadErrorPage();
            SendResponse(writer, status, contentType, body);
            return;
        }

    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
    finally
    {
        //After processing the request, the thread should close the connection.
        client.Close();
    }
}

//helper to send html response
static void SendResponse(StreamWriter writer, string status, string contentType, string body)
{
    writer.WriteLine($"HTTP/1.1 {status}");
    writer.WriteLine($"Content-Type: {contentType}");
    writer.WriteLine($"Content-Length: {Encoding.UTF8.GetByteCount(body)}");
    writer.WriteLine();
    writer.Write(body);
    writer.Flush();
}

// Helper to detect content type based on extension
//Supporting MIME types for correct content delivery (Content-Type: text/css, etc.).
static string GetContentType(string extension)
{
    return extension.ToLower() switch
    {
        ".html" => "text/html",
        ".css" => "text/css",
        ".js" => "application/javascript",
        ".json" => "application/json",
        ".png" => "image/png",
        ".jpg" => "image/jpeg",
        ".gif" => "image/gif",
        ".svg" => "image/svg+xml",
        _ => "application/octet-stream" 
    };
}


//helper for Logging client requests to a file.
static void LogRequest(TcpClient client, string requestLine)
{
    string logFilePath = "server_log.txt"; 
    string clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(); 
    string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    string logEntry = $"{timeStamp} - {clientIP} - {requestLine}";
    File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
}

//helper for Serving a custom error page from error.html instead of an inline error message.
static string LoadErrorPage()
{
    string errorPagePath = Path.Combine("webroot", "error.html");
    if (File.Exists(errorPagePath))
    {
        return File.ReadAllText(errorPagePath);
    }
    return "<html><body><h1>Error occurred</h1></body></html>";
}