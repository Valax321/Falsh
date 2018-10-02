# Falsh
Falsh is a bash-like shell for Windows, although it should (in theory) work on macOS and Linux too. It uses .NET Core 2.1 and is written in C#.

It is intended to eventually be cross-platform, compiled to a native executable using [CoreRT](https://github.com/dotnet/corert). For now it must be run using `dotnet falsh.dll` so it is not the most convenient to use.

## Features
* Will execute any command line program and show its output. Currently it is not possible to write to stdin, but this will come soon.
* Built-in commands for directory changing (`cd` and `pwd`).
* WIP: command history. Persists between runs and has a configurable buffer size.
* WIP: glob for files/directories.
* WIP: UNIX-style pipes.

## Long-Term Goals
* Full bash-style scripting capabilities.

# License
Copyright (C) Andrew Castillo 2018
Released under the MIT License. See the LICENSE file for more information.