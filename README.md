# HTTP Server

This is a HTTP server that's capable of handling simple GET/POST requests, serving files and handling multiple concurrent connections.

The server listens on port 4221.

It accepts requests of types:

1. `GET /echo/<a-random-string>` and responds with `200 OK response with <a-random-string>`.
2. `GET /files/<filename>` and responds an `application/octet-stream` with the contents of the file as the body if the file exists. Otherwise, it returns a `404 Not Found response`.
3. `POST /files/<filename>` and responds with `201 Created` an the file is created with the content of the body of the request.
