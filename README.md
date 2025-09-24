# NETWORG.DevToys.Tools

A collection of tools for [DevToys](https://devtoys.app/).

## Installation

1. [Download DevToys](https://devtoys.app/download) if you haven't already
1. Download the latest release of NETWORG.DevToys.Tools from [Nuget](https://www.nuget.org/packages/NETWORG.DevToys.Tools)
1. Install the extension via DevTools > *Manage Extensions* > *Install an extension*

## Tools

### JSON Value Escape / Unescape

Escape or unescape JSON string values. Based on [DevToys-app/DevToys#877](https://github.com/DevToys-app/DevToys/issues/877)

## Contributing

In order to debug, you can follow [the official guide](https://devtoys.app/doc/articles/extension-development/getting-started/debug-an-extension.html?tabs=vs). The current `launchSettings.json` however expect the executables to be in `temp/cli` and `temp/gui` folders (you can obtain those from [here](https://devtoys.app/download) - use the portable version).