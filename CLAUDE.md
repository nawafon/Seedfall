# Seedfall — Project Memory

## What this project is
A grey-box-first MVP for a game called Seedfall: a Zelda-style, funky 
low-poly game where you graft "Seed-Cores" (Growth / Heat / Wind) to 
grow plants with emergent behavior, earn Sap, and fend off spreading 
Corruption. Goal of the MVP: prove the grafting loop is fun BEFORE any 
art is made.

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
- [x] 0. Project + folders + .gitignore + this file
- [ ] 1. Player movement (grey capsule)
- [ ] 2. SeedCore ScriptableObjects (Growth/Heat/Wind)
- [ ] 3. Planting (grey growing cylinder)
- [ ] 4. Grafting system (the make-or-break test)
- [ ] 5. Sap resource + buy loop
- [ ] 6. Corruption threat
- [ ] --- FUN GATE: prove grey-box game is fun before art ---
- [ ] 7. Blender low-poly models (Python-generated)
- [ ] 8. Cel-shaded URP materials
- [ ] 9. Polish / sound / branch-sprout reward

## Current State
Step 0 complete: folders, .gitignore, CLAUDE.md, initial commit, and push
to GitHub (private repo, origin set) all done. Nothing else built yet.

## Last Session
2026-07-17 — Repo initialized with official Unity .gitignore, first
commit made, pushed to GitHub origin. Retired BUILD_LOG.md — CLAUDE.md
is now the single memory file; this "Last Session" section is the log
going forward.

## Known issues / TODO
(none yet)
