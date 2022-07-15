[import map]: https://deno.land/manual/linking_to_external_code/import_maps

# Fable.Deno

This repository offers bindings for the Deno API and some bindings for the std lib.

## Import Maps

It is recommended to use an [import map] as deno supports them and allow you to customize the way you import dependencies for deno, you can include one using the `deno.json` configuration file on your project's root or by using the CLI

```json
{
  "tasks": {
    "start": "deno run -A --watch=dist/ ./dist/Program.js"
  },
  "importMap": "./import_map.json"
}
```

### Standard Library

Standard library bindings are imported with the following convention

- `fable-deno-*` where `*` is the name of the standard library module being imported

these are a few examples:

- `fable-deno-http`
- `fable-deno-fs`
- `fable-deno-streams`
- `fable-deno-io`

This is to prevent potential collisions with third party bindings, and also means that you have to provide an import map that satisfies the import conditions, for example to be able to import those modules you'd need an import map like this

```json
{
  "imports": {
    "fable-deno-http": "https://deno.land/std/http/mod.ts",
    "fable-deno-fs": "https://deno.land/std/fs/mod.ts",
    "fable-deno-streams": "https://deno.land/std/streams/mod.ts",
    "fable-deno-io": "https://deno.land/std/io/mod.ts"
  }
}
```

if a binding is broken on a newer deno release you can pin the URL to the version the binding still works untill the bindings are updated.

Example: `"fable-deno-http": "https://deno.land/std@0.148.0/http/mod.ts"`

> **Note**: Please keep in mind that some imports don't provide a `mod.ts` file so you will have to change the imports to make it work

### Available Modules

- [x] archive
  - [x] Tar
  - [x] UnTar
  - [x] TarEntry
- [x] async
  - [x] DeadlineError
  - [x] MuxAsyncIterator
  - [x] ERROR_WHILE_MAPPING_MESSAGE
  - [x] DeadlineError
  - [x] MuxAsyncIterator
  - [x] ERROR_WHILE_MAPPING_MESSAGE
  - [x] abortable
  - [x] abortableAsyncIterable
  - [x] abortablePromise
  - [x] deadline
  - [x] debounce
  - [x] deferred
  - [x] delay
  - [x] pooledMap
- [x] bytes
  - [x] concat
  - [x] copy
  - [x] startsWith
  - [x] endsWith
  - [x] equals
  - [x] includesNeedle
  - [x] indexOfNeedle
  - [x] lastIndexOfNeedle
  - [x] repeat
- [ ] collections
- [ ] crypto
- [ ] datetime
- [ ] dotenv
- [ ] encoding
- [ ] examples
- [ ] flags
- [ ] fmt
- [ ] fs
- [ ] hash
- [ ] http
  - [x] HttpError
  - [x] Server
  - [x] Status
  - [x] errors
  - [x] STATUS_TEXT
  - [x] accepts
  - [x] acceptsEncodings
  - [x] acceptsLanguages
  - [x] createHttpError
  - [x] deleteCookie
  - [x] getCookies
  - [x] isClientErrorStatus
  - [x] isErrorStatus
  - [x] isHttpError
  - [x] isInformationalStatus
  - [x] isRedirectStatus
  - [x] isServerErrorStatus
  - [x] isSuccessfulStatus
  - [x] serve
  - [x] serveListener
  - [x] serveTls
  - [x] setCookie
  - [x] ConnInfo
  - [x] Cookie
  - [x] HttpErrorOptions
  - [x] ServeInit
  - [x] ServerInit
  - [x] ServeTlsInit
- [ ] io
- [ ] log
- [ ] media_types
- [ ] node
- [ ] path
- [ ] permissions
- [ ] signal
- [ ] streams
- [ ] textproto
- [x] uuid
  - [x] NIL_UUID
  - [x] isNil
  - [x] V1Options
  - [x] v1.generate
  - [x] v1.validate
  - [x] v4.validate
  - [x] v5.generate
  - [x] v5.validate
- [ ] wasi
- [ ] version.ts
