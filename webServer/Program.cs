using System.Net;
using System.Net.Sockets;


//created TCP server on port 8080 
var listener = new TcpListener(IPAddress.Any, 8080);
listener.Start();

//accepting incoming client connections always
while (true)
{
    var client = listener.AcceptTcpClient();

    //For each incoming connection, create a new thread to handle the request.
    var clientThread = new Thread(() => HandleClient(client));
    clientThread.Start();
}

//Handle Client Connections (Using Threads)
static void HandleClient(TcpClient client)
{
    try
    {
        var stream = client.GetStream();
    }
    catch(Exception ex)
    {
        Console.WriteLine(ex);
    }
    finally
    {
        //After processing the request, the thread should close the connection.
        client.Close();
    }
}
