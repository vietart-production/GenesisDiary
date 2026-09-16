# GenesisDiary — Base GDD ("Game Sáng Thế")

> Baseline design document, consolidated from prior planning discussion + PDF summary (2026-09-17). This is the foundation to iterate on, not a frozen spec.

## Core Concept

The player does not directly control the world. They play a god-like observer: they set initial conditions (terrain, water, environment, the first life), then watch the world **evolve on its own** over simulated centuries or millennia. What emerges — which species survive, which go extinct, what creatures look like generations later — is not predetermined by the designer. It **emerges** from the interaction of environment, survival, reproduction, and time.

## Technical Direction

- Visually a **2D game**, but built on a **3D-type pipeline** (Unity URP 3D, not the 2D toolkit) — so a later upgrade to real depth/3D doesn't require starting over.
- Creatures and environment do **not** use fixed 2D sprites or fixed 3D models. They're built from **shaders and procedural shape-generation systems** (procedural mesh, particle systems, etc.) that construct form from data at runtime.

## Creatures Are Data, Not Assets

Each individual is described by a **DNA/genome** — a set of parameters covering morphology (body length/height/width, head size, number/length/thickness of legs, tail length/curvature, ear/eye size, skin/fur color and density) and, later, behavior (aggression, fear, curiosity, sociality).

- **Reproduction & inheritance**: offspring inherit genes from both parents plus possible mutation — not a copy, a varied recombination. Populations accumulate morphological drift over generations.
- **Body structure** is layered: `Body → Skeleton → Muscle/Movement → Behavior → Appearance`, flexible enough that a mutation from 4 legs to 6 legs generates a new structure procedurally — no new model needed.
- **Procedural animation**: movement is generated from math functions over morphological parameters (phase, amplitude, frequency, body bounce, tail swing, etc.), not animation clips/Animator — so movement stays correct automatically as body structure changes across generations.

## Evolution Emerges From Simulation

No hardcoded milestones like "after 1000 years, legs get longer." The system only provides environment, resources, energy, danger, reproduction, and death. Individuals whose traits improve survival/reproduction leave more offspring, so the corresponding genes become more frequent naturally.

- **Speciation**: populations can diverge over time; branches that differ enough for long enough get recorded as distinct species. Players can view the evolutionary tree and ancestry relationships.
- **World history log**: the system records key events — first population, notable mutations, population branching, new species, extinctions, major environment changes — making the world's history traceable.

## Time System

Time scale is highly variable (1x, 10x, 100x, 1000x+), simulating days through millennia, while still allowing the player to zoom back in and observe individual creatures at normal speed.

## God / Player Interaction

The player can still act: create/destroy environment, change water/climate/food sources, trigger events, or directly affect creatures. Once the player stops intervening, the simulation keeps running on its own.

## Overall Architecture

```
WORLD           → Terrain, Water, Plants, Climate
LIFE SIMULATION → Food, Energy, Death, Reproduction, Population
CREATURE SYSTEM → DNA, Morphology, Skeleton, Movement, Brain, Appearance
EVOLUTION       → Inheritance, Mutation, Selection, Speciation, Extinction
TIME            → Day, Year, Century, Millennium
GOD / PLAYER    → Create, Observe, Modify, Accelerate Time
```

## Core Design Principles

- No dependency on model assets for creature variants.
- Morphology is inheritable, mutable data.
- Animation is generated procedurally from body structure.
- Evolution emerges from survival/reproduction, never hardcoded milestones.
- The world keeps running when the player isn't intervening.
- The representation layer (particle/VFX/procedural mesh) is decoupled from creature logic — not hard-tied to one renderer.

## Prototype Reference (Pre-existing Concept, Not Yet in This Repo)

A procedural-cat prototype was previously explored (outside this repo) as proof of concept: body/head/ears/eyes/legs/paws/tail built from math + particle distribution via Unity's Particle System, i.e. shape generated from data instead of an asset. The logical next step from that prototype: turn it into a real Creature Generator driven by DNA, then let two parent individuals produce an offspring with inherited + mutated genes.

## Current Repo Status (as of 2026-09-17)

GenesisDiary is still an empty Unity URP scaffold — none of the systems above are implemented yet, including the cat prototype.

## Open Questions / Next Steps

_(to be filled in as design discussion continues)_
