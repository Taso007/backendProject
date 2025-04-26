using System.Net;
using System.Net.Sockets;

//1. Start the Web Server
//created TCP server on port 8080 
var listener = new TcpListener(IPAddress.Any, 8080);
listener.Start();

//accepting incoming client connections 
var client = listener.AcceptTcpClient();
var stream = client.GetStream();

