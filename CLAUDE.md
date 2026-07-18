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
3 SeedCorePickup spheres (Step 2). PlantPlot.cs (occupied/growing state,
TryPlant, colored placeholder cylinder that grows over growTimeSeconds)
+ PlantingInteract.cs (E key, plants first inventory core into nearest
unoccupied plot) + 3 Plot_01/02/03 GameObjects (Step 3).

SampleScene hierarchy now follows Unity Organization Standards: 4 root
folders -- PLAYER -- (Player, Main Camera), -- WORLD -- (Test_MeleeTarget,
Plot_01/02/03), -- PICKUPS -- (3 SeedPickup_* spheres), -- ENVIRONMENT --
(Plane, Directional Light, Global Volume). Nothing loose at scene root.
SimpleFollowCamera.cs was deleted (dead code, confirmed unattached,
superseded by MouseOrbitCamera).

## Last Session
2026-07-18 — Reorganized SampleScene hierarchy to match the new Unity
Organization Standards (4 folders, renamed Player/Test_MeleeTarget,
deleted dead SimpleFollowCamera.cs). Built and confirmed working Step 3
(PlantPlot.cs, PlantingInteract.cs, 3 plots). Hit a real
AssetDatabase/CompilationPipeline desync: a freshly-written .cs file
can be recognized as an imported MonoScript asset yet never actually
enter Assembly-CSharp's compiled source list, even after repeated
Unity_ManageAsset "Import" calls, AssetDatabase.Refresh, and Editor
focus — the fix was deleting the file + its .meta entirely and
recreating both from scratch. Also discovered the scene was NOT being
autosaved by any of the MCP GameObject/component edits — a full
Hierarchy reorg sat unsaved in memory until an explicit
Unity_ManageScene Save call. Lesson: always explicitly save the scene
after Editor-only MCP work, don't assume it persists.

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
