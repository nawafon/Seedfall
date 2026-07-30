# Seedfall — Project Memory

## What this project is
Seedfall: a Zelda-style, funky low-poly game. The player revives a dying 
world centered on a wounded World Tree. Core mechanic: grafting two 
Seed-Cores (Growth/Heat/Wind to start) grows a WEAPON, which the player 
harvests and takes on an EXPEDITION into a dangerous sector. The weapon 
wilts from per-instance durability that decrements on each LANDED HIT 
(not every swing, not a timer) — at 0 remaining hits it wilts mid-
combat and reverts to bare fists. At MVP no seed drops on wilt (seed 
drops from enemies are Step 7 scope). This closes the resource loop: 
grow → expedition → wilt → seeds return → grow again.

The player starts with NOTHING but bare-hand melee combat — no starting 
gift, no menu handout. The first Seed-Cores are found by exploring the 
starting green zone, lying in the world.

Corrupted creatures can be healed instead of killed. Healing has a small 
percentage chance to drop a seed. (Whether healed creatures become 
active allies is a promising direction, not yet finalized as MVP scope.)

The resource limit is PLOTS (how many things can grow at once in the 
player's green zone), not seed scarcity — seeds should be reasonably 
easy to find/earn so experimentation is never punished. Plot count is 
a base-building progression axis.

MVP goal: prototype ONLY the grafting/weapon/expedition loop with 3 
cores, a handful of weapon forms, dumb simple enemies, one small test 
arena, and one small farm/plot area. Test whether the core loop is fun 
BEFORE building sectors, story, base building, or defense systems. Grey 
boxes only — no art yet.

Explicitly CUT from MVP (roadmap only, do not build yet): 6-sector map, 
World Tree story/mystery content, escalating defense/source entity, 
unlimited base building, ally AI, hunting/cooking/traversal tools.

## Hard rules (never break)
1. Unity version is 6000.5.4f1. Never upgrade. Never change Project 
   Settings unless explicitly told.
2. Unity MCP is connected: you CAN directly create GameObjects, add 
   components, set Inspector fields, and wire references, and you CAN 
   query scene/hierarchy/component data. This is structural access 
   only, though -- you still CANNOT see rendered visuals (Game view, 
   Scene view, or Blender viewport), judge aesthetic quality, camera 
   feel, or whether gameplay is fun. Never claim something "looks 
   right" or "should appear." Only the user can judge that.
3. After any script or Editor setup, ALWAYS tell the user exactly what 
   you did (GameObjects created, components added, fields set) and 
   what they should test by pressing Play themselves.
4. Never delete/overwrite a file without showing the change first.
5. One system at a time. Build → user tests in Editor → user confirms 
   → next. Never scaffold multiple features at once.
6. Scripts go in Assets/_Seedfall/Scripts/. Docs in .../Docs/. Never 
   scatter files at the project root (except CLAUDE.md and .gitignore).
7. Editor-only setup (creating GameObjects, adding components, setting 
   Inspector fields, wiring references, creating ScriptableObject 
   assets): do this yourself via Unity MCP when connected. Before 
   changing any Project Setting, still stop and ask first, per Rule 1 
   -- MCP access does not override that. If an MCP action fails or the 
   connection drops, say so plainly and fall back to file-based 
   changes plus manual instructions for that specific piece only. 
   Never pretend you did something you didn't.
8. Remind the user to git commit after every working step.
9. For any step involving both new scripts and Editor/MCP setup, follow 
   this order strictly: (1) write ALL scripts for the step first; (2) 
   stop and confirm zero compile errors by querying the Console via MCP 
   -- never assume; (3) only after confirming clean compilation, proceed 
   to MCP-based scene setup (creating GameObjects, attaching components, 
   wiring fields). Querying existing scene/asset state to inform how 
   code is written is fine at any point -- it's CREATING or MODIFYING 
   scene objects that must wait until step 2 is confirmed clean.

## Unity Organization Standards

HIERARCHY (in-scene organization):
- Every scene must have top-level empty GameObjects acting as folders, 
  named with -- around them for visibility: -- PLAYER --, -- WORLD --, 
  -- ENEMIES --, -- PLANTS --, -- UI --, -- MANAGERS --, 
  -- ENVIRONMENT --. Every GameObject you create goes inside the 
  matching one -- none left loose at the scene root.
- Never leave default names like "GameObject", "Cube (1)", 
  "Capsule (2)". Always rename immediately to something descriptive 
  (e.g. "Player", "TestTarget_Cube", "Plot_01").
- Manager/system objects (game state, spawners, controllers with no 
  visual mesh) go under -- MANAGERS -- even if empty GameObjects.

PROJECT FOLDER STRUCTURE (already partly set up, keep following it):
- Assets/_Seedfall/Scripts/Player/, Scripts/Core/, Scripts/Plants/, 
  Scripts/Enemies/, Scripts/UI/ -- one script per file, filename 
  matches class name exactly.
- Prefabs/ -- subfolder per category once there are enough to warrant 
  it (Prefabs/Weapons/, Prefabs/Plants/, Prefabs/Enemies/) rather than 
  one flat folder.
- ScriptableObjects/ -- subfolder per data type 
  (ScriptableObjects/Cores/, ScriptableObjects/Weapons/).
- Materials/, Models/, Scenes/, Docs/ as already established.
- Never create a script, prefab, or asset outside Assets/_Seedfall/ 
  unless it's a third-party package.

NAMING CONVENTIONS:
- Scripts/classes: PascalCase (PlayerController, SeedCore).
- GameObjects in Hierarchy: PascalCase, descriptive, no default Unity 
  names, no trailing numbers unless there are genuinely multiple 
  identical instances (Plot_01, Plot_02 is fine; Cube (3) is not).
- Prefabs: prefix by type where it helps scanning a folder fast, e.g. 
  Weapon_FireBlade, Core_Growth, Enemy_Basic.
- ScriptableObject assets: match the naming already used for cores 
  (SeedCore_Growth, SeedCore_Heat, SeedCore_Wind) and extend that 
  pattern to future data assets.

PREFABS OVER SCENE COPIES:
- Anything that will exist in more than one place (weapons, plants, 
  enemies, pickups) must be made into a Prefab, not duplicated loose 
  in the scene. Tell the user when a prefab is created and where it's 
  saved.

CLEANUP DISCIPLINE:
- Temporary test objects get a "Test_" prefix (e.g. Test_MeleeTarget) 
  so it's obvious later what's throwaway versus permanent. Tell the 
  user if something should be deleted once its job is done.
- Before finishing any task, self-check: anything unrenamed, 
  unparented, or outside the established folder structure? Fix it 
  before reporting the task done.

## Build order (check the box when a step is tested & working)
- [x] 0. Setup / Git / memory file
- [x] 1. Player movement + bare-hand melee (grey capsule, fists work)
- [x] 2. SeedCore data (Growth/Heat/Wind) + findable seed pickups in world
- [x] 3. Planting in a small plot area (limited plot count, not scarcity)
- [x] 4. Grafting system → produces a weapon (not just a seed/plant)
- [x] 5. Weapon pickup, equip, and swing (replacing bare hands)
- [x] 6. Expedition structure: leave plot area, weapon active in a test 
      arena, weapon wilts at expedition end and drops seeds
- [ ] 7. Dumb simple enemies in the test arena to fight (or heal, small 
      chance of seed drop)
- [ ] --- FUN GATE: is grow+graft+expedition+wilt actually fun? ---
- [ ] 8. (post-MVP) Sectors, story, base depth, defense, allies

## Current State
Steps 0-3 complete and confirmed in Editor. Player movement, bare-hand
melee, mouse-orbit camera (Step 1). SeedCoreData ScriptableObject
(Growth/Heat/Wind assets in ScriptableObjects/) + PlayerSeedInventory +
3 SeedCorePickup spheres (Step 2). PlantPlot.cs + PlantingInteract.cs
(E key, plants first inventory core into nearest unoccupied plot) + 3
Plot_01/02/03 GameObjects (Step 3).

Step 4a done and confirmed (grafting LOGIC only): Assets/_Seedfall/
Scripts/Weapons/ has WeaponGimmick.cs (enum), WeaponData.cs
(ScriptableObject: name/damage/range/cooldown/gimmick/
placeholderPrefab), GraftRecipe.cs (coreA/coreB/result + order-
independent Matches()), GraftingSystem.cs (List<GraftRecipe>,
TryGraft(a,b) returns matching WeaponData or null + logs "No recipe
found"). 3 WeaponData + 3 GraftRecipe assets in
ScriptableObjects/Weapons/ (Thornblaze/Windbriar/Cindergale).
GraftTestDebug.cs was TEMPORARY test scaffolding on GraftTestRig (under
-- TESTING --), but the number-key (Alpha1-4) test-graft shortcuts were
removed once GraftMenuUI covered the same combinations for real -- it's
now a small permanent debug tool (I-key logs
PlayerSeedInventory.GetDebugSummary(), the only place that's bound).
GraftTestRig's now-orphaned graftingSystem Inspector reference was left
as-is (Unity drops it silently, harmless) rather than touched via MCP.

Step 4b done and confirmed: GraftMenuUI.cs (Tab to toggle, unlocks/
shows cursor while open) under -- UI --/GraftMenuController, full
Canvas/Panel/2 dropdowns/button/result text built via Unity's own UI
menu commands (not hand-assembled), CanvasScaler set to Scale With
Screen Size so it isn't tiny on high-res displays. WeaponSpawnPoint
child of Player. WeaponPickup_Placeholder.prefab (grey cube) wired
into all 3 WeaponData assets' placeholderPrefab.

DATA MODEL RESTRUCTURE, done and confirmed (design changed mid-build,
see history below): seed/core split into two items -- SeedData
(found/picked up in world, plantable) vs SeedCoreData (graft-ready).
SeedData.cs mirrors SeedCoreData.cs's exact style (get-only properties
over private serialized fields). New SeedPickup.cs (trigger-based,
mirrors SeedCorePickup.cs exactly) -- the 3 world pickups use
SeedPickup + grant SeedData, not SeedCoreData directly.
PlayerSeedInventory.cs got pure additions across two rounds: Part 1
(seeds/Seeds/AddSeed/RemoveSeed/HasSeedOfType/RemoveSeedOfType +
sapAmounts/AddSap/GetSapCount + GetDebugSummary() for the I-key debug
log) and the rock-uses round below -- existing seedCores/core methods
from before Part 1 untouched throughout.
SeedCorePickup.cs is now dead code (superseded by SeedPickup.cs, not
attached to anything, left in place for later cleanup).

How you get SeedCoreData now: the "Cracking Stone" (dedicated world
object) design was built, then SCRAPPED before confirmation and fully
deleted (CrackingStone.cs, CrackMenuUI.cs, and their scene objects --
none of it exists anymore). Replaced with: small rock pickups
(RockPickup.cs, Assets/_Seedfall/Scripts/Tools/, +3 uses each) grant a
durability pool on PlayerSeedInventory (rockUses/AddRockUses/
GetRockUses/UseRock). With >=1 rock use available, press R ANYWHERE
(no proximity needed) to open BreakSeedMenuUI.cs, which breaks 1 held
SeedData into 1 matching SeedCoreData for 1 rock use. Big throwable
rocks are explicitly POST-MVP, not built. 3 RockPickup_01/02/03 in the
scene under -- PICKUPS --. BreakSeedMenuPanel built under the existing
GraftMenu Canvas (no duplicate Canvas/EventSystem).

Confirmed working functionally (rock pickup -> R menu -> break seed ->
core gained, rock uses decrement correctly, "out of rock uses" and
"don't have that seed" paths both correct).

Found and fixed 3 real bugs from that deferred UI report: (1)
GraftMenuUI and BreakSeedMenuUI could both be open simultaneously with
no mutual exclusion -- likely the actual cause of the earlier "Graft
menu doesn't close" report (it probably DID close, just invisibly,
because the other menu was still covering the screen); (2) the player
character could still move via WASD while a menu was open even though
the camera was frozen; (3) (user request, not a bug) the camera didn't
freeze at all originally when a menu opened -- fixed by having
MouseOrbitCamera skip all mouse-look/repositioning while
Cursor.lockState isn't Locked. All three now share that one signal:
every menu unlocks the cursor on open and locks it on close, and both
PlayerController and MouseOrbitCamera check that same lockState to
freeze, and each menu's Open() checks it too to refuse opening on top
of another menu. Confirmed working by user.

PlantPlot growth is plain progress data (_growProgress float, 0-1)
driven by a coroutine -- NOT a scaled transform. Each plot has two
pre-placed, initially-inactive child objects, Stage_Small and
Stage_Grown (colliders removed, wired into the PlantPlot component),
swapped via SetActive when progress crosses stageSwitchThreshold
(default 0.5). This means real small/grown models can later replace
these placeholder cylinders as a straight swap, no code changes needed.
Color tinting by core type (green/orange/cyan) still applied to both
stage objects at plant time.

PLANTING/HARVEST REWORK, done, compiled, committed, and confirmed
working by the user via Play-mode test. PlantPlot now plants SeedData (found
seeds), not SeedCoreData (cores only come from Rock/Break-Seed now,
never planted directly). Field/property renamed _plantedCore/
PlantedCore -> _plantedSeed/PlantedSeed; TryPlant(SeedCoreData, ...)
-> TryPlant(SeedData, ...) (calls inventory.RemoveSeed, not
RemoveSeedCore); TintStages and the maturity log use SeedData's real
fields (SeedType, DisplayName). Added sapYield (int, Inspector-
exposed, default 1) and TryHarvest(PlayerSeedInventory): only succeeds
if occupied+matured, calls inventory.AddSap(seed.SeedType, sapYield),
then fully resets the plot to empty (must replant to use again) --
harvesting yields Sap only, no plant/weapon output at MVP. Added
public HasMatured. PlantingInteract's single E-press interact now does
two passes over nearby colliders: harvest any occupied+matured plot
first (priority), else plant _inventory.Seeds[0] into the nearest
unoccupied plot. growTimeSeconds/stageSmall/stageGrown/
stageSwitchThreshold and their Inspector wiring on the 3 Plot objects
were untouched by this change, per the hard boundary it was built
under. Confirmed working end to end: plant (E near empty plot) ->
grow -> "Plot matured" log + grown stage -> harvest (E again on same
plot) -> "Harvested 1 [Type] Sap" log + sap count up (I key) -> plot
back to fully empty and immediately replantable.

SampleScene hierarchy now follows Unity Organization Standards: 4 root
folders -- PLAYER -- (Player, Main Camera), -- WORLD -- (Test_MeleeTarget,
Plot_01/02/03), -- PICKUPS -- (now 6 SeedPickup_* spheres, 2 per core
type -- see Step 5 note below), -- ENVIRONMENT --
(Plane, Directional Light, Global Volume). Nothing loose at scene root.
SimpleFollowCamera.cs was deleted (dead code, confirmed unattached,
superseded by MouseOrbitCamera).

STEP 5 (weapon pickup/equip/swing), code + scene wiring done via MCP,
confirmed zero compile errors, NOT yet Play-tested -- session ended
before testing happened. 7 files: PlantingInteract.cs (E-key listening
removed, plot logic exposed as public TryInteractWithNearbyPlot());
new PlayerInteract.cs (single E-key arbiter -- on E, scans once for
both nearby PlantPlots and WeaponPickups, whichever candidate is
physically closest wins and fires, ties favor the plot; an unoccupied
plot only counts as a candidate if the player actually has a seed to
plant, matching PlantingInteract's real behavior, so a reachable
weapon pickup isn't ignored for nothing); new WeaponPickup.cs (trigger
collider, TryPickUp(WeaponInventory) -- doesn't destroy itself if the
inventory's full, stays on the ground); BareHandMelee.cs extended IN
PLACE (not renamed -- renaming risks GUID detachment since it's
already on Player) with SetStats(range, radius, cooldown)/
ResetToBareHandStats() (original serialized values cached in Awake),
plus a juice pass: successful hits flash the target's renderer white
via the same _BaseColor/_Color check PlantPlot.TintRenderer uses, and
trigger a small camera shake; new WeaponInventory.cs (3 fixed slots,
keys 1/2/3 equip a slot and call into BareHandMelee's SetStats/Reset --
no parallel attack system, empty slot or nothing equipped = bare fists
automatically); MouseOrbitCamera.cs got a Shake(duration, magnitude)
method whose offset is folded into its own LateUpdate position write
(it recomputes transform.position from scratch every frame, so
anything writing from outside would just get overwritten -- folding
the shake into the same authoritative write sidesteps any script-
execution-order race entirely); GraftMenuUI.cs changed ONLY at its
weapon-spawn point -- a successful graft now calls
WeaponInventory.TryAddWeapon() first, only spawning a WeaponPickup
cube (with WeaponData attached) if all 3 slots are already full;
GraftTestDebug.cs's I-key debug log now also prints WeaponInventory's
slot summary.

Scene wiring done via MCP: WeaponInventory + PlayerInteract added to
Player; BareHandMelee.orbitCamera -> Main Camera's MouseOrbitCamera;
GraftMenuUI.weaponInventory and GraftTestDebug.weaponInventory -> Player's
WeaponInventory (the latter was easy to forget -- caught it via
get_components showing null before saving); WeaponPickup added to the
WeaponPickup_Placeholder prefab with its collider's isTrigger EXPLICITLY
set true in the same RunCommand (PrefabUtility.LoadPrefabContents +
AddComponent does NOT reliably fire Reset() the way the Editor's Add
Component menu does -- confirmed this gap rather than assuming Reset()
would cover it, then verified isTrigger=true on the saved asset
afterward). set_component_property's documented dict-based reference
syntax ({"find":..., "component":...}) does NOT work despite being in
the tool's own schema/example -- fails with a JSON deserialization
error every time; RunCommand + SerializedObject.FindProperty(...)
.objectReferenceValue is the only reliable way to wire scene/component
references right now (matches the pre-existing Known Issues entry
below, now double confirmed on a different property).

Also discovered mid-session: the original Step 2 scene only has ONE
SeedPickup per core type (Growth/Heat/Wind), but every graft consumes
2 cores and each core type feeds 2 of the 3 recipes -- so crafting all
3 weapons needs 6 cores = 6 seeds, impossible with only 3 seed pickups
in the world. Fixed by duplicating each SeedPickup once via RunCommand
(Object.Instantiate of the existing GameObject, which preserves its
wired SeedData reference) -- scene now has 6 SeedPickups total (2 per
type) under -- PICKUPS --, named with a _02 suffix, offset a few units
from the originals. Rock uses were already sufficient (3 rocks x 3
uses = 9 available, only 6 needed) so nothing added there.

Step 5 test plan for next session (nothing below this has been run
yet): press I to see seed/sap/weapon debug summary; collect all 6
seeds; break each into a core via R (6 times); graft Thornblaze
(Growth+Heat), Windbriar (Growth+Wind), Cindergale (Heat+Wind) once
each via Tab -- this exactly uses the 6 cores and fills all 3 slots
with 3 different weapons, no need to graft any recipe twice; confirm
each graft goes straight into a slot (no cube spawns since slots
aren't full yet); press 1/2/3 to switch equipped weapon and confirm
attack range/cooldown actually changes per weapon; swing (left click)
near Test_MeleeTarget and confirm the hit flashes white and the camera
shakes briefly; graft a 4th weapon (needs 2 more cores of some type --
none available with the current seed count, so this specific overflow
test may need one more seed pickup added first -- flag to the user if
reached) to confirm it spawns as a WeaponPickup on the ground instead
of entering inventory; walk up to it and press E to confirm pickup
only succeeds once a slot is free; stand near both a matured plot and
a dropped weapon at different distances and press E to confirm only
the closer one responds; confirm bare fists return when no weapon is
equipped.

STEP 6 (expedition structure + weapon wilt), done, compiled, scene-
wired via MCP, and confirmed working by the user via Play-mode test.
Design lock: same-scene arena (not a separate scene) reached via an
ExpeditionPortal trigger that repositions the player -- no actual
"expedition state" is tracked, portal is cosmetic for this isolation
test. Weapon wilt is per-instance durability on a plain C# wrapper
Assets/_Seedfall/Scripts/Weapons/EquippedWeapon.cs (Data + RemainingHits,
DecrementAndCheckWilt()) -- NOT on the shared WeaponData asset, since
every copy of e.g. Thornblaze would otherwise share one counter.
WeaponInventory's 3 slots now hold EquippedWeapon, not WeaponData
directly. Durability decrements on a LANDED HIT ONLY (not every swing,
not a timer) -- BareHandMelee.cs gained an OnHitLanded event, fired
once per swing that connects with at least one collider (never for a
swing at empty air, never more than once even with multiple colliders
hit); WeaponInventory subscribes and calls DecrementAndCheckWilt(). At
0 remaining hits the weapon wilts: slot nulled, reverts to bare fists
if it was equipped, no seed drop (seed drops are Step 7/enemy scope).
WeaponData gained maxHits (int, default 8, all 3 existing assets set to
5 for testing). New Assets/_Seedfall/Scripts/World/ExpeditionPortal.cs:
trigger collider + serialized destination Transform, teleports any
GameObject with a CharacterController (disables it for the position
write, since CharacterController fights direct transform writes
otherwise, then re-enables).

Scene wiring done via MCP: new "-- ARENA --" root at x=50 (far from the
plot area, which is centered near the origin) with Arena_Floor (Plane,
scale 2 => 20x20), ArenaSpawnPoint (50,1,-6, facing +z),
Test_DurabilityDummy (solid cube, needed because hit-only durability
can't demonstrate wilt in an empty arena), and Portal_ToPlotArea
(trigger cube, dest=PlotAreaReturnPoint). In "-- WORLD --":
PlotAreaReturnPoint (0,1,-4) and Portal_ToArena (trigger cube,
dest=ArenaSpawnPoint). Destinations wired via RunCommand +
SerializedObject.objectReferenceValue (set_component_property doesn't
reliably assign scene-object references, per Known Issues below).
Confirmed working end to end: walk into Portal_ToArena -> teleport to
arena; equip a weapon and hit Test_DurabilityDummy -> console logs
remaining hits counting down, 5th landed hit wilts the weapon and
reverts to bare fists; walk into Portal_ToPlotArea -> teleport back.

## Session Notes Addendum 2 (2026-07-30)
Planting/harvest choice rework, confirmed working by user. PlantPlot.cs:
added seedYield (int, default 2, auto-defaulted correctly on the 3
existing scene plots with no MCP write needed). Split the old single
TryHarvest into TryHarvestForSeeds (returns seedYield SeedData of the
planted type to inventory -- plant 1, get 2 back) and TryHarvestForSap
(unchanged sap behavior), both routing through a new private ClearPlot()
helper (byte-identical reset logic, deduplicated). PlantingInteract.cs:
E path (TryInteractWithNearbyPlot) now calls TryHarvestForSeeds; added
TryHarvestNearbyPlotForSap() for the new F path (same nearby-scan
pattern, no planting fallback). PlayerInteract.cs: added a
[SerializeField] sapHarvestKey (KeyCode.F) checked independently in
Update(), calling _plantingInteract.TryHarvestNearbyPlotForSap()
directly -- does NOT go through the closest-plot-vs-weapon arbitration
(F has no weapon meaning), and that arbitration logic itself was left
byte-identical. No new scene wiring needed. Growth: plant seed (1
consumed) -> mature -> E harvests to 2 seeds of that type (net +1) OR F
harvests to 1 sap (old behavior) -> plot empties either way. This is
the "grow seed supply vs. grow the World Tree" tension the design
intends. Also hit a red herring during this session: a Console
"Assertion failed on expression: 'ValidTRS()'" spam traced (via
stacktrace) to Unity MCP's OWN get_components serializer walking
Transform.lossyScale/rotation via Matrix4x4 -- not from game code, not
from Play mode, not a sign of a broken transform (the transforms
involved were confirmed scale 1,1,1). Known spurious Unity engine
assertion in that internal check; safe to ignore if seen again.

## Session Notes Addendum (2026-07-30)
Step 5 Play-tested and confirmed working functionally (weapon pickup,
equip via 1/2/3, swing, hit flash, camera shake, slot-full ->
ground-drop, E-priority between plot/weapon, bare-fist fallback --
the full test plan listed in Step 5's note above). User's verdict:
"good not great" -- functionally correct but the feel/juice (hit
feedback, weapon differentiation, swing feel) isn't landing yet.
Explicitly deferred, NOT a blocker -- user asked to move on to Step 6
and revisit polish later. Do not start a feel/juice pass unprompted;
wait until the user raises it again.

## Last Session
2026-07-18/19 — Built and confirmed working Step 4b (GraftMenuUI). Hit
two usability bugs, both fixed: cursor never unlocked on menu open
(MouseOrbitCamera hides it by default), and UI was tiny/unreadable at
2560x1440 (no CanvasScaler scaling) -- fixed with Scale With Screen
Size + a centered panel.

Completed the seed/core data model restructure Part 1: split SeedData
(plantable, world pickup) from SeedCoreData (graft-ready). Then built
a "Cracking Stone" world object for turning seeds into cores (Part 2)
-- but the design changed again before confirmation, so it was fully
deleted (scripts + scene objects) and rebuilt as: rock pickups grant a
rock-uses pool, press R anywhere to open a Break Seed menu that
consumes 1 rock use + 1 seed for 1 core. Confirmed working
functionally end to end.

Hit a real "changes made during Play mode don't persist" trap while
iterating on UI sizing -- a fix applied via RunCommand while the user
was in Play mode silently vanished the moment Play stopped, since
Unity discards runtime edits to scene objects. Also found that
TMP_Dropdown has TWO separate text elements that both need sizing: the
visible current-selection Label, and a completely separate template
buried in Template/Viewport/Content/Item/Item Label used for the
popup's option rows -- fixing only the first one left the dropdown's
open-list text tiny and illegible even though everything else looked
correctly sized.

Diagnosed and fixed the deferred bug plus two more found alongside it:
Graft and Break Seed menus had no mutual exclusion (both could be open
at once -- likely the real cause of the earlier "doesn't close"
report), the player could still move while a menu was open, and
(user-requested, not a bug) the camera wasn't frozen at all during
menu use. All fixed via one shared signal (Cursor.lockState) that
every menu, the camera, and player movement now all check consistently.
Confirmed working by user.

Also reworked PlantPlot/PlantingInteract per the SeedData/Sap plan
above (planting now uses SeedData not SeedCoreData; added
TryHarvest/sapYield/HasMatured). This landed as uncommitted working-
tree changes from a prior context window; picked back up this session
via `git status`/`git diff` rather than session memory, committed, and
pushed. Confirmed working by the user via Play-mode test.

2026-07-19/20 -- Built all of Step 5 (weapon pickup/equip/swing) end
to end: code, MCP scene wiring, zero-compile-error confirmation, and a
scene content fix (duplicated seed pickups so 3 weapons are actually
craftable). Full detail is in the STEP 5 paragraph under Current
State above. Session ended before Play-testing -- that's the very
next thing to do. Everything is committed (see git log), nothing left
uncommitted.

## Known issues / TODO
- If a newly-written script's type can't be resolved (CS0246 in a file
  that references it, even though the referenced file itself shows no
  error), don't just keep re-importing it — check whether it's actually
  in Assembly-CSharp's compiled source list
  (UnityEditor.Compilation.CompilationPipeline.GetAssemblies()). If
  it's missing, delete the .cs file + its .meta and recreate both from
  scratch rather than re-importing the existing one.
- After any Editor-only MCP work that changes the scene (GameObjects,
  components, hierarchy), explicitly call Unity_ManageScene Save --
  those changes do not appear to autosave.
- Unity_ManageGameObject's "create" action treats `position` as WORLD
  space even when `parent` is set; "modify" treats `position` as LOCAL
  space. Inconsistent -- compensate accordingly.
- `components_to_remove` and `set_active` passed alongside "create"
  don't reliably take effect -- do them as separate follow-up
  "remove_component"/"modify" calls.
- Once a GameObject is inactive, plain name lookup for `target` fails
  -- use `search_method: "by_path"` with the full hierarchy path.
- `set_component_property` cannot assign GameObject-typed fields (only
  asset references, e.g. ScriptableObjects, work via a plain asset path
  string). For wiring scene-object or component references, use
  Unity_RunCommand with `SerializedObject`/`objectReferenceValue`
  instead. Note: a List<> of asset references (e.g.
  List<GraftRecipe>) DOES work fine via set_component_property as an
  array of asset path strings -- the limitation is specifically
  scene-object/component references, not lists in general.
- The MCP connection can drop mid-task ("Connection revoked") during
  or after compile-heavy operations, with no apparent pattern. Recovery
  is always the same: user re-approves in Project Settings > AI > Unity
  MCP, then check GetState. Before retrying whatever failed, verify
  nothing partial was created (e.g. via Glob on the expected output
  path) rather than assuming a clean slate or assuming it fully ran.
- MouseOrbitCamera locks/hides the cursor by default for mouse-look.
  Any new UI screen that needs clicking (menus, inventory, etc.) MUST
  explicitly unlock/show the cursor when it opens and re-lock/hide it
  when it closes, or it'll be completely unusable with no visible
  cursor -- easy to miss since everything else works fine.
- A new Canvas has no CanvasScaler scaling by default (fixed pixel
  sizes) -- on a high-res display this renders as a tiny illegible
  cluster. Set uiScaleMode to ScaleWithScreenSize with a sane reference
  resolution (e.g. 1920x1080) for any new UI.
- For "missing script" checks after a component swap (remove +
  add different type), don't rely on the Console alone -- query
  GetComponents<Component>() via RunCommand and count nulls. That's
  the same thing a Console-silent "Missing (Mono Script)" Inspector
  warning would show up as.
- Changes made via RunCommand (or any MCP action) while the Editor is
  in Play mode do NOT persist -- Unity discards runtime edits to scene
  objects the moment Play stops. Always check IsPlaying via GetState
  before relying on a scene edit sticking; if true, ask the user to
  stop Play first.
- TMP_Dropdown has two separate text elements that both need sizing
  independently: the visible current-selection Label, and a totally
  separate template at Template/Viewport/Content/Item/Item Label used
  to render each row of the open popup list. Resizing only the Label
  leaves the popup list tiny even though the dropdown looks fixed when
  closed. Also bump the Item row's RectTransform height and the
  Template's width to match, or the bigger text clips/doesn't fit.
- Any script that unlocks the cursor for a menu (currently GraftMenuUI,
  BreakSeedMenuUI) should be treated as part of a shared "is a menu
  open" contract: (1) check Cursor.lockState before opening and refuse
  if it's already unlocked by another menu, (2) PlayerController and
  MouseOrbitCamera both freeze based on that same lockState. If a new
  menu is added later, wire it into this same pattern from the start
  rather than rediscovering these three bugs again.
- When debugging a behavior you can't reproduce yourself (no Play mode
  access), don't guess-fix repeatedly -- add temporary Debug.Log
  instrumentation, ask the user to reproduce, then read the Console
  directly. Remove the temp logging once a real fix lands. In this
  case the actual root cause (two menus open at once) turned out to be
  something instrumentation wasn't even needed for once the user
  described a second symptom.
- When asked to commit "those N files" after a context gap (new
  session, compacted context), don't stage-and-commit by filename
  alone -- `git diff` each file first. Here, 2 of 6 modified files
  (PlantPlot.cs, PlantingInteract.cs) contained an entire separate
  feature (SeedData/Sap harvest rework) that a menu-fix commit message
  didn't mention at all, and CLAUDE.md hadn't documented yet either.
  The work was correct and already compiling, but it was one `git
  diff` away from being silently mislabeled in history. Read the diff,
  not just the file list, before writing the commit message.
