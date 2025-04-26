# Simple TCP Web Server

## Description

A basic web server built with TCP sockets in C#. It serves static `.html`, `.css`, and `.js` files from a `webroot/` folder and handles simple HTTP `GET` requests. Supports error handling, request logging, and concurrent client connections.

---

## Features

- Serve static files (`.html`, `.css`, `.js`).
- Handle multiple clients using ThreadPool.
- Proper HTTP status codes: `200`, `403`, `404`, `405`.
- Log client IPs and requests.
- Serve custom error page (`error.html`).
- Basic MIME type support.

---

## Folder Structure

```
/webroot
  ├── index.html
  ├── about.html
  ├── styles.css
  ├── script.js
  ├── error.html
server_log.txt
Program.cs
```

---

## Notes

- Only serves files inside `webroot/`.
- Directory traversal is prevented.
- Logs saved in `server_log.txt`.

---

## Git Workflow

- Commit at project start.
- Commit after each major feature.
- Push regularly to GitHub.
