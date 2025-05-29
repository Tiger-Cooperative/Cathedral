<div class="header" align="center">  
<img alt="Space Station 14" width="880" height="300" src="https://raw.githubusercontent.com/space-wizards/asset-dump/de329a7898bb716b9d5ba9a0cd07f38e61f1ed05/github-logo.svg">  
</div>

Cathedral is a fork of Space Station 14, which is a remake of Space Station 13 that operates on the Robust Toolbox engine. It is intended to serve as an alternative upstream that is able to be configured to the desires of multiple server owners on a case-by-case basis, rather than catering to a specific type of server. Similarly, there is a strong emphasis on hearing what the owners and operators of servers would like to see developed.

The primary focus of changes made here will be to ensure that the mechanics and roles in this game have depth and impact, as well as being fun for those involved.

As such, there is no designated roleplay level that the content here is intended for. Similarly, there currently is not any official server, and as of the time this is being written, it is unlikely that there will be one.

If anybody has an interest in using this codebase for a server, they are invited to do so.

## Links

<div class="header" align="center">  

None, for now.

</div>

## Documentation/Wiki

The [docs site](https://docs.spacestation14.com/) has documentation on SS14's content, engine, game design, and more.  
Additionally, see these resources for license and attribution information:  
- [Robust Generic Attribution](https://docs.spacestation14.com/en/specifications/robust-generic-attribution.html)  
- [Robust Station Image](https://docs.spacestation14.com/en/specifications/robust-station-image.html)

## Contributing

Contributions are accepted by whoever may wish to offer help, though this project is still in a fledgling stage and thus changes and fixes to pre-existing content will be weighed more heavily than addition of new content.
The list of [issues](https://github.com/Tiger-Cooperative/Cathedral/issues) contains work that will need to be done, and the Discord server has channels for those who wish to contribute.

## Building

1. Clone this repo:
```shell
git clone https://github.com/Tiger-Cooperative/Cathedral.git
```
2. Go to the project folder and run `RUN_THIS.py` to initialize the submodules and load the engine:
```shell
cd Cathedral
python RUN_THIS.py
```
3. Compile the solution:  

Build the server using `dotnet build`.

[More detailed instructions on building the project.](https://docs.spacestation14.com/en/general-development/setup.html)

## License

All code for the content repository is licensed under the [MIT license](https://github.com/space-wizards/space-station-14/blob/master/LICENSE.TXT).  

Most assets are licensed under [CC-BY-SA 3.0](https://creativecommons.org/licenses/by-sa/3.0/) unless stated otherwise. Assets have their license and copyright specified in the metadata file. For example, see the [metadata for a crowbar](https://github.com/space-wizards/space-station-14/blob/master/Resources/Textures/Objects/Tools/crowbar.rsi/meta.json).  

> [!NOTE]
> Some assets are licensed under the non-commercial [CC-BY-NC-SA 3.0](https://creativecommons.org/licenses/by-nc-sa/3.0/) or similar non-commercial licenses and will need to be removed if you wish to use this project commercially.
