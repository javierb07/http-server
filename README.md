# HTTP Server

This is a HTTP server that's capable of handling simple GET/POST requests, serving files and handling multiple concurrent connections.

The server listens on port 4221.

It accepts requests of types:

1. `GET /` the root responds with `200 OK`
2. `GET /echo/<a-random-string>` and responds with `200 OK <a-random-string>` response.
3. `GET /user-agent` and responds with `200 OK <User-Agent>`.
4. `GET /files/<filename>` and responds an `application/octet-stream` with the contents of the file as the body if the file exists. Otherwise, it returns a `404 Not Found`.
5. `POST /files/<filename>` and responds with `201 Created` an the file is created with the content of the body of the request.
6. Any other requests return `404 Not Found`
