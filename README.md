## What is this?

This is CLI projects for every supported game servers for the [Lunatic Panel](https://github.com/mshimshon/LunaticPanel/) Plugin [GameHost](https://github.com/mshimshon/GameHost/) but it also work independently just like [LinuxGMS](https://linuxgsm.com/) does.

These projects are standarizing API for installing and controlling lifecycle of game servers MODS and config file included so the plugin can use a standard method of communication with those CLI.

They can entirely run as standalone via regular console command or bash scripts by design for Automation itself.
The console also create any game server under username lgsm and policy is one container = one server it cannot handle multiple instances.

Most of the consoles are wrapping [LinuxGMS](https://linuxgsm.com/) in the background whenever available, no need to reinvent the wheel there.

## Documentation
Full documentation will be provided soon

## LICENSE
Every CLI is MIT licensed and will remain MIT forever do whatever you need with it.

please note the [GameHost](https://github.com/mshimshon/GameHost/) plugin is NOT MIT therefore if used through the plugin it is bound by its license. 