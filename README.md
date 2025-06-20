# Cosmic Cleanup Crew

## Overview
Cosmic Cleanup Crew is a Unity-powered 3D space adventure where you pilot a free-floating ship, target floating debris, and “zap” it out of existence. It features fully mouse-driven head rotation, physics-based movement, a zap-and-pool effect, and built-in data logging to CSV for post-play analysis.

## Features
- **Free 6-DOF movement** relative to your view (WASD + Space/Ctrl)  
- **Mouse-controlled look** with adjustable sensitivity and clamped pitch  
- **Quick-zap mechanic**: lock gaze on debris for few secs to destroy, complete with sound 
- **Object pooling** for zap effects to minimize runtime garbage  
- **Data logging** (position, rotation, speed, acceleration, etc.) sampled every 0.5 s to `Assets/PlayerData.csv`  
- **Main menu** with randomized title/subtitle/button text and dynamic colors  
- **Debris manager** that spawns debris, tracks zaps, updates UI, and shows a win message

## Setup

### Open in Unity
1. Launch **Unity Hub**  
2. Click **Add** and select the `cosmic-cleanup` folder  

### Import Packages
- Open **Window → Package Manager**  
- Make sure **TextMeshPro** is installed (required for UI text)

## Usage

### Load Scenes
- **MainMenu**: randomized UI; click **Start Mission** or **Launch Mission** to load the Game scene  
- **Game**: contains your player, DebrisManager, DataLogger, ObjectPool, and UI

### Configure the Player
1. Drag the **PlayerController** prefab (or your GameObject) into the scene  
2. In the Inspector, assign:  
   - **Zap Progress** → UI Image for `zapProgress`  
   - **Zap Effect Prefab** → your `ZapEffect.prefab`  
   - **Zap Sound** → optional AudioClip  
3. Make sure there’s an **ObjectPool** GameObject with the same `zapEffectPrefab` assigned

### Run & Play
- Press ▶️ in the Editor  
- **Move:** W/A/S/D  
- **Ascend/Descend:** Space / Left Ctrl  
- **Look:** Mouse  
- **Zap debris:** Aim + hold until the progress bar fills

## Controls

| Action               | Input                             |
|----------------------|-----------------------------------|
| Thrust forward/back  | W / S                             |
| Strafe left/right    | A / D                             |
| Move up/down         | Space / Left Ctrl                 |
| Look around          | Mouse                             |
| Zap debris           | Aim + hold until bar fills        |


## Script Breakdown
- **PlayerController.cs**  
  Movement, look, zapping logic, and hooks into DataLogger.  
- **DebrisManager.cs**  
  Spawns debris, tracks zaps, updates UI, and ends the game when done.  
- **DataLogger.cs**  
  Buffers gameplay data and writes to `Assets/PlayerData.csv` on game end.  
- **ObjectPool.cs / AutoDestroy.cs**  
  Pools `ZapEffect` instances; returns them  to avoid GC spikes.  
- **MainMenuManager.cs**  
  Randomizes menu text & colors; handles Play/Settings/Quit.  
- **Debris.cs**  
  Highlights debris when targeted by changing its material color.

## Contributing
Feel free to open issues or submit PRs! If you add new features, please update this README with usage examples.

---

Happy zapping! 🚀  
