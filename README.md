# Bank Sample: C#
A sample of bank implementation using Token Integration SDK.

Responds to TokenOS bank API requests with static data;
alter the model to integrate with bank systems.


IMPORTANT
=========
Regenerate certificates and keys before deploying this in a production environment.

This [test](test/GenerateKeyTest.cs) can be used to generate a new key pair for signing.

For ssl certificates, see `config/tls/README.md` for details.

Server
=======

Config
------
TokenOS and the bank use TLS for secure communication. Setting this up
involves generating and sharing cryptographic secrets.
These secrets are located in the `config/tls` directory.
`config/tls/README.md` has details.

The server responds to requests with static data.
This static data is configured in the `config/application.conf` file.

## Requirements
### On Windows

There are no prerequisites for Windows.

### On Linux and OSX

Install `Mono` from [here](https://www.mono-project.com/download/stable/).

 `Mono` is an open source implementation of Microsoft's .NET Framework. It brings the .NET framework to non-Windows envrionments like Linux and OSX.
 
Build and Run
------

To build the server run the command specified below. The server uses
gradle build tool.

```sh
nuget restore

msbuild
```

Run
------

```sh
dotnet run --ssl
```
