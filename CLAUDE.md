# Seedfall — Project Memory

## What this project is
Seedfall: a Zelda-style, funky low-poly game. The player revives a dying 
world centered on a wounded World Tree. Core mechanic: grafting two 
Seed-Cores (Growth/Heat/Wind to start) grows a WEAPON, which the player 
harvests and takes on an EXPEDITION into a dangerous sector. The weapon 
wilts at the end of the expedition (not mid-combat, not from per-swing 
durability) and drops seeds back — sometimes mutated/improved. This 
closes the resource loop: grow → expedition → wilt → seeds return → 
grow again.

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
- [ ] 4. Grafting system → produces a weapon (not just a seed/plant)
- [ ] 5. Weapon pickup, equip, and swing (replacing bare hands)
- [ ] 6. Expedition structure: leave plot area, weapon active in a test 
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
GraftTestDebug.cs is TEMPORARY test scaffolding (marked with a "//
TEMP" comment) on GraftTestRig (under -- TESTING --).

Step 4b done and confirmed: GraftMenuUI.cs (Tab to toggle, unlocks/
shows cursor while open) under -- UI --/GraftMenuController, full
Canvas/Panel/2 dropdowns/button/result text built via Unity's own UI
menu commands (not hand-assembled), CanvasScaler set to Scale With
Screen Size so it isn't tiny on high-res displays. WeaponSpawnPoint
child of Player. WeaponPickup_Placeholder.prefab (grey cube) wired
into all 3 WeaponData assets' placeholderPrefab.

DATA MODEL RESTRUCTURE, Part 1 of 2, done and confirmed: seed/core
split into two items -- SeedData (found/picked up in world, plantable)
vs SeedCoreData (graft-ready, only obtainable via Cracking Stone in
Part 2, not built yet). SeedData.cs mirrors SeedCoreData.cs's exact
style (get-only properties over private serialized fields). New
SeedPickup.cs (trigger-based, mirrors SeedCorePickup.cs exactly) --
the 3 world pickups now use SeedPickup + grant SeedData, not
SeedCoreData directly. PlayerSeedInventory.cs got pure additions
(seeds/Seeds/AddSeed/RemoveSeed/HasSeedOfType/RemoveSeedOfType +
sapAmounts/AddSap/GetSapCount + GetDebugSummary() for the I-key debug
log) -- existing seedCores/core methods untouched.
SeedCorePickup.cs is now dead code (superseded by SeedPickup.cs, not
attached to anything, left in place for later cleanup).

Known consequence, expected and not a bug: GraftMenuUI is currently
unusable in practice -- the player can hold SeedData but zero
SeedCoreData, since nothing produces cores yet. Cores return in Part 2
via the Cracking Stone.

PlantPlot growth is plain progress data (_growProgress float, 0-1)
driven by a coroutine -- NOT a scaled transform. Each plot has two
pre-placed, initially-inactive child objects, Stage_Small and
Stage_Grown (colliders removed, wired into the PlantPlot component),
swapped via SetActive when progress crosses stageSwitchThreshold
(default 0.5). This means real small/grown models can later replace
these placeholder cylinders as a straight swap, no code changes needed.
Color tinting by core type (green/orange/cyan) still applied to both
stage objects at plant time.

SampleScene hierarchy now follows Unity Organization Standards: 4 root
folders -- PLAYER -- (Player, Main Camera), -- WORLD -- (Test_MeleeTarget,
Plot_01/02/03), -- PICKUPS -- (3 SeedPickup_* spheres), -- ENVIRONMENT --
(Plane, Directional Light, Global Volume). Nothing loose at scene root.
SimpleFollowCamera.cs was deleted (dead code, confirmed unattached,
superseded by MouseOrbitCamera).

## Last Session
2026-07-18 — Built and confirmed working Step 4b (GraftMenuUI: Tab
menu, dropdowns, graft button, result text, built via Unity's own UI
menu commands rather than hand-assembled). Hit two real usability bugs
after first "confirm" attempt: (1) menu was unusable because
MouseOrbitCamera locks/hides the cursor by default and GraftMenuUI
never released it -- fixed by unlocking/showing the cursor on menu
open; (2) UI was tiny/unreadable on a 2560x1440 display because the
Canvas had no CanvasScaler scaling and small fixed-pixel offsets --
fixed with Scale With Screen Size + a fixed centered 600x450 panel.

Then completed the seed/core data model restructure, Part 1 of 2:
split "seed" and "seed core" into separate items (SeedData vs
SeedCoreData; cores now only obtainable via a Cracking Stone in Part 2,
not yet built). Added SeedData.cs + SeedPickup.cs (new file, mirroring
SeedCoreData.cs/SeedCorePickup.cs exactly), pure additions to
PlayerSeedInventory.cs (verified byte-identical pre-existing code),
swapped SeedCorePickup -> SeedPickup component on all 3 world pickups
(verified no missing-script refs via GetComponents<Component>() null
check, not just Console). GraftMenuUI is now expected-broken (player
holds SeedData, zero SeedCoreData) until Part 2.

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
