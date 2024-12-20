#!/usr/bin/env node
// @flow

'use strict';

const spawn = require('child_process').spawn;
const exec = require('child_process').execSync;
const http = require('http');
const util = require('util');
const path = require('path');
const fs = require('fs');

const port = process.env.PORT | 5000;
const host = `http://127.0.0.1:${port}`;
const serveDir = path.resolve(process.argv[2] || process.cwd());

const filesMimeTypesCache = {};
function getMimeType(filepath) {
  if (!filesMimeTypesCache[filepath]) {
    const splitted_path = filepath.replace(/\\/g, '/').split('/');
    switch (splitted_path[splitted_path.length - 1]) {
      case 'index.html':
      case 'index-worker.html':
      case 'index-mainthread.html':
      case 'help.html':
      case 'credits.html':
        filesMimeTypesCache[filepath] = 'text/html';
        break;
      case 'BasiliskII-worker-ui-thread.js':
      case 'BasiliskII-worker-boot.js':
      case 'BasiliskII.js':
      case 'BasiliskII-mainthread-boot.js':
        filesMimeTypesCache[filepath] = 'application/javascript';
        break;
      case 'index.css':
      case 'base.css':
      case 'credits.css':
      case 'help.css':
      case 'spinner.css':
        filesMimeTypesCache[filepath] = 'text/css';
        break;
      case 'garamond.ttf':
      case 'oswald.ttf':
        filesMimeTypesCache[filepath] = 'font/sfnt';
        break;
      case 'disk':
        filesMimeTypesCache[filepath] = 'application/x-apple-diskimage';
        break;
      case 'rom':
        filesMimeTypesCache[filepath] = 'application/octet-stream';
        break;
      case 'prefs':
        filesMimeTypesCache[filepath] = 'text/plain';
        break;
      case 'favicon.ico':
        filesMimeTypesCache[filepath] = 'image/ico';
        break;
      default:
        filesMimeTypesCache[filepath] = 'text/plain';
        break;
    }
  }
  return filesMimeTypesCache[filepath];
}

function handler(req, res) {
  const reqPath = req.url.replace(/\?.*/, '').replace(/_cb.*/, '');
  const reqPathFSPath = path.join(serveDir, reqPath);

  function errRes(err, code) {
    console.log(`${code} ${req.url} ${err}`);
    res.writeHead(code, {'Content-Type': 'text/plain'});
    res.write(err.stack);
    res.end();
  }

  // does the request point to a valid file or dir at all?
  let reqPathStat = null;
  try {
    reqPathStat = fs.lstatSync(reqPathFSPath);
  } catch (reqPathStatErr) {
    // nothing there
    return errRes(reqPathStatErr, 404);
  }

  try {
    // return file or index file contents
    const filepath = reqPathStat.isDirectory()
      ? path.join(reqPathFSPath, 'index.html')
      : reqPathFSPath;
    const mimeType = getMimeType(filepath);
    const contents = fs.readFileSync(filepath);
    console.log(`200 ${req.url} ${mimeType}`);
    res.setHeader('Cross-Origin-Opener-Policy', 'same-origin');
    res.setHeader('Cross-Origin-Embedder-Policy', 'require-corp');
    res.writeHead(200, {'Content-Type': mimeType});
    res.write(contents);
    res.end();
  } catch (fileReadErr) {
    if (reqPathStat.isDirectory()) {
      // render directory listing
      try {
        const filepath = path.join(serveDir, reqPath);
        const dirlinks = ['..', ...fs.readdirSync(filepath)]
          .map((file) => {
            const fileStat = fs.lstatSync(path.join(reqPathFSPath, file));
            const filename = fileStat.isDirectory() ? `${file}/` : file;

            return `<li><a href="${path.join(
              reqPath,
              filename
            )}">${filename}</a></li>`;
          })
          .join('\n');
        console.log(`200 ${req.url} [dir listing] 'text/html'`);
        res.writeHead(200, {'Content-Type': 'text/html'});
        res.write(`<!DOCTYPE html>
  <html>
  <head>
    <title>Directory listing of ${reqPath}</title>
  </head>
  <body>
  <h1>Directory listing of ${reqPath}</h1>
  <ul>${dirlinks}</ul>
  </body>
  </html>`);
        res.end();
        return;
      } catch (dirlistErr) {
        // directory listing failed somehow
        return errRes(dirlistErr, 500);
      }
    } else {
      // there was a file or dir but we couldn't read it
      return errRes(fileReadErr, 500);
    }
  }
}

const server = http.createServer(handler);
server.listen(port);

console.log(
  `
opening http://127.0.0.1:${port}/ in your browser

press CTRL-C to quit this program
`
);
