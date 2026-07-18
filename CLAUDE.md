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
2. You CANNOT see the Unity Editor or Blender viewport. Never claim 
   something "looks right" or "should appear." Only report file changes.
3. After any script, ALWAYS tell the user: what GameObject to attach it 
   to, what Inspector fields to set, and what they should see on Play.
4. Never delete/overwrite a file without showing the change first.
5. One system at a time. Build → user tests in Editor → user confirms 
   → next. Never scaffold multiple features at once.
6. Scripts go in Assets/_Seedfall/Scripts/. Docs in .../Docs/. Never 
   scatter files at the project root (except CLAUDE.md and .gitignore).
7. Editor-only tasks (attaching components, baking, creating 
   ScriptableObject assets): WRITE the steps for the user. Never pretend 
   you did them.
8. Remind the user to git commit after every working step.

## Build order (check the box when a step is tested & working)
- [x] 0. Setup / Git / memory file
- [ ] 1. Player movement + bare-hand melee (grey capsule, fists work)
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
Step 0 complete: folders, .gitignore, CLAUDE.md, initial commit, and push
to GitHub (private repo, origin set) all done. Nothing else built yet.

## Last Session
2026-07-18 — Design corrected after external review (ChatGPT + Kimi
critiques): replaced mid-combat weapon durability with per-expedition
wilting, added hands-first start, added plots-based resource limit,
added healing-seed-drop chance.

## Known issues / TODO
(none yet)
