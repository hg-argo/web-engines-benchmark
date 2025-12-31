# Web renders benchmark

This repository is a Unity project developed as a reference for a web render engines benchmark. The goal is to evaluate their ability to generate builds to integrate into web pages or mobile apps.

> We chose Unity for developing the reference project since the author of this benchmark is more used to that engine. That is, the experience of the developer for each candidate engine will be taken in account during the benchmarks, especially for evaluating development time.

## What engines

The candidates for this benchmark are:

- [Unity](https://unity.com) (WebGL export, with editor UI)
- [Godot](https://godotengine.org) (Web export, with editor UI)
- [Phaser](https://phaser.io) (with editor UI)
- [PlayCanvas](https://playcanvas.com) (with editor UI)
- [Defold](https://defold.com) (with editor UI)
- [Three JS](https://threejs.org) (code-first framework)
- [Babylon JS](https://www.babylonjs.com) (code-first framework)

## Benchmark project

For evaluating the engines, we tried to develop the same project on all the candidates, and see how fast (and how *completely*) we achieved it. The project is simple on purpose, but must be rich of feedbacks so it's still allow us to test many expected features for render engines (and is also fun to play).

![Reference project demo](reference.gif)

### Concept overview

3D "penalty" game, meant to be used from a mobile device at first, but since the experience will be hosted on a website, it must support larger display and switching portrait/landscape modes.

The player has to swipe upward to *shoot*, then the goalkeeper (an animated NPC) will try to cacth it. It the goalkeeper misses it, a victory popup is displayed. Otherwise, the player is invited to retry.

#### Shot resolution

The goal has 6 defined "keypoints":

- Up-left corner
- Down-left corner
- Up-right corner
- Down-right corner
- Up-middle
- Down-middle

When the player *shoots*, the game evaluates the length and speed of the swipe gesture and the angle:
- **Swipe speed** is used to validate the input if it reaches a threshold, or invalidate it if the movement was too slow
- **Swipe length** determines if the ball go to "up" or "down" keypoint (representing a strong or light shot)
- **Swipe angle** determines the horizontal position of the keypoint.

As examples, doing a long swipe in up-left diagonal will target up-left corner keypoint ; doing a short swipe straight upward will target down-middle keypoint.

The goalkeeper NPC will pick one of the keypoints at random when the player shots, resulting in a 1/6 chance for the player to miss it.

### Details

- **Camera**: The camera is set behind the ball with specific FOV, so the player can see the ball at the bottom of the screen, and the goal in the upper part of the screen.
- **Lighting**
  - A main directional light to hint scene color (no shadows)
  - 4 cold-white spot lights to emulate bright stadium spots (projecting a typical "cross" shadow on game objects)
- **Controls**
  - **Swipe up**: Shoot the ball (unavailable while victory/retry popup is visible)
  - **Touch (or click)**: Interact with UI (for popups)
- **NPC Goalkeeper**:
  - Fully animated (see *Resources* below)
  - Pick a keypoint at random on shot and play the appropriate animations in sync
- **UI**
  - A tutorial text explaining what to do
  - A "victory + retry" popup (displayed on success)
  - A "retry" popup (displayed on miss)
- **Audio**
  - Stadium ambiance
  - Football kick sound (on shot)
  - Cheering crowd (on success)
  - Whistle (on restart)
- **Background**: Plain black color, with flashes emulating people taking photos
- **Physics**
  - Use 3D physics engine to simulate the shot
  - If not available, make the ball follow a spline
  - If not available, make the ball play a predefined animation
  - Use physics to stop the ball in the net
- **States & Feedbacks**
  - **On Wait**, if the player takes too long before shooting
    - Display in-scene tutorial UI (don't blur the game scene or lock controls, show the content in available space)
  - **On Shot**, we want to emphasize the impact:
    - Hit frame (1-3 frames)
    - Camera shake
    - Particle effects on the ball
    - Temporary FOV light change
    - Temporary post-process effects (color aberrations, light change, ...)
    - Flashes in the background intensifies temporarily
    - Ball shot sound
    - Bullet-time on ball's mid-run, adding tension before resolution
  - **On Success**
    - Play "cheerful" stadium ambiance over the existing one
    - Display "victory + retry" UI
  - **On Miss**
    - Reduce stadium ambiance volume (and/or lowpass if audio effects available)
    - Display "retry" UI
  - **On Display "retry" UI**
    - Blur game scene (if available)
    - Play "slide-in" UI transition
  - **On Hide UI**
    - Unblur game scene (if available)
    - Play "slide-out" UI transition

## What will be evaluated

- Supported 3D model formats (try using all of them during development)
- Browser compatibility
- Global performance
  - Loading time
  - Loading weight
  - Device support (test with both high-end and low-end devices)
- Possibility to interact with game content from external source (website or containing app):
  - Reload the game from external button
  - Get the state of the game (*loading*, *waiting for shot*, *shooting*, *victory*, *missed*)
  - Shoot or retry from external button
  - Handle realtime events for state change (if available)
- Development friction
  - Editor available
  - Switch platform (if available)
  - Resolution and responsiveness handling
  - Feature density

## What won't be evaluated

- Generative AI for games (Unity Muse, Phaser's Beam, ...)

## Resources

The files to use to the projects are stored in the [`Resources/`](./Rsources) folder of this repository. Here is the list of the provided files:

- Goalkeeper animations (all available in `FBX (binary)`, `FBX (ASCII)`, `GLB` and `GLTF`):
  - Idle (skinned; to play repeatedly before the shot or after the goalkeeper missed the ball)
  - Idle holding ball (to play repeatedly after the goalkeeper catched the ball)
  - Dive up-left (to play if the picked keypoint is up-left corner, mirror it for up-right corner)
  - Dive down-left (to play if the picked keypoint is down-left corner, mirror it for down-right corner)
  - Catch upward (to play if the picked keypoint is up-middle)
  - Catch down (to play if the picked keypoint is down-middle)
- Ball texture
- Grass texture
- Goal structure texture
- Goal net texture
- Text font
- Retry icon
- UI popup background
- UI button background with variations:
  - Normal (to use at first)
  - Hover (to use when the mouse is over the button)
  - Active (to use when the mouse clicks the button)
- Sounds (all available in `MP3`, `WAV` and `OGG`):
  - Crowd ambiance
  - Crowd cheering
  - Football kick
  - Whistle

## Credits

- [Mixamo](https://www.mixamo.com) for character model and animations
- [Kenney](https://kenney.nl/assets) for particles & UI sprites
- [Google Fonts](https://fonts.google.com) for font [*Jersey 15*](https://fonts.google.com/specimen/Jersey+15)
- [Pixabay](https://pixabay.com)
- [Sketchup Textures Club](https://www.sketchuptextureclub.com/textures) for environment textures

## Reference project docs

This repository includes a workflow for releasing PC and MacOS builds automatically. We encountered a *no disk space* issue while running a job to build the Web version though, which requires to use Cloud or Self-Hosted runners.

As an alternative for the Web export, we chose to include the [Web build output in the repository](./Builds/PenaltyDemo_WebGL_Brotli) directly. And instead of making it part of the releases, we deploy it to GitHub Pages, making the content available.

In order to make the Web export using Brotli compression, we had to enable an option in Unity: `Player Settings > WebGL > Publishing Settings > Decompression Fallback`. This adds a loader script to the build able to decompress Brotli build client-side, since GitHub Pages doesn't support `Content-Encoding: br` headers. Enabling this causes a heavy overhead (30+ Mo) to the output, but can be avoided if the website that hosts the build can be configured to support the custom header.