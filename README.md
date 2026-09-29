## Overview

<img width="2504" height="1402" alt="Spellweaver_01" src="https://github.com/user-attachments/assets/4772d447-d240-4ec9-8274-b07afdd5c781" />

A 2D side-scrolling action-shooter. Players step into the role of a mage venturing into the Abyss. Through exploration and combat, players collect and learn various spell nodes. By combining or fusing these nodes, you can construct unique, devastating spells to defeat the enemies lurking in the depths.

## Inspiration

While playing **Magic Craft**, I was captivated by its highly flexible "spell programming" system. The ability to create unexpected chemical reactions through pure permutation and combination was incredibly thrilling. This directly sparked my drive to create: I wanted to build a game myself, bringing my wildest ideas for spell combinations to life through code.

## Reference

- **Core Mechanics**: Drew heavy inspiration from **Noita** and **Magic Craft**, specifically extracting their brilliant spell-crafting systems and satisfying character progression loops.
- **Level & Enemy Design**: Referenced classic Metroidvania titles like **Hollow Knight** and **Dead Cells** to design the environment structure and enemy placement, emphasizing verticality and a deep sense of exploration.

## Background Story

<img width="966" height="540" alt="spellweaver-gdd" src="https://github.com/user-attachments/assets/3eabd0e0-5bd5-49e6-a85a-eab6a593d708" />

Legend says the uncharted Abyss holds ancient "Spell Nodes"—and monsters mutated by wild magic. As a young truth-seeking mage, you venture into this forbidden zone alone. You must collect these fragments, "reprogram" them into weapons, and uncover the Abyss's deepest secrets.

## Game Control

- **⌨ WASD + SPACE — Move & Jump**: Navigate swiftly through complex terrains and platforms.
- **🖱 Mouse L/R — Aim & Cast**: Aim the wand with the cursor and left-click to cast your equipped spell.
- **▤ TAB — Spell Editor**: Open the panel anytime to rearrange and tweak spells between fights.

## Game Loop

1. **Defeat Enemies**: Battle mutated monsters to secure resources and clear the path forward.
2. **Explore Map**: Delve deeper into the Abyss across multi-layered, interconnected terrain.
3. **Collect Spells**: Gather spell nodes hidden in hard-to-reach areas and dead-ends.
4. **Try Spell Combos**: Rearrange and fuse nodes to craft new spells, then loop back into combat.

## Gameplay

- **◉ Base Orb**: The core "raw material." It's the carrier for all complex spells. Unlimited casts, but consumes Mana. Without modifiers, it's just a simple, straight-shooting magic projectile.

### Flight Patterns

- **→ Straight**: Default path. Fast, direct, and shoots exactly where you aim.
- **⤾ Bounce**: Ricochets off walls and floors. Allows for tricky angles and shooting around corners.
- **↦ Pierce**: Passes through the first target, dealing high damage to all enemies in a line.
- **◍ Hover**: Stops in mid-air (or at cursor). Acts like a "mine," triggering when enemies approach.

### Elemental Effects

- **🔥 Fire**: Applies a burn effect for damage-over-time (DoT). Great against high-HP enemies.
- **❄ Ice**: Slows or freezes targets on hit. Controls enemy pacing to create safe attack windows.
- **⚡ Lightning**: Features "Chain Lightning." Bounces to nearby enemies upon impact. Perfect for clearing swarms.

## Gameplay: Spell Editor

<img width="963" height="540" alt="spellweaver-spell-editor" src="https://github.com/user-attachments/assets/8a964374-c8e0-4fb8-89f5-f99e7c22bcc6" />

**Spell Editor Logic** — Press Tab anytime to open the Spell Editor. Layout: Top is the Assembly Zone (active skills); bottom is the Inventory (collected spells). Craft & Cast: A Base Orb must be in the top zone to fire. Rearrange nodes freely to alter the spell's final form in real-time, just like coding.

**Spell Crafting Rules** — All crafting requires a Base Orb as the core. The game offers two distinct editing methods:

1. **Chain Sequencing (Reversible)**: Place Modifiers (elements/trajectories) before the Base Orb. The system reads left-to-right to apply traits. Swap nodes anytime for quick combat adaptations.
   - Ex: [Fire] + [Base Orb] = Fireball; [Bounce] + [Base Orb] = Bouncing Orb.
2. **Deep Fusion (Irreversible)**: Drag and hover an Element over a Base Orb to permanently fuse them into a powerful, high-tier spell. Consumes nodes, but yields massive combat advantages.
   - Ex: Fusing [Fire] into a [Base Orb] creates an AoE [Explosion Orb].

## Level Design

<img width="959" height="540" alt="spellweaver-level-design" src="https://github.com/user-attachments/assets/edfa5c05-d172-4d76-aa66-f0317905c2ca" />

Instead of a monotonous linear path, the level features a **multi-layered, interconnected structure**.

I hid key chests (containing rare spell nodes) in vertical hard-to-reach areas or dead-ends off the main path. This utilizes the terrain to naturally guide player curiosity, rewarding exploration with crucial progression resources.

Starting from the safe zone on the left, as players delve deeper into the Abyss, the terrain becomes increasingly rugged. Meanwhile, complex combat encounters mixing ground and aerial enemies steadily ramp up the challenge.

## Code Implementation

<img width="959" height="545" alt="spellweaver-code-art" src="https://github.com/user-attachments/assets/fe5ea86d-2d28-406b-9ebb-3ff6fd649272" />

I led the underlying code architecture and utilized an AI assistant (Kiro) to help tackle the heavy lifting of programming. Through this process, I successfully built the core **Spell Editor** and **Spell Library**, implemented a fluid **character controller**, designed **two distinct enemy AIs**, and developed a **level save system**—bringing my initial design blueprint to life as a fully playable prototype.

---

## Game Video

Watch the Spellweaver prototype in action

[▶ Watch on YouTube](https://www.youtube.com/watch?v=-3qvIr6CLFU)


