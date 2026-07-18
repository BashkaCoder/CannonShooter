# Cannon Shooter - Flexus Games Unity Test Task

A single-scene Unity cannon playground focused on custom projectile simulation, readable ricochets, RenderTexture impact marks, and juicy arcade feedback.

The project was built as a Flexus Games Unity Developer test task. The core requirement is implemented directly in gameplay: the projectile leaves a mark on the hit surface using a renderer texture and explodes on the final ricochet.

## Videos

- Reference gameplay: [Cannon_Reference_gameplay.MP4](Docs/Videos/Cannon_Reference_gameplay.MP4)
- My implementation: [Cannon_Gameplay_Implementation_compressed.mp4](Docs/Videos/Cannon_Gameplay_Implementation_compressed.mp4)

## Quick Start

1. Clone the repository.
2. Pull LFS assets:

```bash
git lfs pull
```

3. Open the project in Unity `6000.3.11f1`.
4. Open `Assets/Scenes/Gameplay.unity`.
5. Press Play.

## Controls

Keyboard and mouse:

- Aim: mouse movement, `WASD`, or arrow keys
- Power: mouse wheel, `Q/E`, or `-/+`
- Shoot: left mouse button, `Space`, or `Enter`

Gamepad:

- Aim: right stick
- Power: D-pad vertical or triggers
- Shoot: south button or right shoulder

The HUD shows current power, yaw, and pitch. Trajectory preview updates immediately when aiming or changing power.

## Implemented Features

- One gameplay scene, no bootstrap scene and no DI container.
- Custom projectile physics without projectile `Rigidbody`.
- No `Physics.Simulate` usage.
- Shared projectile simulation for live flight and trajectory prediction.
- Bounce-aware trajectory line with impact circles.
- Distinct final predicted impact marker.
- Sphere and box/rock projectile variants.
- Procedural projectile meshes with randomized silhouettes.
- Ricochets with reflection, damping, and max bounce count.
- RenderTexture impact marks on the wall and floor.
- Regular and final impact stamps use different textures and sizes.
- Regular impacts spawn smaller VFX and lighter feedback.
- Final impact spawns a stronger explosion, sound, and camera shake.
- Cannon recoil, muzzle flash, projectile trail, impact sounds, and camera shake.
- Replaceable materials, stamps, VFX, and audio through scene/config references.

## Technical Stack

- Unity `6000.3.11f1`
- C#
- Unity Input System `1.19.0`
- UGUI / TextMesh Pro
- LineRenderer
- ParticleSystem
- RenderTexture

No extra gameplay packages are required.

## Project Structure

```text
Assets/
  Scenes/
    Gameplay.unity
  _Project/
    Art/
      Mesh/
      Sounds/
      Textures/
    Materials/
      Decals/
      VFX/
    Prefabs/
    Scripts/
      Cannon/
      Feedback/
      Projectiles/
      Trajectory/
      UI/
    Settings/
    Shaders/
    VFX/
```

## Architecture Notes

The scene is intentionally explicit: gameplay objects reference their dependencies through serialized fields, so a reviewer can inspect the setup without hunting for runtime installers or hidden bootstrap logic.

### Cannon

`CannonController` owns player-facing cannon state: yaw, pitch, power, shape selection, shooting, and HUD/trajectory updates. `CannonSettings` keeps input sensitivity, clamps, power range, and cooldown values tunable from the Inspector.

### Projectiles

Projectiles are simulated manually in `ProjectilePhysicsSimulator`. Each physics step integrates velocity with gravity and drag, then resolves collisions with casts:

- sphere projectile variants use `Physics.SphereCast`;
- box/rock variants use `Physics.BoxCast`.

On hit, velocity is reflected around the hit normal and damped. On the final bounce, the projectile plays final impact feedback and destroys itself.

### Trajectory

`TrajectoryPreview` uses the same simulator as real projectiles, which keeps the preview aligned with actual gameplay. It renders the arc with a `LineRenderer` and places impact markers at predicted bounce points.

### Feedback

`ImpactFeedbackController` centralizes hit feedback. `ImpactFeedbackProfile` separates regular and final impact assets:

- regular VFX, stamp, sound, and shake;
- final VFX, stamp, sound, and shake.

`WallDecalPainter` creates a runtime `RenderTexture`, assigns it to the surface material, and stamps into it through shader blits. This avoids CPU pixel loops per hit and keeps stamp textures easy to replace.

## Asset Swapping

Most presentation assets are data-driven through scene references or ScriptableObject settings:

- projectile materials and trail material;
- trajectory line and marker materials;
- regular/final impact VFX prefabs;
- regular/final decal stamp textures;
- regular/final impact sounds;
- shot sound;
- impact stamp sizes and camera shake values.

The most useful assets to inspect first are:

- `Assets/_Project/Settings/CannonSettings.asset`
- `Assets/_Project/Settings/ProjectilePhysicsSettings.asset`
- `Assets/_Project/Settings/ProjectileShapeCatalog.asset`
- `Assets/_Project/Settings/TrajectorySettings.asset`
- `Assets/_Project/Settings/ImpactFeedbackProfile.asset`
