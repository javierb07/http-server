using System.Net;
using System.Net.Sockets;

using http.server.entities;
using http.server.extensions;

namespace http.server;

internal class Program {
  private static async Task Main(string[] args) {
    TcpListener server = new(IPAddress.Any, 4221);
    server.Start();

    while (true) {
      Socket socket = await server.AcceptSocketAsync();
      await Task.Run(() => { _ = HandleConnection(socket, args); });
    }
  }

  private static async Task HandleConnection(Socket socket, string[] args) {
    try {
      while (true) {
        byte[] buffer = new byte[socket.ReceiveBufferSize];
        if (await socket.ReceiveAsync(buffer, SocketFlags.None) == 0) { break; }
        Request request = Request.Parse(buffer);
        string? filesDirectory = args.Contains("--directory") ? args[Array.IndexOf(args, "--directory") + 1] : Directory.GetCurrentDirectory();
        HttpResponseMessage response = socket.HandleRequest(request, filesDirectory);
        await socket.SendAsync(response);
      }
    } finally {
      if (socket.Connected) { socket.Shutdown(SocketShutdown.Both); }
      socket.Close();
    }
  }
}