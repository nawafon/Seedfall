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

## Build order (check the box when a step is tested & working)
- [x] 0. Setup / Git / memory file
- [x] 1. Player movement + bare-hand melee (grey capsule, fists work)
- [ ] 2. SeedCore data (Growth/Heat/Wind) + findable seed pickups in world
- [ ] 3. Planting in a small plot area (limited plot count, not scarcity)
- [ ] 4. Grafting system → produces a weapon (not just a seed/plant)
- [ ] 5. Weapon pickup, equip, and swing (replacing bare hands)
- [ ] 6. Expedition structure: leave plot area, weapon active in a test 
      arena, weapon wilts at expedition end and drops seeds
- [ ] 7. Dumb simple enemies in the test arena to fight (or heal, small 
      chance of seed drop)
- [ ] --- FUN GATE: is grow+graft+expedition+wilt actually fun? ---
- [ ] 8. (post-MVP) Sectors, story, base depth, defense, allies

## Current State
Step 1 complete and confirmed in Editor: CharacterController-based
third-person movement (camera-relative, no jump), bare-hand melee
(left-click, OverlapSphere hit check, cooldown), and a mouse-orbit
follow camera (MouseOrbitCamera.cs on Main Camera, root-level object,
not parented to the player). SimpleFollowCamera.cs still exists on disk
but is unused/unattached — an earlier fixed-angle camera approach
superseded by MouseOrbitCamera.

## Last Session
2026-07-18 — Built and confirmed working: PlayerController.cs,
BareHandMelee.cs, MouseOrbitCamera.cs. Also switched Active Input
Handling to "Both" in Project Settings (approved) so legacy Input
still works alongside the installed new Input System package. Hit and
fixed a camera/movement feedback-loop bug caused by parenting the
camera directly to the rotating player; fix was an independent
mouse-orbit camera that never inherits the player's rotation.

## Known issues / TODO
- SimpleFollowCamera.cs (Assets/_Seedfall/Scripts/Player/) is dead code
  — not attached to anything, superseded by MouseOrbitCamera.cs. Safe
  to delete later; leaving it for now since it hasn't been asked for.
