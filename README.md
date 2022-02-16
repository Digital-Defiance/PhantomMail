# PhantomMail 👻

Independently developed, simple CLI based email client.

Built principally with
- [MailKit](https://www.mimekit.net) ([GitHub](https://github.com/jstedfast/MailKit))
- Gui.cs ([GitHub](https://github.com/migueldeicaza/gui.cs))
- [Spectre](https://spectreconsole.net/) ([GitHub](https://github.com/spectreconsole/spectre.console))

Not affiliated any other project, just using them.

Still in pre-alpha.

![image](https://user-images.githubusercontent.com/3766240/154818266-2796e91b-2240-4f48-95d3-9d37e76bfcc2.png)

This will look very similar to Gui.cs's demo for the moment as I'm ripping it apart and learning... but it goes from the vault password prompting to the full screen menu, then disposes of the MailClient instance.

![Screenshot 2022-02-23 173921](https://user-images.githubusercontent.com/3766240/155440848-744d6ebe-ca1d-49da-a606-7e1751e8d60a.png)

Also supports light and dark mode as well as custom themes, which are saved in the json settings file.

![Screenshot 2022-02-23 173942](https://user-images.githubusercontent.com/3766240/155440859-9e1e1af2-bdd9-42d9-8144-84c9758dfc3c.png)


## Bonus - PhantomKit
- Library that makes encrypted json based settings vaults simple to use.
- Also provides simple code functions for various components of PhantomMail that you can break up, tweak, and re-use in your own variants.

## Documentation
- JSON File format (mostly human editable, except encrypted values- tamper evident): https://github.com/FreddieMercurial/PhantomMail/wiki/PhantomKit-Settings-Vault

## Note:
- Includes another copy of crc32 library cloned from [BrightChain](https://github.com/BrightChain/BrightChain), itself derived from stackoverflow and incrementally improved.
