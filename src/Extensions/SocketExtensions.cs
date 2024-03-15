using System.Net;
using System.Net.Sockets;
using System.Text;

using http.server.entities;

namespace http.server.extensions;

public static class SocketExtensions {
  public static async Task SendAsync(this Socket socket, HttpResponseMessage httpResponse) {
    string content = await httpResponse.Content.ReadAsStringAsync();
    StringBuilder response = new();
    if (httpResponse.Content is not null && !string.IsNullOrEmpty(content)) {
      string contentType = httpResponse.Content is StreamContent ? "application/octet-stream" : "text/plain";
      response.Append($"HTTP/1.1 {(int)httpResponse.StatusCode} {httpResponse.StatusCode}");
      response.Append($"\r\nContent-Type: {contentType}");
      response.Append($"\r\nContent-Length: {content.Length}");
      response.Append("\r\n");
      response.Append($"\r\n{content}");
    } else {
      response.Append($"HTTP/1.1 {(int)httpResponse.StatusCode} {httpResponse.StatusCode}\r\n\r\n");
    }
    byte[] responseBytes = Encoding.UTF8.GetBytes(response.ToString());
    await socket.SendAsync(responseBytes, SocketFlags.None);
  }

  public static HttpResponseMessage HandleRequest(this Socket _, Request request, string? filesDirectory = null) {
    return request.Method.ToLower() switch {
      "get" => HandleGetRequest(request, filesDirectory),
      "post" => HandlePostRequest(request, filesDirectory),
      _ => new(HttpStatusCode.MethodNotAllowed) {
        Content = new StringContent("Method not allowed")
      },
    };
  }

  private static HttpResponseMessage HandleGetRequest(Request request, string? filesDirectory) {
    HttpResponseMessage response;
    if (request.Path.Equals("/")) {
      response = new(HttpStatusCode.OK) { };
    } else if (request.Path.StartsWith("/echo/")) {
      response = new(HttpStatusCode.OK) {
        Content = new StringContent(request.Path.Replace("/echo/", ""), Encoding.UTF8, "text/plain"),
      };
    } else if (request.Path.Equals("/user-agent")) {
      Console.WriteLine("User agent called");
      response = new(HttpStatusCode.OK) {
        Content = new StringContent(request.Headers["User-Agent"]),
      };
    } else if (request.Path.StartsWith("/files/")) {
      if (filesDirectory is not null) {
        string filePath = filesDirectory + "/" + request.Path.Replace("/files/", "");
        if (!File.Exists(filePath)) {
          response = new(HttpStatusCode.NotFound) {
            Content = new StringContent("File not found")
          };
        } else {
          response = new(HttpStatusCode.OK) {
            Content = new StreamContent(File.OpenRead(filePath))
          };
        }
      } else {
        response = new(HttpStatusCode.BadRequest) {
          Content = new StringContent("Missing file name parameter")
        };
      }
    } else {
      response = new(HttpStatusCode.NotFound) { };
    }
    return response;
  }

  private static HttpResponseMessage HandlePostRequest(Request request, string? filesDirectory) {
    HttpResponseMessage response;
    if (request.Path.StartsWith("/files/")) {
      if (filesDirectory is not null) {
        string contentWithoutTrailingNulls = request.Content.TrimEnd('\0');
        byte[] fileContent = Encoding.UTF8.GetBytes(contentWithoutTrailingNulls);
        string filePath = filesDirectory + "/" + request.Path.Replace("/files/", "");
        File.WriteAllBytes(filePath, fileContent);
        response = new(HttpStatusCode.Created) {
          Content = new StringContent($"File created at {filePath}")
        };
      } else {
        response = new(HttpStatusCode.BadRequest) {
          Content = new StringContent("Missing file name parameter")
        };
      }
    } else {
      Console.WriteLine("Path not found");
      response = new(HttpStatusCode.NotFound) {
        Content = new StringContent("Path not found")
      };
    }
    return response;
  }
}