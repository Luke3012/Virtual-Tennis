# 🎾 **VirtualTennis+**

> *A physics-driven tennis simulation built in Unity — real scoring, real rallies, and smart AI opponents.*  

## 🧠 Overview

**VirtualTennis+** is a real-time tennis simulator built using **Unity** and **C#**. It models authentic scoring, ball–racket physics, and adaptive AI through modular and testable systems.

---

## 🧱 Core Systems

- **🧮 Scoring Engine:**  
  Implements full tennis rules — _points_, _games_, _sets_, _tie-breakers_ — via a deterministic state machine.

- **🎾 Physics Mechanics:**  
  Rigidbody-driven ball flight, **spin**, **surface bounce**, and realistic collisions.

- **🤖 AI Opponents:**  
  Uses finite state machines (FSMs) for _movement_, _shot selection_, and _risk-aware decision making_.

- **🕹️ Input & Cameras:**  
  Unity Input System with cinematic cameras: _broadcast_, _tracking_, _top-down_.

- **🧩 UI/UX:**  
  uGUI or UI Toolkit for scoreboard, serve indicators, match progression.

---

## 🧱 Architecture

| Component          | Description |
|-------------------|-------------|
| `MatchManager`     | Oversees match flow and state transitions |
| `BallController`   | Handles physics, spin, and collision |
| `AIController`     | Controls behavior logic for non-player opponents |
| `ScoreSystem`      | Tracks scoring and event triggers |
| `PlayerController` | Inputs and positioning |

- **📄 Data Driven:**  
  `ScriptableObjects` define players, courts, rackets.  
  `JSON` stores match logs and presets.

- **🔔 Events:**  
  C# Events or `UnityEvents` push updates across systems.

---

## 📦 Platforms

- Desktop: 🖥️ Windows
- Compoanion App: 🤖 Android

---

## 📈 Performance

- Physics runs in `FixedUpdate`  
- UI & Animations run in `Update` / `LateUpdate`  
- Object pooling for trails and particles  
- Minimal GC overhead via `struct` reuse

---

## 🧪 Testing

- ✅ PlayMode: serve/rally logic  
- 🔍 EditMode: state transitions and win conditions  
- 🎯 Deterministic seeds for repeatable AI & rallies

---

## ⚠️ Limitations

- Basic aerodynamics  
- No IK animation or full crowd simulation  
- Simplified line call system

---

## 🚀 Future Work

- Multiplayer (LAN/Online)  
- Injury/stamina system  
- Match replays with scrubbing  
- Doubles mode + bracket view
