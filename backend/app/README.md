A private key that can be used for validation is generated.

- Address: `0x53103C2D7875D2f5f02AeC3075155e268a6e3A94`
- Passphrase: `11C25ECB4F3B5F8CB53A59A36DF15C67A192E14708016053D20367A51742B859`
- PrivateKey: `eda6ef63ae945cd15572fcf7d6635a8b3f8d86e85b57a353b482bc82c7fd2ad4`
- PublicKey: `034b83cb8ce52392ad9e46faf398f96c5cd7cdb95a9ea990a9a55cc575237d2b34`

----

Your Libplanet Node
===================

This is a blockchain node application project created using
[libplanet-templates] [.NET template].
It uses [Libplanet] at its core.

[libplanet-templates]: https://github.com/planetarium/libplanet-templates
[.NET template]: https://github.com/dotnet/templating/
[Libplanet]: https://libplanet.io/

Prerequisites
-------------

You need to install [.NET SDK] 6+. Follow the instruction to install
the .NET SDK on the [.NET download page][1].

[.NET SDK]: https://docs.microsoft.com/dotnet/core/sdk
[1]: https://dotnet.microsoft.com/download


Build
-----

```bash
$ dotnet build
```

If you want to build a docker image, You can create a standalone image
with the command below.
```bash
$ docker build . -t <IMAGE_TAG>
```

Create Your Local appsettings.json
---

Copy the File: First, copy Savor22b/appsettings.json to Savor22b/appsettings.local.json.
Modify Paths: Next, modify the paths for GenesisBlockPath, CsvDataResourcePath, etc., to match your local directory structure.

How to Run
----------

```bash
$ dotnet run --project Savor22b
```

### About configuration
By default, this project produces and uses storage and settings via
`appsettings.json` and `SAVOR22B_` prefixed environment variables. If you want to
change settings, please edit that files or set environment variables.

In sh/bash/zsh (Linux or macOS):

```sh
$ SAVOR22B_StorePath="/tmp/planet-node" dotnet run --project Savor22b
```

Or PowerShell (Windows):

```pwsh
PS > $Env:SAVOR22B_StorePath="/tmp/planet-node"; dotnet run --project Savor22b
```

### GraphQL
This project embeds a [GraphQL] server and [GraphQL Playground] by default,
backed by [GraphQL.NET]. You can check the current chain status on the
playground. (The playground is at <http://localhost:38080/ui/playground> by
default.)

To access the Libplanet explorer GraphQL queries, you would have to change the
endpoint to <http://localhost:38080/graphql/explorer>.

The following GraphQL query returns the last 10 blocks and transactions.

```graphql
query
{
  blockQuery
  {
    blocks (limit: 10 desc: true)
    {
      index
      hash
      timestamp

      transactions
      {
        id
        actions
        {
          inspection
        }
      }
    }
  }
}
```

Also, you can find a list of supported GraphQL query in the playground on the
sidebar.

See the [Libplanet.Explorer] project for more details.
Also, if you want to try scenario based tutorial, please check the
`TUTORIAL.md` file.

Publish
-------

If you want to pack this project, use [`dotnet publish`][dotnet publish] as below.

```bash
$ dotnet publish -c Release --self-contained -r linux-x64
$ ls -al Savor22b/bin/Release/net6.0/linux-x64/publish/
```

[dotnet publish]: https://docs.microsoft.com/en-US/dotnet/core/tools/dotnet-publish

[GraphQL]: https://graphql.org/
[GraphQL Playground]: https://github.com/graphql/graphql-playground
[GraphQL.NET]: https://graphql-dotnet.github.io/
[Libplanet.Explorer]: https://github.com/planetarium/libplanet/tree/main/Libplanet.Explorer

