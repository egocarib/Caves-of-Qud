# QudUX
This is a fork of [QudUX-2](https://github.com/egocarib/CavesOfQud-QudUX-v2?tab=readme-ov-file) by Egocarib, I am slowly going through errors the game encounters when trying to load the original mod. When it is done, I'll either do a merge request on the original repo or push this project on steam workshop

See the readme of the [original repo](https://github.com/egocarib/CavesOfQud-QudUX-v2?tab=readme-ov-file) for informations about the mod

## Current status
All API errors have been fixed, excepted some key mapping related code that might require bigger refactoring. I decided to cut the whole feature to be able to test everything else.

### Non-Patch Related
✔ Revamped UI text inventory<br>
✔ Quest Giver location markers<br>
✔ Modify character sprite<br>
✔ Open Autoget settings<br>
✔ Display Game Statistics<br>
✔ Restocker / Merchants additional dialog <br>
❌ Hero locations addition to journal<br>

Probably other things I didn't yet discovered

### Patches
✔ Patch_XRL_Annals_QudHistoryFactory<br>
✔ Patch_XRL_UI_AbilityManager<br>
✔ Patch_XRL_World_Parts_Mutation_MagneticPulse<br>
✔ Patch_XRL_Core_XRLCore<br>
✔ Patch_XRL_Core_Scores<br><br>

⚠️ Patch_XRL_UI_Look - **Works with weird quirks, not usable IMO**<br><br>

❌ Patch_XRL_World_GameObject_Move<br>
❌ Patch_XRL_World_Parts_Campfire<br>
❌ Patch_XRL_World_Parts_Physics<br>
