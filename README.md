# PhantomMail 👻

Independently developed, simple CLI based email client.

Built principally with
- Terminal.Gui ([GitHub](https://github.com/migueldeicaza/gui.cs))
- [MailKit](https://www.mimekit.net) ([GitHub](https://github.com/jstedfast/MailKit))
- Microsft .NET Core [Configuration](https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration?tabs=command-line), [Hosting](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting?view=dotnet-plat-ext-6.0) and [Logging](https://docs.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line) Extensions
- [McMaster CommandLineUtils](https://natemcmaster.github.io/CommandLineUtils/docs/intro.html) and Hosting.CommandLine extensions
- [Serilog](https://serilog.net/) and Hosting/Configuration extensions

Not affiliated any other project, just using them.

Still in pre-alpha.

![image](https://user-images.githubusercontent.com/3766240/155912631-7904ef2c-e093-44a1-b4cb-9017721caf43.png)
![image](https://user-images.githubusercontent.com/3766240/156009196-c6164cdf-6de8-4704-85a5-1d0756f22365.png)

Also supports light and dark mode as well as custom themes, which are saved in the json settings file.

![image](https://user-images.githubusercontent.com/3766240/155921071-7648cb86-1dad-4ec9-b4fa-ca0d39a2a1af.png)

## Bonus - PhantomKit
- Library that makes encrypted json based settings vaults simple to use.
- Also provides simple code functions for various components of PhantomMail that you can break up, tweak, and re-use in your own variants.

## Documentation
- JSON File format (mostly human editable, except encrypted values- tamper evident): https://github.com/FreddieMercurial/PhantomMail/wiki/PhantomKit-Settings-Vault

## Note:
- Includes another copy of crc32 library cloned from [BrightChain](https://github.com/BrightChain/BrightChain) ([Source](https://github.com/BrightChain/BrightChain/blob/main/src/BrightChain.Engine/Helpers/Crc32.cs)), itself derived from stackoverflow and incrementally improved.
