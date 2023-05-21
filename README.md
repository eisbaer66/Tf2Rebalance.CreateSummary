[![c#](https://img.shields.io/badge/language-C%23-%23178600)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![image](https://raw.githubusercontent.com/ZacharyPatten/ZacharyPatten/main/Resources/github-repo-checklist/opens-with-visual-studio-badge.svg)](https://visualstudio.microsoft.com/downloads/)
[![dotnet6](https://img.shields.io/badge/dynamic/xml?color=%23512bd4&label=target&query=%2F%2FTargetFramework%5B1%5D&url=https%3A%2F%2Fraw.githubusercontent.com%2Feisbaer66%2FTf2Rebalance.CreateSummary%2Fmaster%2FTf2Rebalance.CreateSummary%2FTf2Rebalance.CreateSummary.csproj)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![license](https://img.shields.io/badge/license-AGPL--3.0-blue)](https://github.com/eisbaer66/Tf2Rebalance.CreateSummary/blob/main/COPYING)

# Tf2Rebalance.CreateSummary

This CLI tool is used to create summaries for [Rebalanced Fortress 2](https://github.com/JugadorXEI/tf2rebalance_jug) (tf2rebalance_attributes.txt) and [TF2 Custom Attributes](https://github.com/nosoop/SM-TFCustAttr) (tf_custom_attributes.txt).


## Installation
### Prerequisites
Make sure to install [.NET Runtime](https://dotnet.microsoft.com/en-us/download) (minimum version 6.0)

### Setup
1. download the latest zip file matching your operation system from [Releases](https://github.com/eisbaer66/Tf2Rebalance.CreateSummary/releases)
2. extract the zip into a directory

### Build from source
Make sure you have the [.NET SDK](https://dotnet.microsoft.com/en-us/download) installed (minimum version 6.0).  
You can use VisualStudio to build/run.  
Or you can use the dotnet-cli:  
```
git clone https://github.com/eisbaer66/Tf2Rebalance.CreateSummary.git
cd Tf2Rebalance.CreateSummary/Tf2Rebalance.CreateSummary
dotnet run
```


## Usage
```
Creates Summaries from tf2rebalance_attributes.txt files.
try 'Tf2Rebalance.CreateSummary -f:rtf "C:\Path to\tf2rebalance_attributes.txt"'

Usage: Tf2Rebalance.CreateSummary [options] <Files>

Arguments:
  Files         Enter the filenames (i.e. tf2rebalance_attributes.txt)

Options:
  -?|-h|--help  Show help information.
  -f|--format   Output format: Rtf, Text, Json or GroupedJson. Defaults to Rtf
                Allowed values are: Rft, Text, Json, GroupedJson.
                Default value is: Rft.
  -o|--output   Output directory: specify output directory for summaries. Defaults to the directory of the input-file
```


# License
Copyright (C) 2023 icebear <icebear@icebear.rocks>

Tf2Rebalance.CreateSummary is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Tf2Rebalance.CreateSummary is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License along with Tf2Rebalance.CreateSummary. If not, see <https://www.gnu.org/licenses/>. 