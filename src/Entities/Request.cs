using System.Text;

namespace http.server.entities;

public class Request {
  public string Method { get; set; } = "";
  public string Path { get; set; } = "";
  public string Version { get; set; } = "";
  public Dictionary<string, string> Headers { get; set; } = new();
  public string Content { get; set; } = "";

  public static Request Parse(byte[] requestBytes) {
    string requestString = Encoding.UTF8.GetString(requestBytes);
    string[] lines = requestString.Split("\r\n");
    string[] startLine = lines[0].Split(" ");

    Request request = new() {
      Method = startLine[0],
      Path = startLine[1],
      Version = startLine[2]
    };

    int emptyLineIndex = Array.IndexOf(lines, "");
    for (int i = 1; i < emptyLineIndex; i++) {
      string[] header = lines[i].Split(": ");
      if (header.Length == 2) {
        request.Headers.Add(header[0], header[1]);
      }
    }
    if (emptyLineIndex < lines.Length - 1) {
      request.Content = lines[emptyLineIndex + 1];
    }

    return request;
  }
}