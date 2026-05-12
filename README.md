## What is this?

This is the official repository for LGSM's MaksimShimshon.GameManagerPanel support for games. each supported server contains information and scripts to bind LGSM and the panel.
Such as: startup parameters, modding capabilities and other game specific scripts like (Arma 3 auto lowercases all PBOs to avoid mod issues.).

## Are you Hosting Service?
Each supported game should offer the ability to "patch" settings meaning for example "Project Zomboid" has a config_patch.cfg this ensures a forces ini settings change by the client will automatically revert to your allowed value base on the patch file every restart/startup of the game server.

game_info_overrides allows you to for example disable mods for a game... or allow specifc steam branchs or even disable or set default value for startup parameters ie: ports... when the config files are update from our official repo you changes remain for better or worst... your file setting is the ultimate authority and will prevele over ours.

You can also do in AppSetting.json change the repository location from this here to your own and publish your own updates and developped scripts.

## Overrides Location
the files can change based on game as it is game specific but the location remain `/home/lgsm/lunaticpanel/plugins/maksimshimshon_gamemanagepanel/overrides/`.

for the game_info.json it is universal to all games: `/home/lgsm/lunaticpanel/plugins/maksimshimshon_gamemanagepanel/overrides/game_info_overides.json`.
just copy the game_info.json but ensure you leave only the persistent properties you wish to change... to avoid complete override.