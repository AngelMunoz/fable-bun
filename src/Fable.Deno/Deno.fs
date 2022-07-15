namespace Fable.Deno

open System.Collections.Generic
open Fable.Core

open Fetch

open Browser.Types
open System

[<AbstractClass; Erase>]
type Indexable =

    [<Emit("$0[$1]")>]
    member _.Item(key: string) : obj = jsNative

[<AbstractClass; Erase>]
type StringMap =

    [<Emit("$0[$1]")>]
    member _.Item(string: string) : string = jsNative

type NetAddr =
    abstract hostname: string
    abstract port: int
    abstract transport: string

type UnixAddr =
    abstract path: string
    abstract transport: string

type DenoAddr = U2<NetAddr, UnixAddr>

type ReadableStream<'T> =
    interface
    end

type WritableStreamDefaultWriter<'T> =
    abstract closed: bool
    abstract desiredSize: int option
    abstract ready: JS.Promise<unit>
    abstract abort: obj -> unit
    abstract close: unit -> JS.Promise<unit>
    abstract releaseLock: unit -> unit
    abstract write: 'T -> JS.Promise<unit>

type WritableStream<'T> =
    abstract locked: bool
    abstract abort: obj -> unit
    abstract close: unit -> JS.Promise<unit>
    abstract getWriter: unit -> WritableStreamDefaultWriter<'T>


type Conn =
    abstract localAddr: DenoAddr
    abstract readable: ReadableStream<JS.Uint8Array>
    abstract remoteAddr: DenoAddr
    abstract rid: int
    abstract writable: WritableStream<JS.Uint8Array>
    abstract closeWrite: unit -> JS.Promise<unit>

type TlsHandshakeInfo =
    interface
    end

type TlsConn =
    inherit Conn
    abstract handshake: unit -> JS.Promise<TlsHandshakeInfo>

type ListenOptions =
    abstract hostname: string option
    abstract port: int

type Listener =
    inherit IAsyncEnumerable<Conn>
    abstract addr: DenoAddr
    abstract rid: int
    abstract accept: unit -> JS.Promise<unit>
    abstract close: unit -> unit

type TlsListener =
    inherit Listener
    inherit IAsyncEnumerable<TlsConn>
    abstract accept: JS.Promise<TlsConn>

type RequestEvent =
    abstract request: Request
    abstract respondWith: U2<Response, JS.Promise<Response>> -> JS.Promise<unit>

type HttpConn =
    inherit IAsyncEnumerable<RequestEvent>
    abstract rid: int
    abstract close: unit -> unit
    abstract nextRequest: unit -> JS.Promise<RequestEvent option>

type ServerInit =
    inherit ListenOptions

    abstract handler: Request -> U2<Response, JS.Promise<Response>>
    abstract onError: (exn -> U2<Response, JS.Promise<Response>>) option

type OnListenParams =
    abstract hostname: string
    abstract port: int

type ConnInfo =
    abstract localAddr: DenoAddr
    abstract remoteAddr: DenoAddr

[<StringEnum; RequireQualifiedAccess>]
type Signal =
    | [<CompiledName("SIGABRT")>] SIGABRT
    | [<CompiledName("SIGALRM")>] SIGALRM
    | [<CompiledName("SIGBREAK")>] SIGBREAK
    | [<CompiledName("SIGBUS")>] SIGBUS
    | [<CompiledName("SIGCHLD")>] SIGCHLD
    | [<CompiledName("SIGCONT")>] SIGCONT
    | [<CompiledName("SIGEMT")>] SIGEMT
    | [<CompiledName("SIGFPE")>] SIGFPE
    | [<CompiledName("SIGHUP")>] SIGHUP
    | [<CompiledName("SIGILL")>] SIGILL
    | [<CompiledName("SIGINFO")>] SIGINFO
    | [<CompiledName("SIGINT")>] SIGINT
    | [<CompiledName("SIGIO")>] SIGIO
    | [<CompiledName("SIGKILL")>] SIGKILL
    | [<CompiledName("SIGPIPE")>] SIGPIPE
    | [<CompiledName("SIGPROF")>] SIGPROF
    | [<CompiledName("SIGPWR")>] SIGPWR
    | [<CompiledName("SIGQUIT")>] SIGQUIT
    | [<CompiledName("SIGSEGV")>] SIGSEGV
    | [<CompiledName("SIGSTKFLT")>] SIGSTKFLT
    | [<CompiledName("SIGSTOP")>] SIGSTOP
    | [<CompiledName("SIGSYS")>] SIGSYS
    | [<CompiledName("SIGTERM")>] SIGTERM
    | [<CompiledName("SIGTRAP")>] SIGTRAP
    | [<CompiledName("SIGTSTP")>] SIGTSTP
    | [<CompiledName("SIGTTIN")>] SIGTTIN
    | [<CompiledName("SIGTTOU")>] SIGTTOU
    | [<CompiledName("SIGURG")>] SIGURG
    | [<CompiledName("SIGUSR1")>] SIGUSR1
    | [<CompiledName("SIGUSR2")>] SIGUSR2
    | [<CompiledName("SIGVTALRM")>] SIGVTALRM
    | [<CompiledName("SIGWINCH")>] SIGWINCH
    | [<CompiledName("SIGXCPU")>] SIGXCPU
    | [<CompiledName("SIGXFSZ")>] SIGXFSZ

type ConnectOptions =
    abstract hostname: string option
    abstract port: int
    abstract transport: string

type ConnectTlsOptions =
    abstract caCerts: string[] option
    abstract certFile: string option
    abstract hostname: string option
    abstract port: int option

type TcpConn =
    inherit Conn
    abstract setKeepAlive: bool option -> unit
    abstract setNoDelay: bool option -> unit

type SeekMode =
    | Start = 0
    | Current = 1
    | End = 2

type Reader =
    abstract read: JS.Uint8Array -> JS.Promise<int option>

type ReaderSync =
    abstract readSync: JS.Uint8Array -> int option

type Writer =
    abstract write: JS.Uint8Array -> JS.Promise<int>

type WriterSync =
    abstract writeSync: JS.Uint8Array -> int

type Seeker =
    abstract seek: (int * SeekMode) -> JS.Promise<int>

type SeekerSync =
    abstract seekSync: (int * SeekMode) -> int

type Closer =
    abstract close: unit -> unit

type FileInfo =
    abstract atime: DateTime option
    abstract birthtime: DateTime option
    abstract blksize: int option
    abstract blocks: int option
    abstract dev: int option
    abstract gid: int option
    abstract ino: int option
    abstract isDirectory: bool
    abstract isFile: bool
    abstract isSymlink: bool
    abstract mode: int option
    abstract mtime: DateTime option
    abstract nlink: int option
    abstract rdev: int option
    abstract size: int
    abstract uid: int option

type FsFile =
    inherit Reader
    inherit ReaderSync
    inherit Writer
    inherit WriterSync
    inherit Seeker
    inherit SeekerSync
    inherit Closer

    abstract readable: ReadableStream<JS.Uint8Array>
    abstract rid: int
    abstract writable: WritableStream<JS.Uint8Array>


    abstract close: unit -> unit
    abstract read: JS.Uint8Array -> JS.Promise<int option>
    abstract readSync: JS.Uint8Array -> int option
    abstract seek: (int * SeekMode) -> JS.Promise<int>
    abstract seek: (int * int * SeekMode) -> JS.Promise<int>
    abstract seekSync: (int * SeekMode) -> int
    abstract seekSync: (int * int * SeekMode) -> int
    abstract stat: unit -> JS.Promise<FileInfo>
    abstract statSync: unit -> FileInfo
    abstract truncate: int option -> JS.Promise<unit>
    abstract truncateSync: int option -> JS.Promise<unit>
    abstract write: JS.Uint8Array -> JS.Promise<int>
    abstract writeSync: JS.Uint8Array -> int

type InspectOptions =
    abstract colors: bool option
    abstract compact: bool option
    abstract depth: int option
    abstract getters: bool option
    abstract iterableLimit: int option
    abstract showHidden: bool option
    abstract showProxy: bool option
    abstract sorted: bool option
    abstract strAbbreviateSize: int option
    abstract trailingComma: bool option

type ListenOptionsWithTransport =
    inherit ListenOptions
    abstract transport: string option

type ListenTlsOptions =
    inherit ListenOptions

    abstract cert: string option
    abstract certFile: string option
    abstract key: string option
    abstract keyFile: string option
    abstract transport: string option

type MakeTempOptions =
    abstract dir: string option
    abstract prefix: string option
    abstract suffix: string option

type MemoryUsage =
    abstract external: float
    abstract heapTotal: float
    abstract heapUsed: float
    abstract rss: float

type OpMetrics =
    abstract bytesReceived: float
    abstract bytesSentControl: float
    abstract bytesSentData: float
    abstract opsCompleted: float
    abstract opsCompletedAsync: float
    abstract opsCompletedAsyncUnref: float
    abstract opsCompletedSync: float
    abstract opsDispatched: float
    abstract opsDispatchedAsync: float
    abstract opsDispatchedAsyncUnref: float
    abstract opsDispatchedSync: float

type Metrics =
    inherit OpMetrics
    abstract ops: obj

type MkdirOptions =
    abstract mode: int option
    abstract recursive: bool option

type OpenOptions =
    abstract append: bool option
    abstract create: bool option
    abstract createNew: bool option
    abstract mode: int option
    abstract read: bool option
    abstract truncate: bool option
    abstract write: bool option

type DirEntry =
    abstract isDirectory: bool
    abstract isFile: bool
    abstract isSymlink: bool
    abstract name: string

type ReadFileOptions =
    abstract signal: AbortSignal option

type RemoveOptions =
    abstract recursive: bool option

[<StringEnum; RequireQualifiedAccess>]
type DnsRecordType =
    | [<CompiledName("A")>] A
    | [<CompiledName("AAAA")>] AAAA
    | [<CompiledName("ANAME")>] ANAME
    | [<CompiledName("CNAME")>] CNAME
    | [<CompiledName("NS")>] NS
    | [<CompiledName("PTR")>] PTR

type NameServer =
    abstract ipAddr: string
    abstract port: int option

type ResolveDnsOptions =
    abstract nameServer: NameServer option

type ResourceMap =
    interface
    end

type ProcessStatus =
    abstract success: bool
    abstract code: int
    abstract signal: Signal option

[<Erase; RequireQualifiedAccess>]
type StdioRun =
    | [<CompiledName("inherit")>] Inherit
    | [<CompiledName("piped")>] Piped
    | [<CompiledName("NULL")>] Null
    | Number of int

type RunOptions =
    abstract cmd: U2<string[], URL[]>
    abstract cwd: string option
    abstract env: obj option
    abstract stderr: StdioRun option
    abstract stdin: StdioRun option
    abstract stdout: StdioRun option

type Process<'T> =
    abstract pid: int
    abstract rid: int
    abstract stderr: ReadableStream<JS.Uint8Array> option
    abstract stdin: ReadableStream<JS.Uint8Array> option
    abstract stdout: ReadableStream<JS.Uint8Array> option
    abstract close: unit -> unit
    abstract kill: Signal -> unit
    abstract output: unit -> JS.Promise<JS.Uint8Array>
    abstract status: unit -> JS.Promise<ProcessStatus>
    abstract stderrOutput: unit -> JS.Promise<JS.Uint8Array>

type StartTlsOptions =
    abstract caCerts: string[] option
    abstract hostname: string option

[<StringEnum; RequireQualifiedAccess>]
type SymlinkType =
    | File
    | Dir

type SymlinkOptions =
    abstract ``type``: SymlinkType

type PermissionOptionsObject =
    abstract env: U3<string, bool, string[]> option
    abstract ffi: U3<string, bool, U2<string, URL>[]> option
    abstract hrtime: U2<string, bool> option
    abstract net: U3<string, bool, string[]> option
    abstract read: U3<string, bool, U2<string, URL>[]> option
    abstract run: U3<string, bool, U2<string, URL>[]> option
    abstract write: U3<string, bool, U2<string, URL>[]> option


type PermissionOptions = U2<string, PermissionOptionsObject>

type TestFn = TestContext -> U2<unit, JS.Promise<unit>>

and TestStepDefinition =
    abstract fn: TestFn
    abstract ignore: bool option
    abstract name: string
    abstract sanitizeExit: bool option
    abstract sanitizeOps: bool option
    abstract sanitizeResources: bool option

and TestContext =
    abstract name: string
    abstract origin: string
    abstract parent: TestContext option
    abstract step: TestStepDefinition -> JS.Promise<bool>
    abstract step: (string * TestFn) -> JS.Promise<bool>

type TestDefinition =
    abstract fn: TestFn
    abstract ignore: bool option
    abstract name: string
    abstract only: bool option
    abstract permissions: PermissionOptions option
    abstract sanitizeExit: bool option
    abstract sanitizeOps: bool option
    abstract sanitizeResources: bool option

type UpgradeWebSocketOptions =
    abstract idleTimeout: float option
    abstract protocol: string option

type WebSocketUpgrade =
    abstract response: Response
    abstract socket: WebSocket

type WatchOptions =
    abstract recursive: bool


[<StringEnum; RequireQualifiedAccess>]
type FsEventKind =
    | Any
    | Access
    | Create
    | Modify
    | Remove
    | Other

type FsEvent =
    abstract flag: string option
    abstract kind: FsEventKind
    abstract paths: string[]

type FsWatcher =
    inherit IAsyncEnumerable<FsEvent>

    abstract rid: int
    abstract close: unit -> unit
    abstract ``return``: (obj option) -> JS.Promise<obj>

type WriteFileOptions =
    abstract append: bool option
    abstract create: bool option
    abstract mode: int option
    abstract signal: AbortSignal option

[<Global; Erase>]
type Deno =
    static member addSignalListener(signal: Signal, handler: unit -> unit) : unit = jsNative
    static member chdir(directory: U2<string, URL>) : unit = jsNative
    static member chmod(directory: U2<string, URL>, mode: int) : JS.Promise<unit> = jsNative
    static member chmodSync(directory: U2<string, URL>, mode: int) : unit = jsNative
    static member chown(directory: U2<string, URL>, ?uid: int, ?gid: int) : JS.Promise<unit> = jsNative
    static member close(rid: int) : unit = jsNative
    static member connect(connectOptions: ConnectOptions) : JS.Promise<TcpConn> = jsNative
    static member connectTls(connectOptions: ConnectTlsOptions) : JS.Promise<TlsConn> = jsNative
    static member copyFile(from: U2<string, URL>, toPath: U2<string, URL>) : JS.Promise<unit> = jsNative
    static member copyFileSync(from: U2<string, URL>, toPath: U2<string, URL>) : unit = jsNative
    static member create(path: U2<string, URL>) : JS.Promise<FsFile> = jsNative
    static member createSync(path: U2<string, URL>) : FsFile = jsNative
    static member cwd() : string = jsNative
    static member execPath() : string = jsNative
    static member exit(?code: int) : unit = jsNative
    static member fdatasync(rid: int) : JS.Promise<unit> = jsNative
    static member fdatasyncSync(rid: int) : unit = jsNative
    static member fstat(rid: int) : JS.Promise<FileInfo> = jsNative
    static member fstatSync(rid: int) : FileInfo = jsNative
    static member fsync(rid: int) : JS.Promise<unit> = jsNative
    static member fsyncSync(rid: int) : unit = jsNative
    static member ftruncate(rid: int, ?len: int) : JS.Promise<unit> = jsNative
    static member ftruncateSync(rid: int, ?len: int) : unit = jsNative
    static member inspect(value: obj, ?options: InspectOptions) : string = jsNative
    static member isatty(rid: int) : bool = jsNative
    static member kill(pid: int, signal: Signal) : unit = jsNative
    static member link(oldPath: string, newPath: string) : JS.Promise<unit> = jsNative
    static member linkSync(oldPath: string, newPath: string) : unit = jsNative
    static member listen(options: ListenOptionsWithTransport) : Listener = jsNative
    static member listenTls(options: ListenTlsOptions) : TlsListener = jsNative
    static member lstat(path: U2<string, URL>) : JS.Promise<FileInfo> = jsNative
    static member lstatSync(path: U2<string, URL>) : FileInfo = jsNative
    static member makeTempDir(?options: MakeTempOptions) : JS.Promise<string> = jsNative
    static member makeTempDirSync(?options: MakeTempOptions) : string = jsNative
    static member makeTempFile(?options: MakeTempOptions) : JS.Promise<string> = jsNative
    static member makeTempFileSync(?options: MakeTempOptions) : string = jsNative
    static member memoryUsage() : MemoryUsage = jsNative
    static member metrics() : Metrics = jsNative
    static member mkdir(path: U2<string, URL>, ?options: MkdirOptions) : JS.Promise<unit> = jsNative
    static member mkdirSync(path: U2<string, URL>, ?options: MkdirOptions) : unit = jsNative

    [<Emit("Deno.open($0...)")>]
    static member Open(path: U2<string, URL>, ?options: OpenOptions) : JS.Promise<FsFile> = jsNative

    static member openSync(path: U2<string, URL>, ?options: OpenOptions) : FsFile = jsNative
    static member read(rid: int, buffer: JS.Uint8Array) : JS.Promise<int option> = jsNative
    static member readDir(path: U2<string, URL>) : IAsyncEnumerable<DirEntry> = jsNative
    static member readDirSync(path: U2<string, URL>) : IEnumerable<DirEntry> = jsNative
    static member readFile(path: U2<string, URL>, ?options: ReadFileOptions) : JS.Promise<JS.Uint8Array> = jsNative
    static member readFileSync(path: U2<string, URL>, ?options: ReadFileOptions) : JS.Uint8Array = jsNative
    static member readLink(path: U2<string, URL>) : JS.Promise<string> = jsNative
    static member readLinkSync(path: U2<string, URL>) : string = jsNative
    static member readSync(rid: int, buffer: JS.Uint8Array) : int option = jsNative
    static member readTextFile(path: U2<string, URL>, ?options: ReadFileOptions) : JS.Promise<string> = jsNative
    static member readTextFileSync(path: U2<string, URL>, ?options: ReadFileOptions) : string = jsNative
    static member realPath(path: U2<string, URL>) : JS.Promise<string> = jsNative
    static member realPathSync(path: U2<string, URL>) : string = jsNative
    static member remove(path: U2<string, URL>, ?options: RemoveOptions) : JS.Promise<unit> = jsNative
    static member removeSync(path: U2<string, URL>, ?options: RemoveOptions) : unit = jsNative
    static member removeSignalListener(signal: Signal, listener: unit -> unit) : unit = jsNative
    static member rename(oldPath: U2<string, URL>, newPath: U2<string, URL>) : JS.Promise<unit> = jsNative
    static member renameSync(oldPath: U2<string, URL>, newPath: U2<string, URL>) : unit = jsNative

    static member resolveDns
        (
            query: string,
            recordType: DnsRecordType,
            ?options: ResolveDnsOptions
        ) : JS.Promise<string[]> =
        jsNative

    static member resources() : ResourceMap = jsNative
    static member run<'T when 'T :> RunOptions>(options: 'T) : Process<'T> = jsNative
    static member serveHttp(conn: Conn) : HttpConn = jsNative
    static member shutdown(rid: int) : JS.Promise<unit> = jsNative
    static member startTls(conn: Conn, ?options: StartTlsOptions) : JS.Promise<TlsConn> = jsNative
    static member stat(path: U2<string, URL>) : JS.Promise<FileInfo> = jsNative
    static member statSync(path: U2<string, URL>) : FileInfo = jsNative

    static member symlink
        (
            oldPath: U2<string, URL>,
            newPath: U2<string, URL>,
            ?options: SymlinkOptions
        ) : JS.Promise<unit> =
        jsNative

    static member symlinkSync(oldPath: U2<string, URL>, newPath: U2<string, URL>, ?options: SymlinkOptions) : unit =
        jsNative

    static member test(t: TestDefinition) : unit = jsNative
    static member test(name: string, fn: TestFn) : unit = jsNative
    static member test(fn: TestFn) : unit = jsNative
    static member test(name: string, options: TestDefinition, fn: TestFn) : unit = jsNative
    static member test(options: TestDefinition, fn: TestFn) : unit = jsNative
    static member truncate(name: string, ?len: int) : JS.Promise<unit> = jsNative
    static member truncateSync(name: string, ?len: int) : unit = jsNative
    static member upgradeWebSocket(request: Request, ?options: UpgradeWebSocketOptions) : WebSocketUpgrade = jsNative
    static member write(rid: int, data: JS.Uint8Array) : JS.Promise<int> = jsNative
    static member writeSync(rid: int, data: JS.Uint8Array) : int = jsNative

    static member writeFile(path: U2<string, URL>, data: JS.Uint8Array, ?options: WriteFileOptions) : JS.Promise<unit> =
        jsNative

    static member writeFileSync(path: U2<string, URL>, data: JS.Uint8Array, ?options: WriteFileOptions) : unit =
        jsNative

    static member writeTextFile(path: U2<string, URL>, data: string, ?options: WriteFileOptions) : JS.Promise<unit> =
        jsNative

    static member writeTextFileSync(path: U2<string, URL>, data: string, ?options: WriteFileOptions) : unit = jsNative
    static member watchFs(paths: U2<string, string[]>, ?options: WatchOptions) : FsWatcher = jsNative
